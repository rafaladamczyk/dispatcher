using System.Collections.Generic;
using System.Linq;
using Dispatcher.Data;
using Dispatcher.Models;
using Microsoft.AspNet.SignalR;

namespace Dispatcher
{
    public class RequestsHub : Hub
    {
        public DispatcherUow Uow = new DispatcherUow();

        public List<Request> GetActiveRequests()
        {
            return Uow.Requests.GetAll().Where(r => r.Active).ToList();
        }

        public List<RequestType> GetRequestTypes()
        {
            return Uow.RequestTypes.GetAll().ToList();
        }

        protected override void Dispose(bool disposing)
        {
            Uow.Dispose();
            Uow = null;

            base.Dispose(disposing);
        }
    }
}