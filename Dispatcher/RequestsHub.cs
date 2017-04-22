using System.Collections.Generic;
using System.Linq;
using Dispatcher.Models;
using Microsoft.AspNet.SignalR;

namespace Dispatcher
{
    public class RequestsHub : Hub
    {
        public List<DispatchRequest> GetActiveRequests()
        {
            return new DispatcherContext().Requests.Where(r => r.Active).ToList();
        }

        public List<DispatchRequestType> GetRequestTypes()
        {
            return new DispatcherContext().Types.ToList();
        }
    }
}