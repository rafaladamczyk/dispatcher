using System;
using System.Collections.Generic;
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
        private readonly IDispatcherContext db;

        public DispatchRequestController()
        {
            db = new DispatcherContext();
        }

        public DispatchRequestController(IDispatcherContext context)
        {
            db = context;
        }
        
        [ResponseType(typeof(List<DispatchRequest>))]
        public async Task<IHttpActionResult> GetAllDispatchRequests()
        {
            return Ok(await db.Requests.ToListAsync());
        }

        [HttpGet]
        [Route("api/ActiveRequests")]
        [ResponseType(typeof(List<DispatchRequest>))]
        public async Task<IHttpActionResult> GetActiveRequests()
        {
            var activeRequests = await db.Requests.Where(r => r.Active).ToListAsync() ;
           return Ok(activeRequests);
        }

        [HttpGet]
        [Authorize]
        [Route("api/MyRequests")]
        [ResponseType(typeof(List<DispatchRequest>))]
        public async Task<IHttpActionResult> GetMyRequests()
        {
            var myRequests = await db.Requests.Where(r => r.Active).Where(r => r.ProvidingUserName == User.Identity.Name).ToListAsync();
            return Ok(myRequests);
        }

        [ResponseType(typeof(DispatchRequest))]
        public async Task<IHttpActionResult> GetDispatchRequest(int id)
        {
            DispatchRequest request= await db.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }
        
        [HttpGet]
        [HttpPut]
        [HttpPost]
        [Route("api/CreateRequest/{requesterId}/{requestTypeId}")]
        [ResponseType(typeof(DispatchRequest))]
        public async Task<IHttpActionResult> CreateNewRequest(int requesterId, int requestTypeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = await db.Types.FirstOrDefaultAsync(t => t.Id == requestTypeId);
            if (type == null)
            {
                return BadRequest($"{requestTypeId} is not a valid RequestTypeId.");
            }

            var requester = await db.Requesters.FirstOrDefaultAsync(r => r.Id == requesterId);
            if (requester == null)
            {
                return BadRequest($"Requester Id {requesterId} does not exist");
            }

            var existingRequest = await db.Requests.FirstOrDefaultAsync(r => r.Active && r.RequesterId == requesterId && r.TypeId == requestTypeId);
            if (existingRequest != null)
            {
                return BadRequest($"Requester Id {requesterId} already has an active request of type {requestTypeId}");
            }

            var newRequest = new DispatchRequest { RequesterId = requesterId, Active = true, TypeId = requestTypeId, CreationDate = DateTime.UtcNow, CompletionDate = null, Requester = requester, Type = type};
            db.Requests.Add(newRequest);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { controller = "DispatchRequest",id = newRequest.Id }, newRequest);
        }

        [HttpPut]
        [HttpPost]
        [Route("api/AcceptRequest/{requestId}")]
        [Authorize(Roles = "ServiceProviders")]
        public async Task<IHttpActionResult> AcceptRequest(int requestId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = await db.Requests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
            {
                return BadRequest($"Zlecenie o Id {requestId} nie istnieje");
            }

            if (!request.Active)
            {
                return BadRequest($"Zlecenie o Id {requestId} jest już zakończone");
            }

            if (request.ProvidingUserName != null)
            {
                return Conflict();
            }
            
            request.PickedUpDate = DateTime.UtcNow;
            request.ProvidingUserName = User.Identity.Name;
            await db.SaveChangesAsync();

            return Ok();
        }


        [HttpPut]
        [HttpPost]
        [Route("api/CancelRequest/{requestId}")]
        [Authorize(Roles = "ServiceProviders")]
        public async Task<IHttpActionResult> CancelRequest(int requestId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = await db.Requests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
            {
                return BadRequest($"Zlecenie o Id {requestId} nie istnieje");
            }

            if (request.ProvidingUserName != User.Identity.Name)
            {
                return BadRequest($"Zlecenie o Id {requestId} obsługuje inny użytkownik");
            }

            if (!request.Active)
            {
                return BadRequest($"Zlecenie o Id {requestId} jest już zakończone");
            }

            request.PickedUpDate = null;
            request.ProvidingUserName = null;
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [HttpPut]
        [HttpPost]
        [Route("api/CompleteRequest/{requestId}")]
        [Authorize(Roles = "ServiceProviders")]
        public async Task<IHttpActionResult> CompleteRequest(int requestId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = await db.Requests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
            {
                return BadRequest($"Zlecenie o Id {requestId} nie istnieje");
            }

            if (request.ProvidingUserName != User.Identity.Name)
            {
                return BadRequest($"Zlecenie o Id {requestId} obsługuje inny użytkownik");
            }

            if (!request.Active)
            {
                return BadRequest($"Zlecenie o Id {requestId} jest już zakończone");
            }

            request.Active = false;
            request.CompletionDate = DateTime.UtcNow;
            request.Duration = request.CompletionDate - request.CreationDate;
            request.ServiceDuration = request.CompletionDate - request.PickedUpDate;
            await db.SaveChangesAsync();

            return Ok();
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
