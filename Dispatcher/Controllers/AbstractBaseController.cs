using System;
using System.Web.Http;
using Dispatcher.Data.Interfaces;

namespace Dispatcher.Controllers
{
    public abstract class AbstractBaseController : ApiController
    {
        protected IDispatcherUow Uow { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (Uow != null && Uow is IDisposable)
            {
                ((IDisposable)Uow).Dispose();
                Uow = null;
            }
            base.Dispose(disposing);
        }
    }
}