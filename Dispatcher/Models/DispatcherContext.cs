using System.Data.Entity;

namespace Dispatcher.Models
{
    public class DispatcherContext : DbContext, IDispatcherContext
    {
        public DispatcherContext()
            : base()
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<DispatchRequest> Requests { get; set; }
        public DbSet<ServiceProvider> Providers { get; set; }
        public DbSet<DispatchRequester> Requesters { get; set; }

        public void MarkAsModified<T>(T item) where T : class
        {
            Entry(item).State = EntityState.Modified;
        }
    }
}