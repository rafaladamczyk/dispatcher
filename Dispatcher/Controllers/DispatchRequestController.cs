using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NUnit.Framework;

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
        [Route("api/CreateRequest/{requesterId}/{requestType}")]
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

            var newRequest = new DispatchRequest { RequesterId = requesterId, Active = true, Type = (RequestType)requestType, CreationDate = DateTime.UtcNow, CompletionDate = null, Requester = requester };
            db.Requests.Add(newRequest);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { controller = "DispatchRequest",id = newRequest.Id }, newRequest);
        }

        [HttpGet]
        [HttpPut]
        [HttpPost]
        [Route("api/CompleteRequest/{requestId}")]
        [ResponseType(typeof(DispatchRequest))]
        public async Task<IHttpActionResult> CompleteRequest(int requestId)
        {
            //TODO only user with correct service provider id can mark request as completed;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = await db.Requests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
            {
                return BadRequest($"Request Id {requestId} does not exist");
            }

            if (!request.Active)
            {
                return BadRequest($"Request Id {requestId} is already completed");
            }

            request.Active = false;
            request.CompletionDate = DateTime.UtcNow;
            request.Duration = request.CompletionDate - request.CreationDate;
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
