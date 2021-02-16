using Azure.Data.Tables;
using Azure.Storage.Queues;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SupervisorApi.Services;

namespace SupervisorApi
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SupervisorApi", Version = "v1" });
            });
            services.AddSingleton<IRepositoryService>(provider =>
            {
                var connectionString = Configuration["StorageConnectionString"];
                var queueClient = new QueueClient(connectionString, Names.QueueName);

                // Create the queue
                queueClient.CreateIfNotExists();

                var tableClient = new TableClient(connectionString, Names.TableName);

                // Create the table
                tableClient.CreateIfNotExists();
                var repositoryService = new RepositoryService(queueClient, tableClient);
                return repositoryService;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SupervisorApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
