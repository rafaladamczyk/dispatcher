using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Dispatcher
{
    public class RequestsHub : Hub
    {
        public void Affiramtive(string message)
        {
            throw new ArgumentException(message);
        }
    }
}