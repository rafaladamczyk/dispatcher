using Dispatcher.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Dispatcher.Models.DispatcherContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Dispatcher.Models.DispatcherContext context)
        {
            context.Requesters.AddOrUpdate(
                r => r.Id,
                new DispatchRequester { Id = 1, Name = "Maszyna taka" },
                new DispatchRequester { Id = 2, Name = "Maszyna smaka" },
                new DispatchRequester { Id = 3, Name = "Maszyna owaka" });

            context.Providers.AddOrUpdate(p => p.Name, new[] { new ServiceProvider { Name = "test provider" }, new ServiceProvider { Name = "operator Jozek" } });


            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "Rafal"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "Rafal" };

                manager.Create(user, "sekretnehasloadministratora");
                manager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
