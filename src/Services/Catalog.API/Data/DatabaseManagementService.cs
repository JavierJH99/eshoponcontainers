using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public static class DatabaseManagementService
    {
        //It's possible for the application itself to apply migrations
        //programmatically, typically during startup.While productive for local development
        //and testing of migrations, this approach is inappropriate for managing production
        public static async void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // Takes all of our migrations files and apply them against the
                //database in case they are not implemented
                var db = serviceScope.ServiceProvider.GetService<CatalogContext>();
                await db.Database.EnsureCreatedAsync();
                //db.Database.Migrate();
            }
        }
    }
}
