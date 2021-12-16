using AzureStorageTests.Helpers;
using QueueStorageLibrary.Models;
using Xunit;

namespace AzureStorageTests
{
  public class TableStorageTests
  {
    [Fact]
    public void CanCreateQueueClient()
    {
      // arrange
      var tableStorageCommands = TableStorageHelper.GetTableStorageCommandsInstance();

      // act
      var confirmationInserted = tableStorageCommands.InsertConfirmation(new ConfirmationDTO()).Result;

      // assert
      Assert.True(confirmationInserted);
    }
  }
}
