using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Cors.Infrastructure;
using a2klab.BusinessLogic;
using a2klab.Models;
using a2klab.Services;

namespace a2klab
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public static string databaseName { get; set; }
        public static string containerName { get; set; }
        public static string account { get; set; }
        public static string key { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.SwaggerDoc("v1", new Info
                {
                    Version = "beta",
                    Title = "a2klab",
                    Description = "Microservicios de desarrollo: Kestrel, Core 2.1, Swagger, Source: <a href='https://github.com/unarix/a2klab'>Github</a>",
                    // Contact = new Contact
                    // {
                    //     Name = "ntello",
                    //     Email = "ntello@aa2000.com.ar",
                    // }
                });

                var xmlFile = Assembly.GetExecutingAssembly().GetName().Name +".xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });

            // ********************
            // Setup CORS
            // ********************
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            // corsBuilder.WithOrigins("https://rodocop.azurewebsites.net", 
            //                                 "https://localhost:5001",
            //                                 "https://localhost:5002",
            //                                 "https://localhost:5003",
            //                                 "https://localhost:5004"
            // ); // Para una URL especifica. no agregar / al finals!
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });
            
            //Levanto la configuracion de la solucion desde el AppSettings
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            var settings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            //tratar de hacer el Singleton a la DB Cosmos
            try{                
                services.AddSingleton<ICosmosDBService>(InitializeCosmosClientInstanceAsync(settings).GetAwaiter().GetResult());
            }
            catch(Exception ex){
                // Nothing to do here.
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            // Habilitar Swagger
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "a2k-lab");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "a2k-lab");
                //c.RoutePrefix = "docs";//string.Empty;
                c.RoutePrefix = string.Empty;
                c.InjectStylesheet("/help/custom.css");  
                c.DocumentTitle = "AA2000 devLab";
                //c.("/swagger-ui/custom.js");  
                //c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("/help/index.html");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            
            // ********************
            // USAR CORS!
            // ********************
            app.UseCors("SiteCorsPolicy");
        }

        private static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(AppSettings settings)
        {     
            
            databaseName = settings.CosmosDb.DatabaseName;
            containerName = settings.CosmosDb.ContainerName;
            account = settings.CosmosDb.Account;
            key = settings.CosmosDb.Key;

            CosmosClient client = new CosmosClient(account, key);

            CosmosDBService cosmosDBService = new CosmosDBService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDBService;
        }
    }
}
