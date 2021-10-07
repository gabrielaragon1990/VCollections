using System;
using System.Linq;
using VC = VCollections.VCollection;

namespace VCollections
{
    public class VDictionary<TKey, TValue>
    {
        #region Content of the dictionary

        private TKey[] _keys;
        private TValue[] _values;

        #endregion

        #region Public Properties

        public TKey[] Keys => !IsEmpty ? (TKey[])_keys.Clone() : new TKey[0];

        public TValue[] Values => !IsEmpty ? (TValue[])_values.Clone() : new TValue[0];

        public TValue this[TKey key]
        {
            get
            {
                if (key == null) throw new InvalidOperationException("The defined key can not be null");
                if (_keys == null) throw new IndexOutOfRangeException("The dictionary is empty");
                int index = VCollection.IndexOf(ref _keys, key);
                if (index == -1) throw new IndexOutOfRangeException("The defined key does not exist");
                return _values[index];
            }
            set
            {
                if (key == null) throw new InvalidOperationException("The defined key can not be null");
                if (_keys == null) throw new IndexOutOfRangeException("The dictionary is empty");
                TValue aux = value;
                if (aux == null) throw new InvalidOperationException("The defined value can not be null");
                if (!ContainsKey(key)) throw new IndexOutOfRangeException("The defined key does not exist");
                _values[VCollection.IndexOf(ref _keys, key)] = aux;
            }
        }

        public int Capacity { private set; get; }

        public int Count => _keys?.Length ?? 0;

        public bool IsEmpty => Count == 0;

        public bool IsFull => Count == Capacity;

        public int LastIndex => Count - 1;

        #endregion

        public VDictionary(int capacity)
        {
            _keys = null;
            _values = null;
            Capacity = capacity <= 0 ? int.MaxValue : capacity;
        }

        public VDictionary() : this(int.MaxValue) { }

        public VDictionary(TKey[] keys, TValue[] values, int capacity) : this(capacity)
        {
            if (keys == null || values == null) return;
            if (keys.Length != values.Length) return;
            for (int i = 0; i < keys.Length; i++) Add(keys[i], values[i]);
        }

        public VDictionary(TKey[] keys, TValue[] values) : this(keys, values, int.MaxValue) { }

        #region Public Methods

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new InvalidOperationException("The defined key can not be null");
            else if (ContainsKey(key)) throw new InvalidOperationException(
                "The defined key already exists in the dictionary");
            if (Count == Capacity) throw new IndexOutOfRangeException(
                $"The index is out of range, the capacity of the list is {Capacity}");
            //Add in sequence:
            VC.Add(ref _keys, key);
            VC.Add(ref _values, value);
        }

        public bool Remove(TKey key)
        {
            int index;
            var exists = (index = VC.IndexOf(ref _keys, key)) > -1;
            if (exists)
            {
                VC.RemoveAt(ref _keys, index);
                VC.RemoveAt(ref _values, index);
            }
            return exists;
        }

        public bool ContainsKey(TKey key) => VC.Contains(ref _keys, key);

        public bool ContainsValue(TValue value) => VC.Contains(ref _values, value);

        public bool TryToGetKeysByValue(TValue value, out TKey[] keys)
        {
            keys = null;
            TKey[] auxKeys = null;
            VC.For(Count, i =>
            {
                if (_values[i].Equals(value)) VC.Add(ref auxKeys, _keys[i]);
            });
            if (auxKeys != null) keys = auxKeys;
            return keys != null && keys.Length > 0;
        }

        public void SortKeysByDescendingOrder()
        {
            if (Count <= 1) return;
            var ordKeys = (from k in _keys orderby k descending select k).ToArray();
            var ordValues = new TValue[Count];
            VC.For(Count, i => ordValues[i] = this[ordKeys[i]]);
            if (ordKeys.Length != ordValues.Length) return;
            //Set sorted data:
            _keys = ordKeys;
            _values = ordValues;
        }

        public void SortKeysByAscendingOrder()
        {
            if (Count <= 1) return;
            var ordKeys = (from k in _keys orderby k ascending select k).ToArray();
            var ordValues = new TValue[Count];
            VC.For(Count, i => ordValues[i] = this[ordKeys[i]]);
            if (ordKeys.Length != ordValues.Length) return;
            //Set sorted data:
            _keys = ordKeys;
            _values = ordValues;
        }

        public void SortKeyByCustomOrder(Func<TKey[], TKey[]> orderingFunction)
        {
            if (orderingFunction == null || Count <= 1) return;
            var ordKeys = orderingFunction((TKey[])_keys.Clone());
            if (ordKeys == null || ordKeys.Length != _keys.Length) return;
            var ordValues = new TValue[Count];
            VC.For(Count, i => ordValues[i] = this[ordKeys[i]]);
            if (ordKeys.Length != ordValues.Length) return;
            //Set sorted data:
            _keys = ordKeys;
            _values = ordValues;
        }

        public VDictionary<TKey, TValue> GetFilteredDictionary(Func<TKey, bool> condition)
        {
            var filteredKeys = VC.GetFilteredArray(ref _keys, condition);
            var filteredValues = new TValue[filteredKeys != null ? filteredKeys.Length : 0];
            VC.For(Count, i => filteredValues[i] = this[filteredKeys[i]]);
            return new VDictionary<TKey, TValue>(filteredKeys, filteredValues, Capacity);
        }

        public void Foreach(Action<TKey, TValue> iteration) => 
            ForeachKey(k => iteration?.Invoke(k, this[k]));

        public void ForeachKey(Action<TKey> iteration) => VC.Foreach(ref _keys, iteration);

        public void ForeachValue(Action<TValue> iteration) => VC.Foreach(ref _values, iteration);

        public void Clear()
        {
            _keys = null;
            _values = null;
        }

        public void ReleaseAll()
        {
            VC.CleanResource(ref _keys);
            VC.CleanResource(ref _values);
        }

        public VDictionary<TKey, TValue> Clone() => 
            new VDictionary<TKey, TValue>(
                IsEmpty ? new TKey[0] : (TKey[])_keys.Clone(),
                IsEmpty ? new TValue[0] : (TValue[])_values.Clone());

        public override string ToString() => $"Count = {Count}";

        #endregion
    }
}
