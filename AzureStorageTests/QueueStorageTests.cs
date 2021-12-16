using AzureStorageTests.Helpers;
using Newtonsoft.Json;
using QueueStorageLibrary.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AzureStorageTests
{
  public class QueueStorageTests
  {
    private const string QueueName = "test-queue";

    [Fact]
    public void CanCreateQueueClient()
    {
      // arrange
      var queueStorageCommands = QueueStorageHelper.GetQueueStorageCommandsInstance();

      // act
      var queueClient = queueStorageCommands.CreateQueueClient(QueueName).Result;

      // assert
      Assert.NotNull(queueClient);
    }

    [Fact]
    public async Task CanAddMessageToQueue()
    {
      // arrange
      const int orderId = 1;
      var queueClient = QueueStorageHelper.GetMockedQueueClient(orderId);
      var queueStorageCommands = QueueStorageHelper.GetQueueStorageCommandsInstance();

      var orderToRetrieve = QueueStorageHelper.CreateOrder(orderId);
      var orderJson = JsonConvert.SerializeObject(orderToRetrieve);

      // act
      var exception = await Record.ExceptionAsync(async () => await queueStorageCommands.AddMessageToQueue(queueClient.Object, QueueName, orderJson));

      // assert
      Assert.Null(exception);
    }

    [Fact]
    public async Task CanProcessMessages()
    {
      // arrange
      const int orderId = 2;
      var queueClient = QueueStorageHelper.GetMockedQueueClient(orderId);
      var queueStorageCommands = QueueStorageHelper.GetQueueStorageCommandsInstance();

      // act
      ConfirmationDTO confirmation = null;
      var exception = await Record.ExceptionAsync(async () => (_, confirmation) = await queueStorageCommands.ProcessQueueMessage(queueClient.Object, Guid.NewGuid(), 1));

      // assert
      Assert.Null(exception);
      Assert.NotNull(confirmation);
      Assert.Equal(orderId, confirmation.OrderId);
    }
  }
}
