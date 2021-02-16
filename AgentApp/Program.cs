using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Common;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;

namespace AgentApp
{
    class Program
    {
        static int numberOfMilliseconds = 1000;     
        static string storageConnectionString = "StorageConnectionString";
        static Guid agentId = Guid.NewGuid();
        static int magicNumber = RandomNumberGenerator.GetInt32(1, 10);
        static readonly string hiMessage = $"I’m agent {agentId}, my magic number is {magicNumber}";
        static string receivedOrderMessage = "Received order {0}";
        static string numberFoundMessage = "Oh no, my magic number was found";
        static string orderStatusProcessed = "Processed";

        static void Main(string[] args)
        {
            Console.WriteLine(hiMessage);

            try
            {
                // Get the connection string from app settings
                string connectionString = ConfigurationManager.AppSettings[storageConnectionString];

                // Instantiate a QueueClient which will be used to create and manipulate the queue
                var queueClient = new QueueClient(connectionString, Names.QueueName);

                // Create the queue
                queueClient.CreateIfNotExists();

                if (!queueClient.Exists())
                {
                    Console.WriteLine("Cannot create Queue. Make sure the storage emulator running and try again.");
                }

                // Instantiate a TableClient which will be used to create and manipulate the table
                var tableClient = new TableClient(connectionString, Names.TableName);

                // Create the table
                tableClient.CreateIfNotExists();
  
                for (;;)
                {
                    // Get the next message
                    QueueMessage retrievedMessage = queueClient.ReceiveMessage();
                    if (null == retrievedMessage)
                    {
                        Thread.Sleep(numberOfMilliseconds);
                        continue;
                    }

                    var order = JsonSerializer.Deserialize<Order>(retrievedMessage.MessageText);
                    if (order.RandomNumber == magicNumber)
                    {
                        Console.WriteLine(numberFoundMessage);
                        break;
                    }

                    // Process the message
                    Console.WriteLine(receivedOrderMessage, order.OrderId);
                    var confirmation = new Confirmation() {
                        OrderId = order.OrderId,
                        AgentId = agentId,
                        OrderStatus = orderStatusProcessed,
                        RowKey = order.OrderId.ToString(),
                        PartitionKey = agentId.ToString()
                    };

                    tableClient.AddEntity(confirmation);

                    // Delete the message
                    queueClient.DeleteMessage(retrievedMessage.MessageId, retrievedMessage.PopReceipt);
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n\n");
                Console.WriteLine("Make sure the storage emulator running and try again.");
            }
        }
    }
}
