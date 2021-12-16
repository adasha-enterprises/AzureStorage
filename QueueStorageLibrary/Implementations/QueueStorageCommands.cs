using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderModels;
using QueueStorageLibrary.Interfaces;
using QueueStorageLibrary.Models;
using System;
using System.Threading.Tasks;

namespace QueueStorageLibrary.Implementations
{
    public class QueueStorageCommands : IQueueStorageCommands
    {
        private readonly string _storageConnectionString;
        private static readonly object _object = new object();
        private readonly ILogger<QueueStorageCommands> _logger;

        /// <summary>
        /// Constructor for the queue storage commands
        /// </summary>
        /// <param name="options">The type of options being requested <see cref="ConfigSettings"/></param>
        /// <param name="logger">The implementation of the <see cref="ILogger" /> interface</param>
        public QueueStorageCommands(IOptions<ConfigSettings> options, ILogger<QueueStorageCommands> logger)
        {
            _storageConnectionString = options.Value.StorageConnectionString;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<QueueClient> CreateQueueClient(string queueName)
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            return await Task.FromResult(new QueueClient(_storageConnectionString, queueName));
        }

        /// <inheritdoc/>
        public async Task AddMessageToQueue(QueueClient queueClient, string queueName, string message)
        {
            try
            {
                // Create the queue if it doesn't already exist
                await queueClient.CreateIfNotExistsAsync();

                if (await queueClient.ExistsAsync())
                {
                    // Send a message to the queue
                    await queueClient.SendMessageAsync(message);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }    
        }

        /// <inheritdoc/>
        public async Task<(bool, ConfirmationDTO)> ProcessQueueMessage(QueueClient queueClient, Guid agentId, int magicNumber)
        {
            var magicNumberFound = false;
            ConfirmationDTO confirmation = null;

            try
            {
                if (! await queueClient.ExistsAsync())
                {
                    return (false, null);
                }

                // try to get a message from the queue
                while (TryGetRetrievedMessage(queueClient, out QueueMessage[] retrievedMessage))
                {
                    var order = JsonConvert.DeserializeObject<OrderDTO>(retrievedMessage[0].MessageText);

                    lock (_object)
                    {
                        Console.WriteLine($"Received order {order.OrderId}");
                    }

                    magicNumberFound = magicNumber == order.RandomNumber;

                    if (!magicNumberFound)
                    {
                        lock (_object)
                        {
                            Console.WriteLine($"Order Text: {order.OrderText}");
                        }

                        confirmation = new ConfirmationDTO
                        {
                            AgentId = agentId,
                            OrderId = order.OrderId,
                            OrderStatus = "Processed"
                        };

                        // now that the message has been processed, we can delete it from the queue
                        await queueClient.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return (magicNumberFound, confirmation);
        }

        private bool TryGetRetrievedMessage(QueueClient queueClient, out QueueMessage[] retrievedMessage)
        {
            retrievedMessage = null;

            try
            {
                // Get the next message
                retrievedMessage = queueClient.ReceiveMessagesAsync().Result;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return retrievedMessage?.Length > 0;
        }
    }
}
