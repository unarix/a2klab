using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace a2klab
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
            // services.AddCors(options =>
            // {
            //     options.AddDefaultPolicy(
            //         builder =>
            //         {
            //             builder.WithOrigins("https://localhost:5003",
            //                                 "hhttps://localhost:5002");
            //         });
            // });

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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
                //c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("/help/index.html"); // requires file to be added as an embedded resource
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
            app.UseCors(builder => builder.WithOrigins("https://rodocop.azurewebsites.net", "https://localhost:5001","https://localhost:5002","https://localhost:5003","https://localhost:5004"));
            app.UseMvc();
        }
    }
}
