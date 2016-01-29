using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Dispatcher.Controllers
{
    public class DispatchRequestersController : ApiController
    {
        private readonly IDispatcherContext db;

        public DispatchRequestersController()
        {
            db = new DispatcherContext();
        }

        public DispatchRequestersController(IDispatcherContext context)
        {
            db = context;
        }

        // GET: api/DispatchRequesters
        public IQueryable<DispatchRequester> GetRequesters()
        {
            return db.Requesters;
        }

        // GET: api/DispatchRequesters/5
        [ResponseType(typeof(DispatchRequester))]
        public async Task<IHttpActionResult> GetDispatchRequester(int id)
        {
            DispatchRequester dispatchRequester = await db.Requesters.FindAsync(id);
            if (dispatchRequester == null)
            {
                return NotFound();
            }

            return Ok(dispatchRequester);
        }

        // PUT: api/DispatchRequesters/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDispatchRequester(int id, DispatchRequester dispatchRequester)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dispatchRequester.Id)
            {
                return BadRequest();
            }

            db.MarkAsModified(dispatchRequester);

            try
            {
               await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DispatchRequesterExists(id))
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

        // POST: api/DispatchRequesters
        [ResponseType(typeof(DispatchRequester))]
        public async Task<IHttpActionResult> PostDispatchRequester(DispatchRequester dispatchRequester)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Requesters.Add(dispatchRequester);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dispatchRequester.Id }, dispatchRequester);
        }

        // DELETE: api/DispatchRequesters/5
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(DispatchRequester))]
        public async Task<IHttpActionResult> DeleteDispatchRequester(int id)
        {
            ApplicationUser user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            DispatchRequester dispatchRequester = await db.Requesters.FindAsync(id);

            if (dispatchRequester == null)
            {
                return NotFound();
            }

            db.Requesters.Remove(dispatchRequester);
           await  db.SaveChangesAsync();

            return Ok(dispatchRequester);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DispatchRequesterExists(int id)
        {
            return db.Requesters.Count(e => e.Id == id) > 0;
        }
    }
}