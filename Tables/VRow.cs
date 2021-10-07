using System;
using VC = VCollections.VCollection;

namespace VCollections.Tables
{
    public class VRow
    {
        #region Content of Row

        private string[] _fieldNames;
        private VCell[] _cellsValues;

        #endregion

        #region Public Properties

        public string[] FieldNames => _fieldNames != null ? (string[])_fieldNames.Clone() : new string[0];

        public VCell[] FieldValues => _cellsValues != null ? (VCell[])_cellsValues.Clone() : new VCell[0];

        public VCell this[string fieldName]
        {
            get
            {
                if (fieldName == null) throw new InvalidOperationException("The defined field name can not be null");
                if (_fieldNames == null) throw new IndexOutOfRangeException("The row is empty");
                int index = VC.IndexOf(ref _fieldNames, fieldName);
                if (index == -1) throw new IndexOutOfRangeException("The defined field name does not exist");
                return _cellsValues[index];
            }
            internal set
            {
                if (fieldName == null) throw new InvalidOperationException("The defined field name can not be null");
                if (_fieldNames == null) throw new IndexOutOfRangeException("The row is empty");
                var cell = value;
                int index = VC.IndexOf(ref _fieldNames, fieldName);
                if (index == -1) throw new IndexOutOfRangeException("The defined field name does not exist");
                _cellsValues[index] = cell ?? throw new InvalidOperationException("The defined value can not be null");
            }
        }

        public VCell this[int fieldIndex]
        {
            get
            {
                if (!VC.ValidateIndex(Count, fieldIndex)) 
                    throw new IndexOutOfRangeException("The field index is out of range");
                if (_fieldNames == null) throw new IndexOutOfRangeException("The row is empty");
                return _cellsValues[fieldIndex];
            }
            internal set
            {
                if (!VC.ValidateIndex(Count, fieldIndex)) 
                    throw new IndexOutOfRangeException("The field index is out of range");
                if (_fieldNames == null) throw new IndexOutOfRangeException("The row is empty");
                VCell cell = value;
                _cellsValues[fieldIndex] = cell ?? throw new InvalidOperationException("The defined value can not be null");
            }
        }

        public int Count => _fieldNames?.Length ?? 0;

        public bool IsEmpty => Count == 0;

        #endregion

        public VRow(string[] fieldNames, VCell[] fieldValues)
        {
            if (fieldNames == null || fieldValues == null || fieldNames.Length != fieldValues.Length) return;            
            VC.For(fieldValues.Length, i => Add(fieldNames[i], fieldValues[i]));
        }

        #region Public Methods

        public void Add(string fieldName, VCell fieldValue)
        {
            if (string.IsNullOrEmpty(fieldName)) 
                throw new InvalidOperationException("The defined key can not be null");
            else if (ContainsField(fieldName)) 
                throw new InvalidOperationException("The defined key already exists in the dictionary");
            //Add in sequence
            VC.Add(ref _fieldNames, fieldName);
            VC.Add(ref _cellsValues, fieldValue);
        }

        public bool Remove(string fieldName)
        {
            int index;
            bool exists = (index = VC.IndexOf(ref _fieldNames, fieldName)) > 1;
            if (exists)
            {
                VC.RemoveAt(ref _fieldNames, index);
                VC.RemoveAt(ref _cellsValues, index);
            }
            return exists;
        }

        public VRow Join(VRow joinRow)
        {
            if (VC.Comparer(_fieldNames, joinRow._fieldNames) > 0) 
                throw new InvalidOperationException("All the fields between the rows must be different.");
            return new VRow(
                VC.Join(_fieldNames, joinRow.FieldNames),
                VC.Join(_cellsValues, joinRow.FieldValues));
        }

        public bool ContainsField(string fieldName) => VC.Contains(ref _fieldNames, fieldName);

        public bool ContainsValue(object fieldValue) =>
            VC.Contains(ref _cellsValues, c => c.Value != null && c.Equals(fieldValue));

        public VCell[] GetCells(params string[] fieldNames)
        {
            if (fieldNames == null || fieldNames.Length == 0)
                throw new InvalidOperationException("The field names can not be null or empty");
            var cells = new VCell[0];
            VC.Foreach(ref fieldNames, n => VC.Add(ref cells, this[n]));
            return cells;
        }

        public VRow GetFilteredRow(params string[] fieldNames) => new VRow(fieldNames, GetCells(fieldNames));

        public void Foreach(Action<string, VCell> iteration) => 
            ForeachFieldName(fname => iteration?.Invoke(fname, this[fname]));

        public void ForeachFieldName(Action<string> iteration) => VC.Foreach(ref _fieldNames, iteration);

        public void ForeachCellValue(Action<object> iteration) => ForeachCell(cell => iteration?.Invoke(cell.Value));

        public void ForeachCell(Action<VCell> iteration) => VC.Foreach(ref _cellsValues, iteration);

        public void Clear()
        {
            VC.CleanResource(ref _fieldNames);
            VC.CleanResource(ref _cellsValues);
        }

        public VRow Clone() => new VRow(
                IsEmpty ? new string[0] : (string[])_fieldNames.Clone(),
                IsEmpty ? new VCell[0] : (VCell[])_cellsValues.Clone());

        public override string ToString() => $"Row fields = {Count}";

        #endregion
    }
}
