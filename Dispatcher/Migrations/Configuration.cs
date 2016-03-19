﻿using Dispatcher.Models;
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
                new DispatchRequestType {Id = 1, Name = "Załadunek tira"},
                new DispatchRequestType {Id = 2, Name = "Trociny"});

            
            AddUser(context, "Admin", "admin@123", "Admin", "ObslugaZlecen", "TworzenieZlecen");
            AddUser(context, "Rafal", "12341234", "ObslugaZlecen");
            AddUser(context, "Konrad", "12341234", "ObslugaZlecen");
        }

        private void AddUser(DispatcherContext context, string userName, string password, params string[] roles)
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
        }
    }
}
