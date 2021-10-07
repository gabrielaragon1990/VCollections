using System;
using VC = VCollections.VCollection;

namespace VCollections.Tables
{
    public class ResultsReader
    {
        #region Private Attributes

        private readonly VDictionary<string, VList<VCell>> _data;

        #endregion

        #region Public Properties

        public int ColumnCount => _data.Count;

        public int RowCount { private set; get; }

        public int CurrentIndex { get; private set; }

        public VCell this[string columnName]
        {
            get
            {
                if (!VC.ValidateIndex(RowCount, CurrentIndex))
                    throw new IndexOutOfRangeException("The position index is out of range");
                if (!_data.ContainsKey(columnName))
                    throw new IndexOutOfRangeException("The column name was not found");
                return _data[columnName][CurrentIndex];
            }
        }

        public VCell this[int columnIndex]
        {
            get
            {
                if (!VC.ValidateIndex(RowCount, CurrentIndex))
                    throw new IndexOutOfRangeException("The position index is out of range");
                if (!VC.ValidateIndex(ColumnCount, columnIndex))
                    throw new IndexOutOfRangeException("The column index is out of range");
                return _data.Values[columnIndex][CurrentIndex];
            }
        }

        public string[] ColumnNames => _data.Keys;

        #endregion

        public ResultsReader(string[] columnNames, VCell[][] data)
        {
            if (columnNames == null || columnNames.Length == 0)
                throw new InvalidOperationException("The information is inconsistent");
            if (data == null) data = new VCell[0][];

            _data = new VDictionary<string, VList<VCell>>();
            CurrentIndex = -1;

            RowCount = data == null ? 0 : data.Length;

            VC.Foreach(ref columnNames, col => _data.Add(col, new VList<VCell>()));

            if (data.Length > 0)
                VC.Foreach(ref data, row => VC.For(ref columnNames, c => _data[columnNames[c]].Add(row[c])));
        }

        #region Public Methods

        public bool Read() => VC.ValidateIndex(RowCount, ++CurrentIndex);

        public void Reset() => CurrentIndex = -1;

        public VCell[] GetCurrentRowResults()
        {
            if (!VC.ValidateIndex(RowCount, CurrentIndex))
                throw new IndexOutOfRangeException("The position index is out of range");
            var row = new VCell[ColumnCount];
            for (int col = 0; col < ColumnCount; col++) row[col] = this[col];
            return row;
        }

        public object[] GetCurrentRowValues()
        {
            if (!VC.ValidateIndex(RowCount, CurrentIndex))
                throw new IndexOutOfRangeException("The position index is out of range");
            var row = new object[ColumnCount];
            for (int col = 0; col < ColumnCount; col++) row[col] = this[col].Value;
            return row;
        }

        public override string ToString() => 
            $"Result: Columns = {ColumnCount}, Rows = {RowCount}";

        #endregion
    }
}
