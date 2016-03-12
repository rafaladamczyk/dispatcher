using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dispatcher.Models
{
    public interface IDispatcherContext : IDisposable
    {
        DbSet<DispatchRequest> Requests { get; }
        DbSet<DispatchRequester> Requesters { get; }
        DbSet<DispatchRequestType> Types { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void MarkAsModified<T>(T item) where T : class;
    }
}
