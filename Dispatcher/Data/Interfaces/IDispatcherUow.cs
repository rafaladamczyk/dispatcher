using Dispatcher.Models;

namespace Dispatcher.Data.Interfaces
{
    public interface IDispatcherUow
    {
        void Commit();

        IRepository<Request> Requests { get; }

        IRepository<RequestType> RequestTypes { get; }

        IRepository<ApplicationUser> Users { get; }
    }
}
