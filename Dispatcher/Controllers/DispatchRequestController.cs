using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Models;

namespace Dispatcher.Controllers
{
    public class DispatchRequestController : ApiController
    {
        private readonly DispatcherContext db = new DispatcherContext();

        // GET: api/ServiceProviders
        public IQueryable<DispatchRequest> DispatchRequests()
        {
            return db.Requests;
        }

        // GET: api/ServiceProviders/5
        [ResponseType(typeof(DispatchRequest))]
        public async Task<IHttpActionResult> GetDispatchRequest(int id)
        {
            ServiceProvider serviceProvider = await db.Providers.FindAsync(id);
            if (serviceProvider == null)
            {
                return NotFound();
            }

            return Ok(serviceProvider);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
