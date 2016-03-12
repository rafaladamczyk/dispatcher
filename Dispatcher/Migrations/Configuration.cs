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

            context.Types.AddOrUpdate(
                t => t.Id, 
                new DispatchRequestType {Id = 1, Name = "Zabierz rzeczy"},
                new DispatchRequestType {Id = 2, Name = "Przywiez rzeczy"});


            AddUser(context, "Admin", "admin@123", "Admin");
            AddUser(context, "Rafal", "12341234", "ServiceProviders");
            AddUser(context, "Konrad", "12341234", "ServiceProviders");
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
