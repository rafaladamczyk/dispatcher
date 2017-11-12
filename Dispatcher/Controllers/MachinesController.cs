using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Dispatcher.Models;

namespace Dispatcher.Controllers
{
    public class MachinesController : AbstractBaseController
    {
        [HttpGet]
        [Route("api/Machines/{id}")]
        public Machine GetMachine(int id)
        {
            var machine = Uow.Machines.GetById(id);
            if (machine == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return machine;
        }

        [HttpGet]
        [Route("api/Machines")]
        public IEnumerable<Machine> GetMachines()
        {
            return Uow.Machines.GetAll();
        }
    }
}
