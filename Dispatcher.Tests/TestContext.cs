using System.Data.Entity;
using System.Threading.Tasks;
using Dispatcher.Models;

namespace Dispatcher.Tests
{
    public class TestContext : IDispatcherContext
    {
        public TestContext()
        {
            Requests = new TestDbSet<DispatchRequest>();
            Requesters = new TestDbSet<DispatchRequester>();
            Providers = new TestDbSet<ServiceProvider>();
        }

        public DbSet<DispatchRequest> Requests { get; }
        public DbSet<ServiceProvider> Providers { get; }
        public DbSet<DispatchRequester> Requesters { get; }

        public int SaveChanges()
        {
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.Factory.StartNew(() => 0);
        }

        public void MarkAsModified<T>(T item) where T : class
        {
        }

        public void Dispose() { }
    }
}
