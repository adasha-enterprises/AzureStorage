using Microsoft.Azure.Cosmos.Table;
using System;

namespace OrderModels
{
    /// <summary>
    /// Class used to represent a table in Azure storage that stores a confirmation of an order
    /// </summary>
    public class Confirmation : TableEntity
    {
        public int OrderId { get; set; }
        public Guid AgentId { get; set; }
        public string OrderStatus { get; set; }
    }
}
