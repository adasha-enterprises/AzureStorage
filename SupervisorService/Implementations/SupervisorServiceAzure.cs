using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueStorageLibrary.Interfaces;
using SupervisorService.Interfaces;
using QueueStorageLibrary.Models;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SupervisorService.Implementations
{
    public class SupervisorServiceAzure : ISupervisorService
    {
        private readonly IQueueStorageCommands _queueStorageCommands;
        private readonly ITableStorageCommands _tableStorageCommands;
        private readonly ILogger<SupervisorServiceAzure> _logger;
        private const string QueueName = "orders";
        private int _orderId;

        /// <summary>
        /// Constructor for the supervisor service
        /// </summary>
        /// <param name="queueStorageCommands">The implementation of the <see cref="IQueueStorageCommands" /> interface</param>
        /// <param name="tableStorageCommands">The implementation of the <see cref="ITableStorageCommands" /> interface</param>
        /// <param name="logger">The implementation of the <see cref="ILogger" /> interface</param>
        public SupervisorServiceAzure(IQueueStorageCommands queueStorageCommands, ITableStorageCommands tableStorageCommands, ILogger<SupervisorServiceAzure> logger)
        {
            _queueStorageCommands = queueStorageCommands;
            _tableStorageCommands = tableStorageCommands;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ConfirmationDTO> InsertOrderAsync(OrderDTO order)
        {
            ConfirmationDTO confirmation = null;

            try
            {
                order.OrderId = _orderId++;

                var json = JsonConvert.SerializeObject(order, Formatting.Indented);

                var queueClient = await _queueStorageCommands.CreateQueueClient(QueueName);
                await _queueStorageCommands.AddMessageToQueue(queueClient, QueueName, json);

                Console.WriteLine($"Send order #{order.OrderId} with random number {order.RandomNumber}");

                //TODO: need to implement a better solution for getting the confirmation, but for now, just sleep until it's received
                do
                {
                    Thread.Sleep(500);
                    confirmation = await RetrieveAsync(order.OrderId.ToString());
                } while (confirmation == null);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return await Task.FromResult(confirmation);
        }

        /// <inheritdoc/>
        public async Task<ConfirmationDTO> RetrieveAsync(string orderId)
        {
            try
            {
                var retrievedConfirmation = await _tableStorageCommands.RetrieveAsync(orderId);

                if (retrievedConfirmation != null)
                {
                    var confirmation = new ConfirmationDTO
                    {
                        OrderId = retrievedConfirmation.OrderId,
                        OrderStatus = retrievedConfirmation.OrderStatus,
                        AgentId = retrievedConfirmation.AgentId
                    };

                    return confirmation;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }
    }
}
