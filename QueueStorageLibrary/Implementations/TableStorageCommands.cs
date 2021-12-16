using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderModels;
using QueueStorageLibrary.Interfaces;
using QueueStorageLibrary.Models;
using System;
using System.Threading.Tasks;

namespace QueueStorageLibrary.Implementations
{
  public class TableStorageCommands : ITableStorageCommands
  {
    // private const string TableName = "confirmations";
    private const string PartitionKey = "orders";
    private readonly string _storageConnectionString;
    private readonly ILogger<TableStorageCommands> _logger;
    private readonly CloudTableClient _cloudTableClient;
    private readonly string _confirmationsTableName;
    private readonly string _orderIdTableName;

    /// <summary>
    /// Constructor for the table storage commands
    /// </summary>
    /// <param name="options">The type of options being requested <see cref="ConfigSettings"/></param>
    /// <param name="logger">The implementation of the <see cref="ILogger" /> interface</param>
    /// <param name="cloudTableClient">The implementation of the <see cref="CloudTableClient" /></param>
    public TableStorageCommands(IOptions<ConfigSettings> options, ILogger<TableStorageCommands> logger, CloudTableClient cloudTableClient)
    {
       _storageConnectionString = options.Value.StorageConnectionString;
      _confirmationsTableName = options.Value.ConfirmationTableName;
      _orderIdTableName = options.Value.OrderIdTableName;
      _logger = logger;
      _cloudTableClient = cloudTableClient;
    }

    /// <inheritdoc/>
    public async Task<Confirmation> RetrieveAsync(string orderId)
    {
      Confirmation confirmation = null;

      try
      {
        var retrieveOperation = TableOperation.Retrieve<Confirmation>(PartitionKey, orderId);
        confirmation = await ExecuteTableOperation(retrieveOperation, _confirmationsTableName) as Confirmation;
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
      }

      return confirmation;
    }

    /// <inheritdoc/>
    public async Task<bool> InsertConfirmation(ConfirmationDTO confirmation)
    {
      var insertedConfirmation = new Confirmation
      {
        AgentId = confirmation.AgentId,
        OrderId = confirmation.OrderId,
        OrderStatus = "Processed",
        PartitionKey = PartitionKey,
        RowKey = confirmation.OrderId.ToString()
      };

      var confirmationInserted = InsertOrMergeAsync(insertedConfirmation).Result;

      return await Task.FromResult(confirmationInserted != null);
    }

    private async Task<T> InsertOrMergeAsync<T>(T entity) where T : TableEntity
    {
      T tableEntity = null;

      try
      {
        var insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
        tableEntity = await ExecuteTableOperation(insertOrMergeOperation, _confirmationsTableName) as T;
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
      }

      return tableEntity;
    }

    private async Task<object> ExecuteTableOperation(TableOperation tableOperation, string tableName)
    {
      var table = await GetCloudTable(tableName);

      if (table != null)
      {
        var tableResult = await table.ExecuteAsync(tableOperation);
        return tableResult.Result;
      }

      return null;
    }

    private async Task<CloudTable> GetCloudTable(string tableName)
    {
      CloudTable table = null;

      try
      {
        var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
        var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        table = tableClient.GetTableReference(tableName);
        // table = _cloudTableClient.GetTableReference(tableName);
        await table.CreateIfNotExistsAsync();
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
      }

      return table;
    }
  }
}
