using Azure;
using Azure.Data.Tables;
using System;

namespace Common
{
    public class Confirmation : ITableEntity
    {
        public int OrderId { get; set; }
        public Guid AgentId { get; set; }
        public string OrderStatus { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
