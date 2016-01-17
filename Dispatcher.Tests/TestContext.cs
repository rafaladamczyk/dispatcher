using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispatcher.Models;
using NSubstitute.Routing.Handlers;

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
            var task = new Task<int>(() => 0);
            task.Start();
            return task;
        }

        public void MarkAsModified<T>(T item) where T : class
        {
        }

        public void Dispose() { }
    }
}
