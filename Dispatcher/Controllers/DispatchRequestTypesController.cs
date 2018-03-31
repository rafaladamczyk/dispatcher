using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Models;
using Microsoft.AspNet.SignalR;

namespace Dispatcher.Controllers
{
    public class DispatchRequestTypesController : ApiController
    {
        private readonly DispatcherContext db = new DispatcherContext();

        // GET: api/DispatchRequestTypes
        public IQueryable<DispatchRequestType> GetTypes()
        {
            return db.Types.Where(t => !t.ForSelf);
        }

        [HttpGet]
        [Route("api/selfRequestTypes")]
        public IQueryable<DispatchRequestType> GetSelfRequestTypes()
        {
            return db.Types.Where(t => t.ForSelf);
        }


        // GET: api/DispatchRequestTypes/5
        [ResponseType(typeof(DispatchRequestType))]
        public async Task<IHttpActionResult> GetDispatchRequestType(int id)
        {
            DispatchRequestType dispatchRequestType = await db.Types.FindAsync(id);
            if (dispatchRequestType == null)
            {
                return NotFound();
            }

            return Ok(dispatchRequestType);
        }

        // PUT: api/DispatchRequestTypes/5
        [System.Web.Http.Authorize(Roles = "Admin")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDispatchRequestType(int id, DispatchRequestType dispatchRequestType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dispatchRequestType.Id)
            {
                return BadRequest();
            }

            db.Entry(dispatchRequestType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
                BroadcastRequestTypes();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DispatchRequestTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DispatchRequestTypes
        [System.Web.Http.Authorize(Roles = "Admin")]
        [ResponseType(typeof(DispatchRequestType))]
        public async Task<IHttpActionResult> PostDispatchRequestType(DispatchRequestType dispatchRequestType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(dispatchRequestType?.Name))
            {
                return BadRequest("Nazwa nie może być pusta.");
            }

            var existingType = await db.Types.FirstOrDefaultAsync(t => t.Name == dispatchRequestType.Name);
            if (existingType != null)
            {
                return BadRequest($"Typ o nazwie {dispatchRequestType.Name} już istnieje.");
            }

            db.Types.Add(dispatchRequestType);
            await db.SaveChangesAsync();

            BroadcastRequestTypes();

            return CreatedAtRoute("DefaultApi", new { id = dispatchRequestType.Id }, dispatchRequestType);
        }

        // DELETE: api/DispatchRequestTypes/5
        [ResponseType(typeof(DispatchRequestType))]
        [System.Web.Http.Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> DeleteDispatchRequestType(int id)
        {
            DispatchRequestType dispatchRequestType = await db.Types.FindAsync(id);
            if (dispatchRequestType == null)
            {
                return NotFound();
            }

            db.Types.Remove(dispatchRequestType);
            await db.SaveChangesAsync();

            BroadcastRequestTypes();
            BroadcastActiveRequests();

            return Ok(dispatchRequestType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DispatchRequestTypeExists(int id)
        {
            return db.Types.Count(e => e.Id == id) > 0;
        }

        private void BroadcastRequestTypes()
        {
            GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.updateRequestTypes(db.Types.ToList());
        }

        private void BroadcastActiveRequests()
        {
            var activeRequests = db.Requests.Where(r => r.Active).ToList();
            GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.updateActiveRequests(activeRequests);
        }
    }
}