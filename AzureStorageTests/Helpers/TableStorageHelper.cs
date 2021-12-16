using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrderModels;
using QueueStorageLibrary.Implementations;
using QueueStorageLibrary.Interfaces;
using System;

namespace AzureStorageTests.Helpers
{
  public static class TableStorageHelper
  {
    public static ITableStorageCommands GetTableStorageCommandsInstance()
    {
      var options = new Mock<IOptions<ConfigSettings>>();
      var configSettings = new ConfigSettings { StorageConnectionString = "UseDevelopmentStorage=true" };
      options.Setup(o => o.Value).Returns(configSettings);

      var logger = new Mock<ILogger<TableStorageCommands>>();

      var cloudTableMock = new Mock<CloudTable>(new Uri("http://unittests.localhost.com/FakeTable")
        , (TableClientConfiguration)null);  //apparently Moq doesn't support default parameters 
      //so have to pass null here

      //control what happens when ExecuteAsync is called
      cloudTableMock.Setup(table => table.ExecuteAsync(It.IsAny<TableOperation>()))
        .ReturnsAsync(new TableResult());

      var cloudTableClientMock = new Mock<CloudTableClient>(new Uri("http://localhost")
        , new StorageCredentials(accountName: "blah", keyValue: "blah")
        , (TableClientConfiguration)null);  //apparently Moq doesn't support default parameters 
      //so have to pass null here

      //control what happens when GetTableReference is called
      cloudTableClientMock.Setup(client => client.GetTableReference(It.IsAny<string>()))
        .Returns(cloudTableMock.Object);

      return new TableStorageCommands(options.Object, logger.Object, cloudTableClientMock.Object);
    }
  }
}
