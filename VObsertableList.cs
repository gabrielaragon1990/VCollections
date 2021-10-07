using System.Collections.Specialized;

namespace VCollections
{
    public class VObservableList<T> : VCollection<T>, INotifyCollectionChanged
    {
        public override T this[int index]
        {
            get => base[index];
            set
            {
                var old = base[index];
                base[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace, value, old));
            }
        }

        public VObservableList() : base() { }

        public VObservableList(T[] elements) : base(elements) { }

        #region Private Methods

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) =>
            CollectionChanged?.Invoke(this, e);

        #endregion

        #region Public Methods

        public override void Add(T element)
        {
            base.Add(element);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, element));
        }

        public override bool Remove(T element)
        {
            var removed = base.Remove(element);
            if (removed) OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            return removed;
        }

        public override bool RemoveAt(int index)
        {
            var removed = base.RemoveAt(index);
            if (removed) OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            return removed;
        }

        public override bool RemoveRange(T[] elements)
        {
            var removed = base.RemoveRange(elements);
            if (removed) OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
            return removed;
        }

        public override void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public override object Clone() => new VObservableList<T>(_content);

        #endregion

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
