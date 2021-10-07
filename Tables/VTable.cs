using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VC = VCollections.VCollection;

namespace VCollections.Tables
{
    public class VTable : IEnumerable<VRow>
    {
        #region Content of the Table

        private VRow[] _rows; //rows, columns -> Order of the data
        private string[] _columnNames;
        private Type[] _types;
        private object[] _defaultValues;

        #endregion

        #region Public Properties

        public int ColumnCount => _columnNames == null ? 0 : _columnNames.Length;

        public int RowCount => _rows == null ? 0 : _rows.Length;

        public string[] ColumnNames => _columnNames;

        public VRow this[int rowIndex]
        {
            get
            {
                if (!VC.ValidateIndex(RowCount, rowIndex))
                    throw new IndexOutOfRangeException("The row index is out of range");
                return _rows[rowIndex];
            }
        }

        public VCell this[int rowIndex, int columnIndex]
        {
            get
            {
                if (!VC.ValidateIndex(RowCount, rowIndex))
                    throw new IndexOutOfRangeException("The row index is out of range");
                if (!VC.ValidateIndex(ColumnCount, columnIndex))
                    throw new IndexOutOfRangeException("The column index is out of range");
                return _rows[rowIndex][columnIndex];
            }
            set
            {
                if (!VC.ValidateIndex(RowCount, rowIndex))
                    throw new IndexOutOfRangeException("The row index is out of range");
                if (!VC.ValidateIndex(ColumnCount, columnIndex))
                    throw new IndexOutOfRangeException("The column index is out of range");
                if (value == null || value.Value == null)
                    throw new InvalidCastException("The field value can not be null");
                if (!ValidateType(value.Value, columnIndex))
                    throw new InvalidCastException("The value provided is not the type of the column");
                _rows[rowIndex][columnIndex].Value = value.Value;
            }
        }

        public VCell this[int rowIndex, string columnName]
        {
            get
            {
                if (!VC.ValidateIndex(RowCount, rowIndex))
                    throw new IndexOutOfRangeException("The row index is out of range");
                if (!VC.Contains(ref _columnNames, columnName))
                    throw new IndexOutOfRangeException("The column name was not found");
                return _rows[rowIndex][GetColumnIndex(columnName)];
            }
            set
            {
                if (!VC.ValidateIndex(RowCount, rowIndex))
                    throw new IndexOutOfRangeException("The row index is out of range");
                if (!VC.Contains(ref _columnNames, columnName))
                    throw new IndexOutOfRangeException("The column name was not found");
                if (value == null || value.Value == null)
                    throw new InvalidCastException("The field value can not be null");
                if (!ValidateType(value.Value, GetColumnIndex(columnName)))
                    throw new InvalidCastException("The value provided is not the type of the column");
                _rows[rowIndex][GetColumnIndex(columnName)].Value = value.Value;
            }
        }

        public int LastIndex => RowCount - 1;

        public VRow First => RowCount > 0 ? this[0] : null;

        public VRow Last => RowCount > 0 ? this[LastIndex] : null;

        #endregion

        public VTable()
        {
            _rows = new VRow[0];
            _columnNames = new string[0];
            _types = new Type[0];
            _defaultValues = new object[0];
        }

        public VTable(VTable table)
        {
            _rows = table._rows;
            _columnNames = table._columnNames;
            _types = table._types;
            _defaultValues = table._defaultValues;
        }

        public VTable(string[] columnNames) : this() => VC.Foreach(ref columnNames, c => CreateColumn(c));

        #region Public Methods

        public void CreateColumn<T>(string columnName, T defaultValue)
        {
            if (!ValidateItemName(columnName))
                throw new InvalidOperationException("The column name is invalid");
            if (VC.Contains(ref _columnNames, columnName))
                throw new InvalidOperationException("The column name is already exist");
            if (defaultValue == null)
                throw new InvalidCastException("The default value can not be null");
            //Default value:
            VC.Add(ref _defaultValues, defaultValue);
            //Type:
            VC.Add(ref _types, defaultValue.GetType());
            //Column name:
            VC.Add(ref _columnNames, columnName);
            //Adding for each row a new field, this represents the new column in the matrix/column
            VC.For(ref _rows, i => _rows[i].Add(columnName, new VCell(defaultValue)));
        }

        public void CreateColumn(string columnName) => CreateColumn(columnName, new object());

