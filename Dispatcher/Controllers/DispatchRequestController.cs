using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Models;

namespace Dispatcher.Controllers
{
    public class DispatchRequestController : ApiController
    {
        private readonly IDispatcherContext db = new DispatcherContext();

        public DispatchRequestController()
        {
        }

        public DispatchRequestController(IDispatcherContext context)
        {
            db = context;
        }

        public IQueryable<DispatchRequest> DispatchRequests()
        {
            return db.Requests;
        }

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

        [Route("api/DispatchRequest/{requesterId}/{requestType}")]
        [HttpPut]
        [HttpPost]
        [ResponseType(typeof(DispatchRequest))]
        public async Task<IHttpActionResult> CreateNewRequest(int requesterId, int requestType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requester = await db.Requesters.FirstOrDefaultAsync(r => r.Id == requesterId);
            if (requester == null)
            {
                return BadRequest($"Requester Id {requesterId} does not exist");
            }

            if (!Enum.IsDefined(typeof(RequestType), requestType))
            {
                return BadRequest($"{requestType} is not a valid RequestType.");
            }

            var newRequest = new DispatchRequest { RequesterId = requesterId, Type = (RequestType)requestType, CreationDate = DateTime.UtcNow, CompletionDate = null, Requester = requester };
            db.Requests.Add(newRequest);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { controller = "DispatchRequest",id = newRequest.Id }, newRequest);
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
