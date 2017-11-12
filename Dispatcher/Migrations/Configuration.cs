using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Dispatcher.Data;
using Dispatcher.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dispatcher.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DispatcherContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DispatcherContext context)
        {
            var admin = AddUser(context, "Admin", "admin@123", "Admin");
            AddRole(context, "TworzenieZlecen");
            AddRole(context, "ObslugaZlecen");

            var type = new RequestType() { ForSelf = false, Id = 1, Name = "Stupid request" };
            context.Types.AddOrUpdate(type);
            context.SaveChanges();
            context.Requests.AddOrUpdate(new Request() { Id = 1, CreationDate = DateTime.UtcNow, CreatorId = admin.Id, TypeId = type.Id });
        }

        private void AddRole(DbContext context, string role)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var newRole = new IdentityRole { Name = role };

            roleManager.Create(newRole);
        }

        private ApplicationUser AddUser(DispatcherContext context, string userName, string password, params string[] roles)
        {
            foreach (var role in roles)
            {
                if (!context.Roles.Any(r => r.Name == role))
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);
                    var newRole = new IdentityRole { Name = role };

                    roleManager.Create(newRole);
                }
            }

            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var user = context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = userName };
                manager.Create(user, password);
            }

            foreach (var role in roles)
            {
                manager.AddToRole(user.Id, role);
            }

            return user;
        }
    }
}
