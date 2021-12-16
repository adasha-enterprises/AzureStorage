using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using OrderModels;
using QueueStorageLibrary.Implementations;
using QueueStorageLibrary.Interfaces;
using QueueStorageLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageTests.Helpers
{
  public static class QueueStorageHelper
  {
    public static QueueMessage GetQueueMessage(string messageBody)
    {
      var queueMessage = QueuesModelFactory.QueueMessage(
        "id2",
        "pr2",
        messageBody,
        1,
        insertedOn: DateTimeOffset.UtcNow);

      var metadata = new Dictionary<string, string> { { "key", "value" }, };
      int messageCount = 5;
      // var queueProp = QueuesModelFactory.QueueProperties(metadata, messageCount);

      return queueMessage;
    }

    public static OrderDTO CreateOrder(int orderId)
    {
      return new OrderDTO
      {
        OrderId = orderId,
        RandomNumber = 6,
        OrderText = "Test Order"
      };
    }

    public static IQueueStorageCommands GetQueueStorageCommandsInstance()
    {
      var options = new Mock<IOptions<ConfigSettings>>();
      var configSettings = new ConfigSettings { StorageConnectionString = "UseDevelopmentStorage=true" };
      options.Setup(o => o.Value).Returns(configSettings);

      var logger = new Mock<ILogger<QueueStorageCommands>>();
      return new QueueStorageCommands(options.Object, logger.Object);
    }

    public static Mock<QueueClient> GetMockedQueueClient(int orderId)
    {
      var orderToRetrieve = CreateOrder(orderId);
      var orderJson = JsonConvert.SerializeObject(orderToRetrieve);

      var response = Mock.Of<Response>();
      var existsResponse = new Mock<Response<bool>>();
      existsResponse.Setup(r => r.Value).Returns(true);

      var queueMessage = GetQueueMessage(orderJson);

      var queueMessageResponse = new Mock<Response<QueueMessage[]>>();
      queueMessageResponse.SetupSequence(m => m.Value)
        .Returns(new[] { queueMessage })
        .Returns(new QueueMessage[0]);

      // queueMessageResponse.Setup(x => x.GetEnumerator()).Returns(queueMessage.GetEnumerator());

      var sendMessageResponse = Mock.Of<Response<SendReceipt>>();

      var queueClient = new Mock<QueueClient>();
      queueClient.Setup(q => q.CreateIfNotExistsAsync(It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));
      queueClient.Setup(q => q.ExistsAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(existsResponse.Object));
      queueClient.Setup(q => q.ReceiveMessagesAsync()).Returns(Task.FromResult(queueMessageResponse.Object));
      queueClient.Setup(q => q.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(response));
      queueClient.Setup(q => q.SendMessageAsync(It.IsAny<string>())).Returns(Task.FromResult(sendMessageResponse));

      return queueClient;
    }
  }
}
