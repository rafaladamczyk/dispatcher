using Dispatcher.Models;

namespace Dispatcher.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.DispatcherContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.DispatcherContext context)
        {
            context.Providers.AddOrUpdate(r => r.Name, new ServiceProvider { Name = "Rafal Adamczyk" }, new ServiceProvider { Name = "Test Provider" });
            context.Requesters.AddOrUpdate(r => r.Id, new DispatchRequester { Id = 1, Name = "Maszyna taka"}, new DispatchRequester { Id = 2, Name = "Maszyna smaka"});

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
