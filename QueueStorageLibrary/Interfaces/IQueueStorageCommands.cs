using Azure.Storage.Queues;
using QueueStorageLibrary.Models;
using System;
using System.Threading.Tasks;

namespace QueueStorageLibrary.Interfaces
{
    public interface IQueueStorageCommands
    {
        /// <summary>
        /// Method to create a queue client
        /// </summary>
        /// <param name="queueName">The queue name to use</param>
        /// <returns><see cref="QueueClient"/></returns>
        Task<QueueClient> CreateQueueClient(string queueName);

        /// <summary>
        /// Method to add a message to the specified queue
        /// </summary>
        /// <param name="queueClient"><see cref="QueueClient"/></param>
        /// <param name="queueName">The queue name to use</param>
        /// <param name="message">The message to be added to the queue</param>
        Task AddMessageToQueue(QueueClient queueClient, string queueName, string message);

        /// <summary>
        /// Method to process messages retrieved from the specified queue
        /// </summary>
        /// <param name="queueClient"><see cref="QueueClient"/></param>
        /// <param name="agentId">The id of the agent processing orders</param>
        /// <param name="magicNumber">A random number between 1 and 10</param>
        /// <returns>A boolean that indicates whether or not the message was processed, as well as a confirmation object</returns>
        Task<(bool, ConfirmationDTO)> ProcessQueueMessage(QueueClient queueClient, Guid agentId, int magicNumber);
    }
}
