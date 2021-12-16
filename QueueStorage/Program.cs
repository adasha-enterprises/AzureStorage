using AgentService.Implementations;
using AgentService.Interfaces;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderModels;
using QueueStorageLibrary.Extensions;
using QueueStorageLibrary.Implementations;
using QueueStorageLibrary.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace QueueStorage
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.File(
                        path: "logs/application.log",
                        fileSizeLimitBytes: 10485760,     // 10 MB
                        encoding: System.Text.Encoding.Unicode
                    )
                    // .WriteTo.Console()
                    .CreateLogger();

                using IHost host = CreateHostBuilder(args)
                    .UseSerilog()
                    .Build();

                var agentId = Guid.NewGuid();
                var magicNumber = new Random().Next(1, 10);

                Console.WriteLine($"I'm agent {agentId}, my magic number is {magicNumber}");

                var agentService = host.Services.GetService<IAgentService>();

                if (agentService != null)
                {
                    await agentService.RunAsync(agentId, magicNumber); 
                }

                await host.RunAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }

        /// <summary>
        /// Builds a <see cref="IHostBuilder"/> configured with the appropriate options and services.
        /// </summary>
        /// <param name="args">Startup arguments.</param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddScoped<IQueueStorageCommands, QueueStorageCommands>();
                services.AddScoped<ITableStorageCommands, TableStorageCommands>();
                services.AddScoped<IAgentService, AgentServiceAzure>();

                services.Configure<ConfigSettings>(context.Configuration.GetSection("AppSettings"));

                var configSettings = context.Configuration.GetSection("AppSettings").Get<ConfigSettings>();

                services.AddAzureClients(builder =>
                  builder.AddCloudTableClient(configSettings.StorageConnectionString, options => { })
                  //builder.AddCloudTableClient(options =>
                  //{
                  //  options.StorageConnectionString = configSettings.StorageConnectionString;
                  //  options.OrderIdTableName = "OrderId";
                  //})
                );
            });
        }
    }
}
