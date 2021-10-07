namespace VCollections
{
    public class VHashtable : VDictionary<object, object>
    {
        public VHashtable() : base() { }

        public VHashtable(int capacity) : base(capacity) { }

        public VHashtable(object[] keys, object[] values) : base(keys, values) { }

        public VHashtable(object[] keys, object[] values, int capacity) : base(keys, values, capacity) { }

        public new VHashtable Clone() => new VHashtable(Keys, Values, Capacity);
    }
}
