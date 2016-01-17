using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispatcher.Models
{
    public interface IDispatcherContext : IDisposable
    {
        DbSet<DispatchRequest> Requests { get; }
        DbSet<ServiceProvider> Providers { get; }
        DbSet<DispatchRequester> Requesters { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void MarkAsModified<T>(T item) where T : class;
    }
}
