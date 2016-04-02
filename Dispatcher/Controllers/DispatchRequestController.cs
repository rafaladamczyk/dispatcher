using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Models;
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
        [AllowAnonymous]
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
        [Authorize(Roles = "TworzenieZlecen")]
        [Route("api/CreateRequest/{typeId}")]
        [ResponseType(typeof(DispatchRequest))]
        public async Task<IHttpActionResult> CreateNewRequest(int typeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = await db.Types.FirstOrDefaultAsync(t => t.Id == typeId);
            if (type == null)
            {
                return BadRequest($"Nieznany typ zlecenia {typeId}");
            }

            if (type.ForSelf)
            {
                return BadRequest($"Zlecenie typu '{type.Name}' jest zleceniem specjalnym.");
            }

            var existingRequest = await db.Requests.FirstOrDefaultAsync(r => r.Active && r.RequestingUserName == User.Identity.Name && r.TypeId == typeId);
            if (existingRequest != null)
            {
                return BadRequest($"Zlecenie typu '{type.Name}' dla użytkownika '{User.Identity.Name}' już istnieje.");
            }

            var newRequest = new DispatchRequest { RequestingUserName = User.Identity.Name, Active = true, TypeId = typeId, CreationDate = DateTime.UtcNow, CompletionDate = null, Type = type};
            db.Requests.Add(newRequest);
            await db.SaveChangesAsync();
           
            return CreatedAtRoute("DefaultApi", new { controller = "DispatchRequest",id = newRequest.Id }, newRequest);
        }

        [HttpGet]
        [Authorize(Roles="ObslugaZlecen")]
        [Route("api/CreateSpecialRequest/{typeId}")]
        [ResponseType(typeof(DispatchRequest))]
        public async Task<IHttpActionResult> CreateNewSpecialRequest(int typeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = await db.Types.FirstOrDefaultAsync(t => t.Id == typeId);
            if (type == null)
            {
                return BadRequest($"Nieznany typ zlecenia {typeId}");
            }

            if (!type.ForSelf)
            {
                return BadRequest($"Zlecenie typu '{type.Name}' nie jest zleceniem specjalnym.");
            }

            var existingRequest = await db.Requests.FirstOrDefaultAsync(r => r.Active && r.RequestingUserName == User.Identity.Name && r.TypeId == typeId);
            if (existingRequest != null)
            {
                return BadRequest($"Zlecenie typu '{type.Name}' dla użytkownika '{User.Identity.Name}' już istnieje.");
            }
            
            var newRequest = new DispatchRequest { ProvidingUserName = User.Identity.Name, PickedUpDate = DateTime.UtcNow, RequestingUserName = User.Identity.Name, Active = true, TypeId = typeId, CreationDate = DateTime.UtcNow, CompletionDate = null, Type = type };
            db.Requests.Add(newRequest);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { controller = "DispatchRequest", id = newRequest.Id }, newRequest);
        }

        [HttpDelete]
        [Route("api/DeleteRequest/{id}")]
        [ResponseType(typeof(DispatchRequest))]
        [Authorize(Roles = "TworzenieZlecen")]
        public async Task<IHttpActionResult> DeleteRequest(int id)
        {
            var request = await db.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(request.ProvidingUserName))
            {
                return BadRequest($"Nie można usunąć zlecenia o Id {id}, bo jest już w trakcie obsługi.");
            }

            db.Requests.Remove(request);
            await db.SaveChangesAsync();

            return Ok(request);
        }

        [HttpPut]
        [HttpPost]
        [Route("api/AcceptRequest/{requestId}")]
        [Authorize(Roles = "ObslugaZlecen")]
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
        [Route("api/CancelRequest/{requestId}")]
        [Authorize(Roles = "ObslugaZlecen")]
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

            if (request.Type.ForSelf)
            {
                db.Requests.Remove(request);
            }
            else
            {
                request.PickedUpDate = null;
                request.ProvidingUserName = null;
            }

            await db.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPut]
        [Route("api/CompleteRequest/{requestId}")]
        [Authorize(Roles = "ObslugaZlecen")]
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
