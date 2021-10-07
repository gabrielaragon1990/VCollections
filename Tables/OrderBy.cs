namespace VCollections.Tables
{
    public struct OrderBy
    {
        public string ColumnName;
        public OrderType OrderType;

        public OrderBy(string columnName, OrderType orderType)
        {
            ColumnName = columnName;
            OrderType = orderType;
        }

        public override string ToString() =>
            $"Order by '{ColumnName}' {(OrderType == OrderType.ASCENDING ? "ASC" : "DESC")}";
    }

    public enum OrderType
    {
        ASCENDING,
        DESCENDING
    }
}
