using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VCollections
{
    public class VCollection<T> : IEnumerable<T>
    {
        #region Implicit convertions and implicit operator

        public static VCollection<T> operator +(VCollection<T> list1, VCollection<T> list2) =>
            new VCollection<T>(VCollection.Join(list1.ToArray(), list2.ToArray()));

        public static VCollection<T> operator +(VCollection<T> list, T[] array) =>
            new VCollection<T>(VCollection.Join(list.ToArray(), array));

        public static VCollection<T> operator +(T[] array, VCollection<T> list) =>
            new VCollection<T>(VCollection.Join(array, list.ToArray()));

        public static VCollection<T> operator -(VCollection<T> list1, VCollection<T> list2)
        {
            var removedList = new VCollection<T>(list1.ToArray());
            removedList.RemoveRange(list2._content);
            return removedList;
        }

        public static VCollection<T> operator -(VCollection<T> list, T[] array)
        {
            var removedList = new VCollection<T>(list.ToArray());
            removedList.RemoveRange(array);
            return removedList;
        }

        public static VCollection<T> operator -(T[] array, VCollection<T> list)
        {
            var removedList = new VCollection<T>(array);
            removedList.RemoveRange(list._content);
            return removedList;
        }

        public static implicit operator VCollection<T>(T[] array) => new VCollection<T>(array);

        public static implicit operator T[](VCollection<T> list) => list?.ToArray() ?? new T[0];

        #endregion

        #region Content of the Collection

        protected T[] _content;

        #endregion

        #region Public Properties

        public virtual int Count => _content?.Length ?? 0;

        public virtual int Capacity { private set; get; }

        public virtual bool IsEmpty => Count == 0;

        public virtual bool IsFull => Count == Capacity;

        public virtual T this[int index]
        {
            get
            {
                if (VCollection.ValidateIndex(Count, index)) return _content[index];
                else throw new IndexOutOfRangeException("The index is out of range");
            }
            set
            {
                if (VCollection.ValidateIndex(Count, index)) _content[index] = value;
                else throw new IndexOutOfRangeException("The index is out of range");
            }
        }

        public virtual T First => !IsEmpty ? _content[0] : default;

        public virtual T Last => !IsEmpty ? _content[LastIndex] : default;

        public virtual int LastIndex => Count - 1;

        #endregion

        public VCollection(int capacity)
        {
            _content = null;
            Capacity = capacity <= 0 ? int.MaxValue : capacity;
        }

        public VCollection(T[] elements) : this(elements, int.MaxValue) { }

        public VCollection(T[] elements, int capacity) : this(capacity) => AddRange(elements);

        public VCollection() : this(int.MaxValue) { }

        #region Public Methods

        public virtual void Add(T element)
        {
            if (Count == Capacity) throw new IndexOutOfRangeException(
                $"The index is out of range, the capacity of the list is {Capacity}");
            VCollection.Add(ref _content, element);
        }

        public virtual void AddRange(params T[] elements) => VCollection.Foreach(ref elements, e => Add(e));

        public virtual bool Remove(T element)
        {
            if (element == null) return false;
            return VCollection.Remove(ref _content, element);
        }

        public virtual bool RemoveAt(int index) => VCollection.RemoveAt(ref _content, index);

        public virtual bool RemoveRange(T[] elements) => VCollection.RemoveRange(ref _content, elements);

        public virtual int IndexOf(T element) => VCollection.IndexOf(ref _content, element);

        public virtual int IndexOf(int initIndex, T element) => VCollection.IndexOf(ref _content, initIndex, element);

        public virtual int LastIndexOf(T element) => VCollection.LastIndexOf(ref _content, element);

        public virtual int LastIndexOf(int initIndex, T element) => VCollection.LastIndexOf(ref _content, initIndex, element);

        public void For(Action<int> iteration) => VCollection.For(Count, iteration);

        public void Foreach(Action<T> iteration) => VCollection.Foreach(ref _content, iteration);

        public void Foreach(Action<int, T> iteration) => VCollection.Foreach(ref _content, iteration);

        public virtual bool Contains(T element) => VCollection.Contains(ref _content, element);

        public virtual object Clone() => new VCollection<T>(ToArray(), Capacity);

        public virtual T[] ToArray() => IsEmpty ? new T[0] : (T[])_content.Clone();

        public virtual void Clear() => _content = null;

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_content ?? new T[0]).GetEnumerator();

        public override string ToString() => $"Count = {Count}";

        #endregion

        #region Private Methods

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_content ?? new T[0]).GetEnumerator();

        #endregion
    }

    #region Static class

    public static class VCollection
    {
        public static void Add<T>(ref T[] array, T element)
        {
            if (array == null) array = new T[0];
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = element;
        }

        public static void AddRange<T>(ref T[] array, T[] elements)
        {
            if (array == null) array = new T[0];
            if (elements != null && elements.Length > 0)
                foreach (T e in elements) Add(ref array, e);
        }

        public static bool Remove<T>(ref T[] array, T element)
        {
            int index = IndexOf(ref array, element);
            if (index > -1) return RemoveAt(ref array, index);
            else return false;
        }

        public static int RemoveWhere<T>(ref T[] array, Func<T, bool> condition)
        {
            int removed = 0;
            if (array != null && condition != null) foreach (var e in
                (from element in array
                 where condition(element)
                 select element).ToArray()) Remove(ref array, e);
            return removed;
        }

        public static bool RemoveAt<T>(ref T[] array, int index)
        {
            if (array == null || !ValidateIndex(array.Length, index)) 
                return false;
            RemoveAndSort(ref array, index);
            return true;
        }

        public static bool RemoveRange<T>(ref T[] array, T[] elements)
        {
            var removedElements = Comparer(array, elements) > 0;
            if (removedElements) foreach (T e in elements) Remove(ref array, e);
            return removedElements;
        }

        public static int IndexOf<T>(ref T[] array, T element) => IndexOf(ref array, 0, element);

        public static int IndexOf<T>(ref T[] array, int initIndex, T element)
        {
            int index = -1;
            if (array == null) return index;
            if (!ValidateIndex(array.Length, initIndex)) initIndex = 0;
            for (int i = initIndex; i < array.Length; i++)
                if (array[i].Equals(element)) { index = i; break; }
            return index;
        }

        public static int IndexOf<T>(ref T[] array, Func<T, bool> condition) => IndexOf(ref array, 0, condition);

        public static int IndexOf<T>(ref T[] array, int initIndex, Func<T, bool> condition)
        {
            int index = -1;
            if (array == null || condition == null) return index;
            if (!ValidateIndex(array.Length, initIndex)) initIndex = 0;
            for (int i = initIndex; i < array.Length; i++)
                if (condition(array[i])) { index = i; break; }
            return index;
        }

        public static int LastIndexOf<T>(ref T[] array, T element) => 
            LastIndexOf(ref array, array != null ? array.Length - 1 : 0, element);

        public static int LastIndexOf<T>(ref T[] array, int initIndex, T element)
        {
            int index = -1;
            if (array == null) return index;
            if (!ValidateIndex(array.Length, initIndex)) initIndex = array.Length - 1;
            for (int i = initIndex; i >= 0; i--)
                if (array[i].Equals(element)) { index = i; break; }
            return index;
        }

        public static int LastIndexOf<T>(ref T[] array, Func<T, bool> condition) => 
            LastIndexOf(ref array, array != null ? array.Length : 0, condition);

        public static int LastIndexOf<T>(ref T[] array, int initIndex, Func<T, bool> condition)
        {
            int index = -1;
            if (array == null || condition == null) return index;
            if (!ValidateIndex(array.Length, initIndex)) initIndex = array.Length - 1;
            for (int i = initIndex; i >= 0; i--)
                if (condition(array[i])) { index = i; break; }
            return index;
        }

        public static bool Contains<T>(ref T[] array, T element) => IndexOf(ref array, element) >= 0;

        public static bool Contains<T>(ref T[] array, Func<T, bool> condition)
        {
            var contains = false;
            if (array != null && condition != null)
                for (int i = 0; i < array.Length; i++)
                    if (condition(array[i])) { contains = true; break; }
            return contains;
        }

        public static bool Insert<T>(ref T[] array, int index, T element) => 
            InsertRange(ref array, index, new T[] { element });

        public static bool InsertRange<T>(ref T[] array, int index, T[] elements)
        {
            if (array == null || elements == null || elements.Length == 0) return false;

            T[] finalArray = null;

            if (ValidateIndex(array.Length, index) || (array.Length == 0 && index == 0))
            { //Add first middle of the array
                int i;
                for (i = 0; i < index; i++) Add(ref finalArray, array[i]);
                //Add new elements
                AddRange(ref finalArray, elements);
                //Add last middle of the array
                for (i = index; i < array.Length; i++) Add(ref finalArray, array[i]);
            }

            bool inserted;
            if (inserted = finalArray != null && finalArray.Length > 0)
                array = finalArray;

            return inserted;
        }

        public static void SortByDescendingOrder<T>(ref T[] array)
        {
            if (array == null || array.Length <= 1) return;
            array = (from e in array orderby e descending select e).ToArray();
        }

        public static void SortByDescendingOrder<T>(ref T[] array, Func<T, object> orderingValue)
        {
            if (array == null || array.Length <= 1 || orderingValue == null) return;
            array = (from e in array orderby orderingValue(e) descending select e).ToArray();
        }

        public static void SortByAscendingOrder<T>(ref T[] array)
        {
            if (array == null || array.Length <= 1) return;
            array = (from e in array orderby e ascending select e).ToArray();
        }

        public static void SortByAscendingOrder<T>(ref T[] array, Func<T, object> orderingValue)
        {
            if (array == null || array.Length <= 1 || orderingValue == null) return;
            array = (from e in array orderby orderingValue(e) ascending select e).ToArray();
        }

        public static T[] GetFilteredArray<T>(ref T[] array, Func<T, bool> condition)
        {
            if (array == null || condition == null) return new T[0];
            return (from e in array where condition(e) select e).ToArray();
        }

        public static int ElementCounter<T>(ref T[] array, T element)
        {
            if (array == null || element == null) return 0;
            return (from e in array where e.Equals(element) select e).Count();
        }

        public static int ElementCounter<T>(ref T[] array, Func<T, bool> condition)
        {
            if (array == null) return 0;
            return (from e in array where condition(e) select e).Count();
        }

        public static int Comparer<T>(T[] array1, T[] array2)
        {
            if (array1 == null || array2 == null) return 0;
            return (from e1 in array1 join e2 in array2 on e1 equals e2 select e1).Count();
        }

        public static int Comparer<T>(VList<T> list, T[] array) => Comparer(list.Content, array);

        public static int Comparer<T>(VList<T> list1, VList<T> list2) => Comparer(list1.Content, list2.Content);

        public static int Comparer<T>(T[] array1, T[] array2, Func<T, object> relationValue1, Func<T, object> relationValue2)
        {
            if (array1 == null || array2 == null || relationValue1 == null || relationValue2 == null) return 0;
            return (from e1 in array1 join e2 in array2 on relationValue1(e1) equals relationValue2(e2) select e1).Count();
        }

        public static int Comparer<T>(VList<T> list, T[] array, Func<T, object> relationValue1, Func<T, object> relationValue2) =>
            Comparer(list.Content, array, relationValue1, relationValue2);

        public static int Comparer<T>(VList<T> list1, VList<T> list2, Func<T, object> relationValue1, Func<T, object> relationValue2) =>
            Comparer(list1.Content, list2.Content, relationValue1, relationValue2);

        public static T[] Join<T>(T[] array1, T[] array2)
        {
            if ((array1 == null || array1.Length == 0) && (array2 == null || array2.Length == 0))
                return new T[0];
            var joined = new T[(array1 != null ? array1.Length : 0) + (array2 != null ? array2.Length : 0)];
            int i = 0;
            Foreach(ref array1, e => joined[i++] = e);
            Foreach(ref array2, e => joined[i++] = e);
            return joined;
        }

        public static VList<T> Join<T>(VList<T> list1, VList<T> list2) => new VList<T>(Join(list1.Content, list2.Content));

        public static T[] Join<T>(VList<T> list, T[] array)
        {
            if ((list == null || list.IsEmpty) && (array == null || array.Length == 0))
                return new T[0];
            var joined = new T[(list != null ? list.Count : 0) + (array != null ? array.Length : 0)];
            int i = 0;
            list?.Foreach(e => joined[i++] = e);
            Foreach(ref array, e => joined[i++] = e);
            return joined;
        }

        public static T[] Join<T>(T[] array, VList<T> list) => Join(array, list.Content);

        public static T[] GetRange<T>(ref T[] array, int initIndex) => GetRange(ref array, initIndex, array.Length - initIndex);

        public static T[] GetRange<T>(ref T[] array, int initIndex, int quantity)
        {
            if (array == null || array.Length == 0 || quantity <= 0) return new T[0];
            if (!ValidateIndex(array.Length, initIndex)) return new T[0];
            if (array.Length - initIndex < quantity)
                throw new InvalidOperationException("The specified number of items can not be obtained.");
            var range = new T[quantity];
            for (int i = 0; i < quantity; i++) range[i] = array[initIndex + i];
            return range;
        }

        public static void For(int count, Action<int> iteration)
        {
            for (int i = 0; i < count; i++) iteration?.Invoke(i);
        }

        public static void For(int initIndex, int finalIndex, Action<int> iteration)
        {
            for (int i = initIndex; i <= finalIndex; i++) iteration?.Invoke(i);
        }

        public static void For<T>(ref T[] array, Action<int> iteration)
        {
            if (IsArrayEmpty(array)) return;
            for (int i = 0; i < array.Length; i++) iteration?.Invoke(i);
        }

        public static void For<T>(ref T[] array, Func<T, bool> where, Action<int> iteration)
        {
            if (IsArrayEmpty(array)) return;
            for (int i = 0; i < array.Length; i++)
                if (where?.Invoke(array[i]) ?? true) iteration?.Invoke(i);
        }

        public static void Foreach<T>(ref T[] array, Action<T> iteration)
        {
            if (IsArrayEmpty(array)) return;
            foreach (T e in array) iteration?.Invoke(e);
        }

        public static void Foreach<T>(ref T[] array, Func<T, bool> where, Action<T> iteration)
        {
            if (IsArrayEmpty(array)) return;
            foreach (T e in array) 
                if (where?.Invoke(e) ?? true) iteration?.Invoke(e);
        }

        public static void Foreach<T>(ref T[] array, Action<int, T> iteration)
        {
            if (IsArrayEmpty(array)) return;
            for (int i = 0; i < array.Length; i++) iteration?.Invoke(i, array[i]);
        }

        public static void Foreach<T>(ref T[] array, Func<int, T, bool> where, Action<int, T> iteration)
        {
            if (IsArrayEmpty(array)) return;
            for (int i = 0; i < array.Length; i++) 
                if (where?.Invoke(i, array[i]) ?? true) iteration?.Invoke(i, array[i]);
        }

        public static void Foreach<T>(ref T[] array, int startIndex, int length, Action<T> iteration)
        {
            if (IsArrayEmpty(array)) return;
            for (int i = startIndex; i < length; i++) iteration?.Invoke(array[i]);
        }

        public static void Foreach<T>(ref T[] array, int startIndex, int length, Action<int, T> iteration)
        {
            if (IsArrayEmpty(array)) return;
            for (int i = startIndex; i < length; i++) iteration?.Invoke(i, array[i]);
        }

        public static bool ValidateIndex(int Size, int index) => index >= 0 && index < Size;

        public static void CleanResource<T>(ref T[] array)
        {
            if (!IsArrayEmpty(array)) 
                for (int i = 0; i < array.Length; i++) array[i] = default;
            array = null;
        }

        public static bool In<T>(T value, params T[] array) => Contains(ref array, value);

        public static bool IsArrayEmpty<T>(T[] array) => array == null || array.Length == 0;

        internal static void RemoveAndSort<T>(ref T[] array, int removedIndex)
        {
            if (IsArrayEmpty(array)) return;
            if (ValidateIndex(array.Length, removedIndex))
            {
                if (removedIndex != array.Length - 1)
                    for (; removedIndex < array.Length; removedIndex++)
                    {
                        if (removedIndex < array.Length - 1)
                            array[removedIndex] = array[removedIndex + 1];
                        else break;
                    }
                array[removedIndex] = default;
                Array.Resize(ref array, array.Length - 1);
            }
        }
    }

    #endregion
}
