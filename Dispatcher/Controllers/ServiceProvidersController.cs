using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Models;

namespace Dispatcher.Controllers
{
    public class ServiceProvidersController : ApiController
    {
        private readonly DispatcherContext db = new DispatcherContext();

        // GET: api/ServiceProviders
        public IQueryable<ServiceProvider> GetServiceProviders()
        {
            return db.Providers;
        }

        // GET: api/ServiceProviders/5
        [ResponseType(typeof(ServiceProvider))]
        public async Task<IHttpActionResult> GetServiceProvider(int id)
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