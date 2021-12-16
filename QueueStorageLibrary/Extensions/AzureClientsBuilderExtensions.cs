using Azure.Core.Extensions;
using Microsoft.Azure.Cosmos.Table;
using OrderModels;
using System;

namespace QueueStorageLibrary.Extensions
{
  public static class AzureClientsBuilderExtensions
  {
    /// <summary>
    /// Registers a <see cref="CloudTableClient"/> instance with the provided <paramref name="storageConnectionString"/>
    /// </summary>
    /// <param name="storageConnectionString">Master connection string for accessing the storage account</param>
    /// <param name="setupAction">Configuration options</param>
    /// <returns></returns>
    public static IAzureClientBuilder<CloudTableClient, ConfigSettings> AddCloudTableClient<TBuilder>(this TBuilder builder, string storageConnectionString, Action<ConfigSettings> setupAction)
      where TBuilder : IAzureClientFactoryBuilder
    {
      if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

      return builder.RegisterClientFactory<CloudTableClient, ConfigSettings>(options => CloudStorageAccount.Parse(storageConnectionString).CreateCloudTableClient());
    }

    /// <summary>
    /// Registers a <see cref="CloudTableClient"/> instance with the provided <paramref name="setupAction"/>
    /// </summary>
    /// <param name="setupAction">Configuration options</param>
    /// <returns></returns>
    public static IAzureClientBuilder<CloudTableClient, ConfigSettings> AddCloudTableClient<TBuilder>(this TBuilder builder, Action<ConfigSettings> setupAction)
      where TBuilder : IAzureClientFactoryBuilder
    {
      if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

      return builder.RegisterClientFactory<CloudTableClient, ConfigSettings>(options => CloudStorageAccount.Parse(options.StorageConnectionString).CreateCloudTableClient());
    }

    /// <summary>
    /// Registers a <see cref="CloudTableClient"/> instance with the provided <paramref name="setupAction"/>
    /// </summary>
    /// <param name="configSettings">Configuration options</param>
    /// <returns></returns>
    public static IAzureClientBuilder<CloudTableClient, ConfigSettings> AddCloudTableClient<TBuilder>(this TBuilder builder, ConfigSettings configSettings)
      where TBuilder : IAzureClientFactoryBuilder
    {
      return builder.RegisterClientFactory<CloudTableClient, ConfigSettings>(options => CloudStorageAccount.Parse(configSettings.StorageConnectionString).CreateCloudTableClient());
    }

    ///// <summary>
    ///// Registers a <see cref="CloudTableClient"/> instance with the provided <paramref name="storageConnectionString"/>
    ///// </summary>
    ///// <param name="storageConnectionString">Master connection string for accessing the storage account</param>
    ///// <returns></returns>
    //public static IAzureClientBuilder<CloudTableClient, ConfigSettings> AddCloudTableClient<TBuilder>(this TBuilder builder, string storageConnectionString)
    //  where TBuilder : IAzureClientFactoryBuilder
    //{
    //  return AddCloudTableClient(builder, storageConnectionString, options => { });
    //}

    ///// <summary>
    ///// Registers a <see cref="CloudTable"/> instance
    ///// </summary>
    ///// <param name="storageUri">Base URI of the table storage container, e.g. https://mycontainer.table.core.windows.net</param>
    ///// <param name="sasToken">SAS token scoped to the table to be accessed</param>
    ///// <param name="tableName">Name of the table to be accessed</param>
    ///// <param name="setupAction">Configuration options</param>
    ///// <returns></returns>
    //public static IAzureClientBuilder<CloudTable, ConfigSettings> AddCloudTable<TBuilder>(this TBuilder builder, Uri storageUri, string sasToken, string tableName, Action<ConfigSettings> setupAction)
    //  where TBuilder : IAzureClientFactoryBuilder
    //{
    //  if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

    //  CloudTable cloudTableClient = new CloudTableClient(storageUri, new StorageCredentials(sasToken)).GetTableReference(tableName);
    //  return builder.RegisterClientFactory<CloudTable, ConfigSettings>(options => cloudTableClient);
    //}

    ///// <summary>
    ///// Registers a <see cref="CloudTable"/> instance
    ///// </summary>
    ///// <param name="storageUri">Base URI of the table storage container, e.g. https://mycontainer.table.core.windows.net</param>
    ///// <param name="sasToken">SAS token scoped to the table to be accessed</param>
    ///// <param name="tableName">Name of the table to be accessed</param>
    ///// <returns></returns>
    //public static IAzureClientBuilder<CloudTable, ConfigSettings> AddCloudTable<TBuilder>(this TBuilder builder, Uri storageUri, string sasToken, string tableName)
    //  where TBuilder : IAzureClientFactoryBuilder
    //{
    //  return AddCloudTable(builder, storageUri, sasToken, tableName, options => { });
    //}

    ///// <summary>
    ///// Registers a <see cref="CloudTable"/> instance
    ///// </summary>
    ///// <param name="storageConnectionString">Master connection string for accessing the storage account</param>
    ///// <param name="tableName">Name of the table to be accessed</param>
    ///// <param name="setupAction">Configuration options</param>
    ///// <returns></returns>
    //public static IAzureClientBuilder<CloudTable, ConfigSettings> AddCloudTable<TBuilder>(this TBuilder builder, string storageConnectionString, string tableName, Action<ConfigSettings> setupAction)
    //  where TBuilder : IAzureClientFactoryBuilder
    //{
    //  if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

    //  var cloudTableClient = CloudStorageAccount.Parse(storageConnectionString).CreateCloudTableClient().GetTableReference(tableName);

    //  return builder.RegisterClientFactory<CloudTable, ConfigSettings>(options => cloudTableClient);
    //}

    ///// <summary>
    ///// Registers a <see cref="CloudTable"/> instance
    ///// </summary>
    ///// <param name="storageConnectionString">Master connection string for accessing the storage account</param>
    ///// <param name="tableName">Name of the table to be accessed</param>
    ///// <param name="setupAction">Configuration options</param>
    ///// <returns></returns>
    //public static IAzureClientBuilder<CloudTable, ConfigSettings> AddCloudTable<TBuilder>(this TBuilder builder, Action<ConfigSettings> setupAction)
    //  where TBuilder : IAzureClientFactoryBuilder
    //{
    //  if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

    //  return builder.RegisterClientFactory<CloudTable, ConfigSettings>(options => CloudStorageAccount.Parse(options.StorageConnectionString).CreateCloudTableClient().GetTableReference(options.ConfirmationTableName));
    //}
  }
}
