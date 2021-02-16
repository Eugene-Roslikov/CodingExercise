using Azure.Data.Tables;
using Azure.Storage.Queues;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace SupervisorApi.Services
{
    public class RepositoryService: IRepositoryService
    {
        private const string sendOrderMessage = "Send order #{0} with random number {1}";
        private readonly QueueClient queueClient;
        private readonly TableClient tableClient;
        private int OrderId = 0;
        public RepositoryService(QueueClient queueClient, TableClient tableClient)
        {
            this.queueClient = queueClient;
            this.tableClient = tableClient;
        }

        public IEnumerable<Confirmation> GetAllOrderConfirmations()
        {
            return tableClient.Query<Confirmation>();
        }

        public Order SendOrder(Order order)
        {
            order.OrderId = GetNextOrderId();
            order.RandomNumber = RandomNumberGenerator.GetInt32(1, 10);
            queueClient.SendMessage(JsonSerializer.Serialize<Order>(order));

            Console.WriteLine(sendOrderMessage, order.OrderId, order.RandomNumber);
            return order;
        }

        private int GetNextOrderId()
        {
            // This minimalist implementation is not atomic and makes service statefull
            // So it is not for production
            return OrderId++;
        }
    }
}
