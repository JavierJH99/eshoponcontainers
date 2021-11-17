using Catalog.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Catalog.API
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
            });
            //services.AddDbContext<CatalogContext>(options => 
            //options.UseSqlServer(
            //    Configuration.GetConnectionString("DefaultConnection")));

            string conexionURI = "ASPNETCORE_ENVIRONMENT";
            string conexion = Environment.GetEnvironmentVariable(conexionURI);
            // En caso de que el entorno de despliegue producción (Docker)
            if (conexion == "Production")
            {
                services.AddDbContext<CatalogContext>(options =>
                {
                    options.UseSqlServer(Configuration
                    .GetConnectionString("SQLServerDockerConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(
                            typeof(Startup).GetTypeInfo()
                            .Assembly.GetName().Name);
                            //Configuring Connection Resiliency:
                            sqlOptions.
                            EnableRetryOnFailure(maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                });
            }
            else
            {
                services.AddDbContext<CatalogContext>(options => options
                .UseSqlServer(Configuration
                .GetConnectionString("DefaultConnection")));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            DatabaseManagementService.MigrationInitialisation(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
