using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderModels;
using QueueStorageLibrary.Extensions;
using QueueStorageLibrary.Implementations;
using QueueStorageLibrary.Interfaces;
using SupervisorService.Implementations;
using SupervisorService.Interfaces;

namespace SupervisorAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SupervisorAPI", Version = "v1" });
            });

            services.Configure<ConfigSettings>(Configuration.GetSection("AppSettings"));

            services.AddSingleton<IQueueStorageCommands, QueueStorageCommands>();
            services.AddSingleton<ITableStorageCommands, TableStorageCommands>();
            services.AddSingleton<ISupervisorService, SupervisorServiceAzure>();

            var configSettings = Configuration.GetSection("AppSettings").Get<ConfigSettings>();

            services.AddAzureClients(builder =>
              builder.AddCloudTableClient(configSettings)
              //builder.AddCloudTableClient(options =>
              //{
              //  options.StorageConnectionString = configSettings.StorageConnectionString;
              //  options.OrderIdTableName = "OrderId";
              //})
              //builder.AddCloudTableClient(configSettings.StorageConnectionString, options =>
              //{
              //  options.ConfirmationTableName = configSettings.ConfirmationTableName;
              //})
              //builder.AddCloudTable(configSettings.StorageConnectionString, configSettings.ConfirmationTableName, options =>
              //{
              //  options.ConfirmationTableName = configSettings.ConfirmationTableName;
              //})
              //builder.AddCloudTable(options =>
              //  {
              //    options.StorageConnectionString = configSettings.StorageConnectionString;
              //    options.ConfirmationTableName = "OrderId";
              //  })
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SupervisorAPI v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
