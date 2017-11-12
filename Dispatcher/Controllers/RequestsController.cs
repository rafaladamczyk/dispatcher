using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Dispatcher.Data;
using Dispatcher.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace Dispatcher.Controllers
{
    public class RequestsController : AbstractBaseController
    {
        public RequestsController()
        {
            Uow = new DispatcherUow();    
        }

        [HttpGet]
        [Route("api/Requests")]
        [AllowAnonymous]
        public IEnumerable<Request> GetAllDispatchRequests()
        {
            return Uow.Requests.GetAll();
        }

        [HttpGet]
        [Route("api/ActiveRequests")]
        [AllowAnonymous]
        public IEnumerable<Request> GetActiveRequests()
        {
           return Uow.Requests.GetAll().Where(r => r.Active).ToList();
        }

        [HttpGet]
        [Route("api/Requests/{id}")]
        [AllowAnonymous]
        public Request GetRequest(int id)
        {
            Request request = Uow.Requests.GetById(id);
            if (request == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return request;
        }

        [HttpPost]
        [System.Web.Http.Authorize(Roles = "TworzenieZlecen, ObslugaZlecen")]
        [Route("api/Requests")]
        public HttpResponseMessage NewRequest(Request request)
        {
            try
            {
                request.Active = true;
                request.CreationDate = DateTime.UtcNow;
                request.CreatorId = User.Identity.GetUserId();

                Uow.Requests.Add(request);
                Uow.Commit();

                Uow.Requests.GetEntry(request).Reference(r => r.Type).Load();
                if (request.Type.ForSelf)
                {
                    request.ProviderId = request.CreatorId;
                    request.PickedUpDate = request.CreationDate;
                }

                BroadcastRequest(request);

                var response = Request.CreateResponse(HttpStatusCode.Created, request);

                //response.Headers.Location =
                //    new Uri(Url.Link(/*WebApiConfig.ControllerAndId*/"somepath", new { id = request.Id }));

                return response;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = ex.Message });
            }
        }

        [HttpDelete]
        [Route("api/Requests/{id}")]
        [System.Web.Http.Authorize(Roles = "TworzenieZlecen, ObslugaZlecen")]
        public HttpResponseMessage DeleteRequest(int id)
        {
            Uow.Requests.Delete(id);
            Uow.Commit();

            BroadcastRequestRemoved(id);
            
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [Route("api/Requests")]
        [System.Web.Http.Authorize(Roles = "ObslugaZlecen")]
        public HttpResponseMessage UpdateRequest(Request request)
        {
            Uow.Requests.Update(request);
            Uow.Commit();

            BroadcastRequest(request);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [Route("api/Requests/Accept")]
        [System.Web.Http.Authorize(Roles = "ObslugaZlecen")]
        public HttpResponseMessage AcceptRequest(Request request)
        {
            request.ProviderId = User.Identity.GetUserId();
            request.PickedUpDate = DateTime.UtcNow;
            
            Uow.Requests.Update(request);
            Uow.Commit();

            BroadcastRequest(request);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [Route("api/Requests/Cancel")]
        [System.Web.Http.Authorize(Roles = "ObslugaZlecen")]
        public HttpResponseMessage CancelRequest(Request request)
        {
            request.ProviderId = null;
            request.PickedUpDate = null;

            Uow.Requests.Update(request);
            Uow.Commit();

            BroadcastRequest(request);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }


        [HttpPut]
        [Route("api/Requests/Complete")]
        [System.Web.Http.Authorize(Roles = "ObslugaZlecen")]
        public HttpResponseMessage CompleteRequest(Request request)
        {
            request.Active = false;
            request.CompletionDate = DateTime.UtcNow;
            request.Duration = request.CompletionDate - request.CreationDate;
            request.ServiceDuration = request.CompletionDate - request.PickedUpDate;

            Uow.Requests.Update(request);
            Uow.Commit();

            BroadcastRequest(request);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        
        private void BroadcastRequest(Request request)
        {
            GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.updateRequest(request);
        }

        private void BroadcastRequestRemoved(int id)
        {
            GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.removeRequest(id);
        }
    }
}
