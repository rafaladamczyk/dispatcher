using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dispatcher.Models
{
    public class DispatcherContext : IdentityDbContext<ApplicationUser>, IDispatcherContext
    {
        public DispatcherContext()
            : base("DispatcherContext")
        {
            //Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<DispatchRequest> Requests { get; set; }
        public DbSet<DispatchRequestType> Types { get; set; }

        public void MarkAsModified<T>(T item) where T : class
        {
            Entry(item).State = EntityState.Modified;
        }
        public static DispatcherContext Create()
        {
            return new DispatcherContext();
        }
    }
}