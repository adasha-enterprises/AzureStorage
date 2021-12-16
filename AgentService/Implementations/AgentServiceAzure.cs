using AgentService.Interfaces;
using Microsoft.Extensions.Logging;
using QueueStorageLibrary.Interfaces;
using QueueStorageLibrary.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AgentService.Implementations
{
    public class AgentServiceAzure : IAgentService
    {
        private readonly IQueueStorageCommands _queueStorageCommands;
        private readonly ITableStorageCommands _tableStorageCommands;
        private readonly ILogger<AgentServiceAzure> _logger;
        private const string QueueName = "orders";
        private static readonly object _object = new object();

        /// <summary>
        /// Constructor for the agent service
        /// </summary>
        /// <param name="queueStorageCommands">The implementation of the <see cref="IQueueStorageCommands" /> interface</param>
        /// <param name="tableStorageCommands">The implementation of the <see cref="ITableStorageCommands" /> interface</param>
        /// <param name="logger">The implementation of the <see cref="ILogger" /> interface</param>
        public AgentServiceAzure(IQueueStorageCommands queueStorageCommands, ITableStorageCommands tableStorageCommands, ILogger<AgentServiceAzure> logger)
        {
            _queueStorageCommands = queueStorageCommands;
            _tableStorageCommands = tableStorageCommands;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task RunAsync(Guid agentId, int magicNumber)
        {
            var magicNumberFound = false;
            ConfirmationDTO confirmation = null;

            await Task.Run(async () =>
            {
                try
                {
                    var queueClient = await _queueStorageCommands.CreateQueueClient(QueueName);

                    while (!magicNumberFound)
                    {
                        (magicNumberFound, confirmation) = await _queueStorageCommands.ProcessQueueMessage(queueClient, agentId, magicNumber);

                        if (confirmation != null && await _tableStorageCommands.InsertConfirmation(confirmation))
                        {
                          lock (_object)
                          {
                            Console.WriteLine("Confirmation has been sent to supervisor");
                          }
                        }

                        Thread.Sleep(500);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            });

            lock (_object)
            {
                Console.WriteLine("Oh no, my magic number was found!");
            }

            while (true)
            {
                Console.ReadLine();  // keep looping until application is forced to quit.
            }
        }
    }
}
