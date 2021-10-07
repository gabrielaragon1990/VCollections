using System;
using System.Threading;
using VC = VCollections.VCollection;

namespace VCollections
{
    public class VQueue<T>
    {
        #region Content of the Queue and object "Lock"

        private T[] _content;
        private readonly object _objLock;

        #endregion

        #region Public Properties

        public int Count => _content?.Length ?? 0;

        public int Capacity { private set; get; }

        public bool IsEmpty => Count == 0;

        public bool IsFull => Count == Capacity;

        #endregion

        public VQueue(int capacity)
        {
            _content = null;
            _objLock = new object();
            Capacity = capacity <= 0 ? int.MaxValue : capacity;
        }

        public VQueue() : this(int.MaxValue) { }

        public VQueue(T[] elements, int capacity) : this(capacity) => VC.Foreach(ref elements, e => Enqueue(e));

        public VQueue(T[] elements) : this(elements, int.MaxValue) { }

        #region Public Methods

        public void Enqueue(T element)
        {
            if (IsFull) throw new IndexOutOfRangeException(
                "The index is out of range, the queue is full");
            VC.Add(ref _content, element);
        }

        public void SafeEnqueue(T element)
        {
            Monitor.Enter(_objLock);
            if (IsFull) Monitor.Wait(_objLock);
            try
            {
                Enqueue(element);
            }
            catch { }
            Monitor.Pulse(_objLock);
            Monitor.Exit(_objLock);
        }

        public T Dequeue()
        {
            if (IsEmpty) throw new InvalidOperationException("There are not elements in the queue");
            T element = _content[0];
            VC.RemoveAt(ref _content, 0);
            return element;
        }

        public T SafeDequeue()
        {
            Monitor.Enter(_objLock);
            if (IsEmpty) Monitor.Wait(_objLock);
            T element;
            try
            {
                element = Dequeue();
            }
            catch { element = default; }
            Monitor.Pulse(_objLock);
            Monitor.Exit(_objLock);
            return element;
        }

        public T Peek() => !IsEmpty ? _content[0] : default;

        public T Peek(int index) => VC.ValidateIndex(Count, index) ? _content[index] : default;

        public bool Contains(T element)
        {
            lock (_objLock) return VC.Contains(ref _content, element);
        }

        public bool Contains(Func<T, bool> condition)
        {
            lock (_objLock) return VC.Contains(ref _content, condition);
        }

        public int ElementCounter(T element)
        {
            lock (_objLock) return VC.ElementCounter(ref _content, element);
        }

        public int ElementCounter(Func<T, bool> condition)
        {
            lock (_objLock) return VC.ElementCounter(ref _content, condition);
        }

        public void Clear() => _content = null;

        public T[] ToArray() => IsEmpty ? new T[0] : (T[])_content.Clone();

        public VQueue<T> Clone() => new VQueue<T>(ToArray(), Capacity);

        public override string ToString() => $"Count = {Count}";

        #endregion
    }
}
