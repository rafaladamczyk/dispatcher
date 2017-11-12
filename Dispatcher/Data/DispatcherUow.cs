using System;
using System.Threading;
using Dispatcher.Data.Interfaces;
using Dispatcher.Models;

namespace Dispatcher.Data
{
    public class DispatcherUow : IDispatcherUow, IDisposable
    {
        private DispatcherContext DbContext { get; set; }
        private Lazy<IRepository<Request>> requests;
        private Lazy<IRepository<RequestType>> requestTypes;
        private Lazy<IRepository<ApplicationUser>> users;
        private Lazy<IRepository<Machine>> machines;

        public DispatcherUow()
        {
            CreateDbContext();
        }

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            //System.Diagnostics.Debug.WriteLine("Committed");
            DbContext.SaveChanges();
        }

        public IRepository<Request> Requests => requests.Value;

        public IRepository<RequestType> RequestTypes => requestTypes.Value;

        public IRepository<ApplicationUser> Users => users.Value;
        public IRepository<Machine> Machines => machines.Value;

        protected void CreateDbContext()
        {
            DbContext = new DispatcherContext();

            // Do NOT enable proxied entities, else serialization fails
            //DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = false;

            requests = new Lazy<IRepository<Request>>(() => new EFRepository<Request>(DbContext), LazyThreadSafetyMode.PublicationOnly);
            requestTypes = new Lazy<IRepository<RequestType>>(() => new EFRepository<RequestType>(DbContext), LazyThreadSafetyMode.PublicationOnly);
            users = new Lazy<IRepository<ApplicationUser>>(() => new EFRepository<ApplicationUser>(DbContext), LazyThreadSafetyMode.PublicationOnly);
            machines = new Lazy<IRepository<Machine>>(() => new EFRepository<Machine>(DbContext), LazyThreadSafetyMode.PublicationOnly);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DbContext?.Dispose();
            }
        }
    }
}