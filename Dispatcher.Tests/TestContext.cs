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
            Types = new TestDbSet<DispatchRequestType>();
        }

        public DbSet<DispatchRequest> Requests { get; }
        public DbSet<DispatchRequester> Requesters { get; }
        public DbSet<DispatchRequestType> Types { get; }

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