        public void DropColumn(string columnName)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (!VC.Contains(ref _columnNames, columnName))
                throw new IndexOutOfRangeException("The column name was not found");
            //Try get the column index:
            int colIndex = GetColumnIndex(columnName);
            if (colIndex > -1)
            { //Remove the column position for each array of the table:
                VC.RemoveAt(ref _defaultValues, colIndex);
                VC.RemoveAt(ref _types, colIndex);
                VC.RemoveAt(ref _columnNames, colIndex);
                VC.For(ref _rows, i => _rows[i].Remove(columnName));
            }
        }

        public int Insert(params object[] values)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (values == null) values = new object[0];
            //Create an array with ColumnCount size:
            var newRow = new VCell[ColumnCount];
            //For each column names:
            VC.For(ref _columnNames, i =>
            {
                object value = null;
                if (VC.ValidateIndex(values.Length, i))
                {
                    if (values[i] == null || !ValidateType(values[i], i))
                        throw new InvalidCastException($"The value can only be of type '{_types[i].FullName}' and can not be null");
                    value = values[i];
                }
                else value = _defaultValues[i];
                newRow[i] = new VCell(value);
            });
            //Adding the new Row:
            VC.Add(ref _rows, new VRow(_columnNames, newRow));
            //Return the last row index:
            return RowCount - 1;
        }

        public int Insert(string[] columnNames, object[] values)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || values == null || columnNames.Length != values.Length)
                throw new InvalidOperationException("The received row information is inconsistent");
            //If a name column is not exists:
            VC.Foreach(ref columnNames, colName =>
            {
                if (!VC.Contains(ref _columnNames, colName))
                    throw new IndexOutOfRangeException($"The column name '{colName}' was not found");
            });
            //Create an array with ColumnCount size:
            var newRow = new VCell[ColumnCount];
            //For each column names:
            VC.For(ref _columnNames, i =>
            {
                object value = null;
                if (VC.Contains(ref columnNames, _columnNames[i]))
                {
                    var auxVal = values[VC.IndexOf(ref columnNames, _columnNames[i])];
                    if (auxVal == null || !ValidateType(auxVal, i))
                        throw new InvalidCastException($"The value can only be of type '{_types[i].FullName}' and can not be null");
                    value = auxVal;
                }
                else value = _defaultValues[i];
                newRow[i] = new VCell(value);
            });
            //Adding the new Row:
            VC.Add(ref _rows, new VRow(_columnNames, newRow));
            //Return the last row index:
            return RowCount - 1;
        }

        public int Delete()
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            int deletedRows = RowCount;
            if (RowCount > 0) _rows = new VRow[0];
            return deletedRows;
        }

        public int Delete(Func<VRow, bool> where)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (where == null) throw new InvalidOperationException("A Where condition has not been defined");
            int lastRowCount = RowCount;
            _rows = (from row in _rows where !@where(row) select row).ToArray();
            return lastRowCount - RowCount;
        }

        public int Delete(int rowIndex)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (!VC.ValidateIndex(RowCount, rowIndex)) return 0;
            VC.RemoveAt(ref _rows, rowIndex);
            return 1;
        }

        public int Update(string[] columnNames, object[] values)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || values == null || columnNames.Length != values.Length)
                throw new InvalidOperationException("The received row information is inconsistent");
            //For each existing row:
            VC.Foreach(ref _rows, upRow =>
                //For each column name:
                VC.For(ref columnNames, i =>
                { //If the row not contains the column, return
                    if (!upRow.ContainsField(columnNames[i]))
                        throw new IndexOutOfRangeException($"The column name '{columnNames[i]}' was not found");
                    //Get the column name index in the Table and validate the value:
                    int _colIndex = VC.IndexOf(ref _columnNames, columnNames[i]);
                    if (values[i] == null || !ValidateType(values[i], _colIndex))
                        throw new InvalidCastException($"The value can only be of type '{_types[_colIndex].FullName}' and can not be null");
                    //Updating the field value:
                    upRow[columnNames[i]].Value = values[i];
                }));
            //Returns the current row count because all have been updated:
            return RowCount;
        }

        public int Update(Func<VRow, bool> where, string[] columnNames, object[] values)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || values == null || columnNames.Length != values.Length)
                throw new InvalidOperationException("The received row information is inconsistent");
            if (where == null) throw new InvalidOperationException("A Where condition has not been defined");
            int updated = 0;
            //For each existing row:
            foreach (var upRow in from row in _rows where @where(row) select row)
            { //For each column name:
                VC.For(ref columnNames, i =>
                { //If the row not contains the column, return
                    if (!upRow.ContainsField(columnNames[i]))
                        throw new IndexOutOfRangeException($"The column name '{columnNames[i]}' was not found");
                    //Get the column name index in the Table and validate the value:
                    int _colIndex = VC.IndexOf(ref _columnNames, columnNames[i]);
                    if (values[i] == null || !ValidateType(values[i], _colIndex))
                        throw new InvalidCastException($"The value can only be of type '{_types[_colIndex].FullName}' and can not be null");
                    //Updating the field value:
                    upRow[columnNames[i]].Value = values[i];
                });
                updated++;
            }
            return updated;
        }

        public int Update(int rowIndex, string[] columnNames, object[] values)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || values == null || columnNames.Length != values.Length)
                throw new InvalidOperationException("The received row information is inconsistent");
            if (!VC.ValidateIndex(RowCount, rowIndex)) throw new IndexOutOfRangeException("The row index is out of range");
            //For each column name:
            VC.For(ref columnNames, i =>
            { //If the row not contains the column, return
                if (!_rows[rowIndex].ContainsField(columnNames[i]))
                    throw new IndexOutOfRangeException($"The column name '{columnNames[i]}' was not found");
                //Get the column name index in the Table and validate the value:
                int _colIndex = VC.IndexOf(ref _columnNames, columnNames[i]);
                if (values[i] == null || !ValidateType(values[i], _colIndex))
                    throw new InvalidCastException($"The value can only be of type '{_types[_colIndex].FullName}' and can not be null");
                //Updating the field value:
                _rows[rowIndex][columnNames[i]].Value = values[i];
            });
            //Returns 1 row updated
            return 1;
        }

        public ResultsReader SelectAll()
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            var cells = new VCell[RowCount][];
            VC.For(ref _rows, i => cells[i] = _rows[i].FieldValues);
            return new ResultsReader(_columnNames, cells);
        }

        public ResultsReader SelectAll(Func<VRow, bool> where)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (where == null) throw new InvalidOperationException("A Where condition has not been defined");
            var cells = new VCell[0][];
            foreach (var selRow in from row in _rows where @where(row) select row) 
                VC.Add(ref cells, selRow.FieldValues);
            return new ResultsReader(_columnNames, cells);
        }

        public ResultsReader SelectAll(OrderBy orderBy)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (orderBy.Equals(default(OrderBy))) 
                throw new InvalidOperationException("No ordering configuration has been defined");
            if (!VC.Contains(ref _columnNames, orderBy.ColumnName))
                throw new IndexOutOfRangeException("The column name was not found");
            //Sort by de column index whit LinQ:
            VRow[] selectedRows = null;
            switch (orderBy.OrderType)
            {
                case OrderType.ASCENDING: selectedRows =
                        (from row in _rows
                         orderby row[orderBy.ColumnName].Value ascending
                         select row).ToArray(); break;
                case OrderType.DESCENDING: selectedRows =
                        (from row in _rows
                         orderby row[orderBy.ColumnName].Value descending
                         select row).ToArray(); break;
            }
            var cells = new VCell[0][];
            VC.Foreach(ref selectedRows, selRow => VC.Add(ref cells, selRow.FieldValues));
            selectedRows = null;
            return new ResultsReader(_columnNames, cells);
        }

        public ResultsReader SelectAll(Func<VRow, bool> where, OrderBy orderBy)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (orderBy.Equals(default(OrderBy)))
                throw new InvalidOperationException("No ordering configuration has been defined");
            if (!VC.Contains(ref _columnNames, orderBy.ColumnName))
                throw new IndexOutOfRangeException("The column name was not found");
            if (where == null) throw new InvalidOperationException("A Where condition has not been defined");
            //Sort by de column index whit LinQ:
            VRow[] selectedRows = null;
            switch (orderBy.OrderType)
            {
                case OrderType.ASCENDING: selectedRows =
                        (from row in _rows
                         where @where(row)
                         orderby row[orderBy.ColumnName].Value ascending
                         select row).ToArray(); break;
                case OrderType.DESCENDING: selectedRows =
                        (from row in _rows
                         where @where(row)
                         orderby row[orderBy.ColumnName].Value descending
                         select row).ToArray(); break;
            }
            var cells = new VCell[0][];
            VC.Foreach(ref selectedRows, selRow => VC.Add(ref cells, selRow.FieldValues));
            selectedRows = null;
            return new ResultsReader(_columnNames, cells);
        }

        public ResultsReader SelectRow(int rowIndex) => SelectRow(rowIndex, _columnNames);

        public ResultsReader SelectRow(int rowIndex, params string[] columnNames)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || columnNames.Length == 0)
                throw new InvalidOperationException("No columns defined in the select method");
            if (!VC.ValidateIndex(RowCount, rowIndex))
                throw new IndexOutOfRangeException("The row index is out of range");
            var cells = new VCell[1][];
            VC.Foreach(ref columnNames, colName =>
            { //If the row not contains the column, return
                if (!_rows[rowIndex].ContainsField(colName))
                    throw new IndexOutOfRangeException($"The column name '{colName}' was not found");
                //Adding the field:
                VC.Add(ref cells[0], _rows[rowIndex][colName]);
            });
            //Returns the values of the row:
            return new ResultsReader(columnNames, cells);
        }

        public ResultsReader Select(params string[] columnNames)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || columnNames.Length == 0)
                throw new InvalidOperationException("No columns defined in the select method");
            var cells = new VCell[0][];
            foreach (var selCells in from row in _rows select GetOnlyRequestedCells(row, columnNames))
                VC.Add(ref cells, selCells);
            return new ResultsReader(columnNames, cells);
        }

        public ResultsReader Select(Func<VRow, bool> where, params string[] columnNames)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || columnNames.Length == 0)
                throw new InvalidOperationException("No columns defined in the select method");
            if (where == null) throw new InvalidOperationException("A Where condition has not been defined");
            var cells = new VCell[0][];
            foreach (var selCells in from row in _rows where @where(row) 
                select GetOnlyRequestedCells(row, columnNames)) VC.Add(ref cells, selCells);
            return new ResultsReader(columnNames, cells);
        }

        public ResultsReader Select(OrderBy orderBy, params string[] columnNames)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || columnNames.Length == 0)
                throw new InvalidOperationException("No columns defined in the select method");
            if (orderBy.Equals(default(OrderBy)))
                throw new InvalidOperationException("No ordering configuration has been defined");
            if (!VC.Contains(ref _columnNames, orderBy.ColumnName))
                throw new IndexOutOfRangeException("The column name was not found");
            var cells = new VCell[0][];
            switch (orderBy.OrderType)
            {
                case OrderType.ASCENDING: foreach (var selCells in 
                    from row in _rows 
                    orderby row[orderBy.ColumnName].Value ascending 
                    select GetOnlyRequestedCells(row, columnNames)) 
                        VC.Add(ref cells, selCells); 
                    break;
                case OrderType.DESCENDING: foreach (var selCells in 
                    from row in _rows
                    orderby row[orderBy.ColumnName].Value descending
                    select GetOnlyRequestedCells(row, columnNames)) 
                        VC.Add(ref cells, selCells);
                    break;
            }
            return new ResultsReader(columnNames, cells);
        }

        public ResultsReader Select(Func<VRow, bool> where, OrderBy orderBy, params string[] columnNames)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("No columns defined in the table");
            if (columnNames == null || columnNames.Length == 0)
                throw new InvalidOperationException("No columns defined in the select method");
            if (where == null) throw new InvalidOperationException("A Where condition has not been defined");
            if (orderBy.Equals(default(OrderBy)))
                throw new InvalidOperationException("No ordering configuration has been defined");
            if (!VC.Contains(ref _columnNames, orderBy.ColumnName))
                throw new IndexOutOfRangeException("The column name was not found");
            var cells = new VCell[0][];
            switch (orderBy.OrderType)
            {
                case OrderType.ASCENDING: foreach (var selCells in 
                    from row in _rows
                    where @where(row)
                    orderby row[orderBy.ColumnName].Value ascending
                    select GetOnlyRequestedCells(row, columnNames)) 
                        VC.Add(ref cells, selCells);
                    break;
                case OrderType.DESCENDING: foreach (var selCells in 
                    from row in _rows
                    where @where(row)
                    orderby row[orderBy.ColumnName].Value descending
                    select GetOnlyRequestedCells(row, columnNames)) 
                        VC.Add(ref cells, selCells);
                    break;
            }
            return new ResultsReader(columnNames, cells);
        }

        public void OrderBy(OrderBy orderBy)
        {
            if (orderBy.Equals(default(OrderBy)))
                throw new InvalidOperationException("No ordering configuration has been defined");
            if (!VC.Contains(ref _columnNames, orderBy.ColumnName))
                throw new IndexOutOfRangeException("The column name was not found");
            //Sort by de column index whit LinQ:
            VRow[] ordered = null;
            switch (orderBy.OrderType)
            {
                case OrderType.ASCENDING: ordered =
                    (from row in _rows orderby row[orderBy.ColumnName].Value ascending select row).ToArray(); break;
                case OrderType.DESCENDING: ordered = 
                    (from row in _rows orderby row[orderBy.ColumnName].Value descending select row).ToArray(); break;
            }
            //If the ordering method have all the rows:
            if (ordered != null && ordered.Length == RowCount) _rows = ordered;
            ordered = null;
        }

        public void CustomOrdering(Func<VRow[], VRow[]> sortFunction)
        {
            if (RowCount <= 1) return;
            if (sortFunction == null) throw new InvalidOperationException("The sort function can not be null");
            var ordered = sortFunction((VRow[])_rows.Clone());
            if (ordered == null || ordered.Length != RowCount) throw new InvalidOperationException(
                "The sort function must return the same number of rows as those contained in the current table");
            _rows = ordered;
        }

        public int IndexOf(Func<VRow, bool> condition) => VC.IndexOf(ref _rows, condition);

        public int LastIndexOf(Func<VRow, bool> condition) => VC.LastIndexOf(ref _rows, condition);

        public VRow GetDefaultRow() => GetDefaultRow((object[])_defaultValues.Clone());

        public VRow GetDefaultRow(object[] cellValues)
        {
            if (ColumnCount == 0) throw new InvalidOperationException("There are not columns in the table");
            var defaultCells = new VCell[ColumnCount];
            VC.For(ref cellValues, i => defaultCells[i] = new VCell(cellValues[i]));
            return new VRow(ColumnNames, defaultCells);
        }

        public void For(Action<int> iteration) => VC.For(ref _rows, iteration);

        public void Foreach(Action<VRow> iteration) => VC.Foreach(ref _rows, iteration);

        public void Foreach(Action<int, VRow> iteration) => VC.Foreach(ref _rows, iteration);

        public VList<VRow> GetFirstRows(int rowCount)
        {
            if (rowCount < 0) rowCount = 0;
            var rowList = new VList<VRow>();
            if (RowCount > 0)
            {
                if (RowCount <= rowCount) Foreach(r => rowList.Add(r));
                else VC.For(0, rowCount - 1, i => rowList.Add(this[i]));
            }
            return rowList;
        }

        public VRow[] GetFirstRowsAsArray(int rowCount)
        {
            if (rowCount < 0) rowCount = 0;
            var rowArray = new VRow[0];
            if (RowCount > 0)
            {
                if (RowCount <= rowCount) Foreach(r => VC.Add(ref rowArray, r));
                else VC.For(0, rowCount - 1, i => VC.Add(ref rowArray, this[i]));
            }
            return rowArray;
        }

        public bool GetFirstRow(Action<VRow> onFirstRow)
        {
            bool ok;
            if (ok = First != null) onFirstRow?.Invoke(First);
            return ok;
        }

        public bool GetLastRow(Action<VRow> onLastRow)
        {
            bool ok;
            if (ok = Last != null) onLastRow?.Invoke(Last);
            return ok;
        }

        public IEnumerator<VRow> GetEnumerator() => ((IEnumerable<VRow>)_rows).GetEnumerator();        

        public override string ToString() => $"Row count = {RowCount}, Column count = {ColumnCount}";

        #endregion

        #region Private Methods

        private int GetColumnIndex(string columnName) => VC.IndexOf(ref _columnNames, columnName);

        private bool ValidateItemName(string name) => name != null && name != string.Empty;

        private bool ValidateType(object value, int columnIndex) => value != null &&
            (_types[columnIndex] is object || _types[columnIndex].Equals(value.GetType()) || value == DBNull.Value);

        private VCell[] GetOnlyRequestedCells(VRow row, string[] requestedColumns)
        {
            var cells = new VCell[0];
            VC.Foreach(ref requestedColumns, cName =>
            {
                if (!VC.Contains(ref _columnNames, cName))
                    throw new IndexOutOfRangeException("The column name was not found");
                VC.Add(ref cells, row[cName]);
            });
            return cells;
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<VRow>)_rows).GetEnumerator();

        #endregion
    }
}
