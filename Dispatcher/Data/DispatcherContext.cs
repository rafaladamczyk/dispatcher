using System.Data.Entity;
using Dispatcher.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dispatcher.Data
{
    public class DispatcherContext : IdentityDbContext<ApplicationUser>
    {
        public DispatcherContext()
            : base("DispatcherContext")
        {
            //Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<Request> Requests { get; set; }

        public DbSet<RequestType> Types { get; set; }

        public DbSet<Machine> Machines { get; set; }

        public static DispatcherContext Create()
        {
            return new DispatcherContext();
        }
    }
}