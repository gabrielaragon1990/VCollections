using System;
using VC = VCollections.VCollection;

namespace VCollections
{
    public class VList<T> : VCollection<T>
    {
        #region Implicit convertions and implicit operator

        public static VList<T> operator +(VList<T> list1, VList<T> list2) =>
            new VList<T>(VC.Join(list1.ToArray(), list2.ToArray()));

        public static VList<T> operator +(VList<T> list, T[] array) =>
            new VList<T>(VC.Join(list.ToArray(), array));

        public static VList<T> operator +(T[] array, VList<T> list) =>
            new VList<T>(VC.Join(array, list.ToArray()));

        public static VList<T> operator -(VList<T> list1, VList<T> list2)
        {
            var removedList = new VList<T>(list1.ToArray());
            removedList.RemoveRange(list2._content);
            return removedList;
        }

        public static VList<T> operator -(VList<T> list, T[] array)
        {
            var removedList = new VList<T>(list.ToArray());
            removedList.RemoveRange(array);
            return removedList;
        }

        public static VList<T> operator -(T[] array, VList<T> list)
        {
            var removedList = new VList<T>(array);
            removedList.RemoveRange(list._content);
            return removedList;
        }

        public static implicit operator VList<T>(T[] array) => new VList<T>(array);

        public static implicit operator T[](VList<T> list) => list != null ? list.ToArray() : new T[0];

        #endregion

        #region Content of the collection

        internal T[] Content => _content;

        #endregion

        public VList() : base() { }

        public VList(int capacity) : base(capacity) { }

        public VList(T[] elements) : base(elements) { }

        public VList(T[] elements, int capacity) : base(elements, capacity) { }

        #region Public Methods

        public int RemoveWhere(Func<T, bool> condition) => VC.RemoveWhere(ref _content, condition);

        public int IndexOf(Func<T, bool> condition) => VC.IndexOf(ref _content, condition);

        public int IndexOf(int initIndex, Func<T, bool> condition) => VC.IndexOf(ref _content, initIndex, condition);

        public int LastIndexOf(Func<T, bool> condition) => VC.LastIndexOf(ref _content, condition);

        public int LastIndexOf(int initIndex, Func<T, bool> condition) => VC.LastIndexOf(ref _content, initIndex, condition);

        public bool Contains(Func<T, bool> condition) => VC.Contains(ref _content, condition);

        public bool Insert(int index, T element) => VC.Insert(ref _content, index, element);

        public bool InsertRange(int index, T[] elements) => VC.InsertRange(ref _content, index, elements);

        public void For(Func<T, bool> where, Action<int> iteration) => VC.For(ref _content, where, iteration);

        public void Foreach(Func<T, bool> where, Action<T> iteration) => VC.Foreach(ref _content, where, iteration);

        public void Foreach(Func<int, T, bool> where, Action<int, T> iteration) => VC.Foreach(ref _content, where, iteration);

        public void Foreach(int startIndex, int length, Action<T> iteration) => VC.Foreach(ref _content, startIndex, length, iteration);

        public void Foreach(int startIndex, int length, Action<int, T> iteration) => VC.Foreach(ref _content, startIndex, length, iteration);

        public int Comparer(VList<T> list) => VC.Comparer(_content, list._content);

        public int Comparer(T[] array) => VC.Comparer(_content, array);

        public int Comparer(VList<T> listToCompare, Func<T, object> relationValue1, Func<T, object> relationValue2) =>
            VC.Comparer(_content, listToCompare._content, relationValue1, relationValue2);

        public int Comparer(T[] arrayToCompare, Func<T, object> relationValue1, Func<T, object> relationValue2) =>
            VC.Comparer(_content, arrayToCompare, relationValue1, relationValue2);        

        public int ElementCounter(T element) => VC.ElementCounter(ref _content, element);

        public int ElementCounter(Func<T, bool> condition) => VC.ElementCounter(ref _content, condition);

        public void SortByDescendingOrder() => VC.SortByDescendingOrder(ref _content);

        public void SortByAscendingOrder() => VC.SortByAscendingOrder(ref _content);

        public void CustomOrdering(Func<T[], T[]> sortFunction)
        {
            if (Count <= 1 || sortFunction == null) return;
            var sorted = sortFunction(ToArray());
            if (sorted != null && sorted.Length == Count) _content = sorted;
        }

        public VList<T> GetFilteredList(Func<T, bool> condition) =>
            new VList<T>(VC.GetFilteredArray(ref _content, condition));

        public T[] GetFilteredArray(Func<T, bool> condition) => VC.GetFilteredArray(ref _content, condition);

        public T GetElement(int index) => ValidateIndex(index) ? _content[index] : default;

        public T GetFistElement(Func<T, bool> condition)
        {
            int index = IndexOf(condition);
            return ValidateIndex(index) ? _content[index] : default;
        }

        public T GetLastElement(Func<T, bool> condition)
        {
            int index = LastIndexOf(condition);
            return ValidateIndex(index) ? _content[index] : default;
        }

        public T[] GetRange(int initIndex) => VC.GetRange(ref _content, initIndex);

        public T[] GetRange(int initIndex, int quantity) => VC.GetRange(ref _content, initIndex, quantity);

        public override object Clone() => new VList<T>(ToArray(), Capacity);

        public bool ValidateIndex(int index) => index >= 0 && index < Count;

        public void ReleaseAll() => VC.CleanResource(ref _content);

        #endregion
    }
}
