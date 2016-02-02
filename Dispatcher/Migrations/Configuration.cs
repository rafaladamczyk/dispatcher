using Dispatcher.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dispatcher.Migrations
{
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


            AddUser(context, "Rafal", "sekretnehasloadministratora", "Admin");
            AddUser(context, "Wozkowy", "password123", "ServiceProviders");
            AddUser(context, "Maurycy", "12345678", "ServiceProviders");
        }

        private void AddUser(DispatcherContext context, string userName, string password, string role)
        {
            if (!context.Roles.Any(r => r.Name == role))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var newRole = new IdentityRole { Name = role };

                manager.Create(newRole);
            }

            if (!context.Users.Any(u => u.UserName == userName))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = userName };

                manager.Create(user, password);
                manager.AddToRole(user.Id, role);
            }
        }
    }
}
