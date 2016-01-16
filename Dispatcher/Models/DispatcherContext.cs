using System.Data.Entity;

namespace Dispatcher.Models
{
    public class DispatcherContext : DbContext
    {
        public DispatcherContext()
            : base()
        {
        }

        public DbSet<DispatchRequest> Requests { get; set; }
        public DbSet<ServiceProvider> Providers { get; set; }
        public DbSet<DispatchRequester> Requesters { get; set; }
    }
}