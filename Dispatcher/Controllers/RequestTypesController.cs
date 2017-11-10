using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dispatcher.Data;
using Dispatcher.Models;
using Microsoft.AspNet.SignalR;

namespace Dispatcher.Controllers
{
    public class RequestTypesController : AbstractBaseController
    {
        public RequestTypesController()
        {
            Uow = new DispatcherUow();
        }

        [HttpGet]
        [Route("api/RequestTypes")]
        public IEnumerable<RequestType> GetTypes()
        {
            return Uow.RequestTypes.GetAll();
        }

        [HttpGet]
        [Route("api/RequestTypes/{id}")]
        public RequestType GetDispatchRequestType(int id)
        {
            var request = Uow.RequestTypes.GetById(id);
            if (request == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return request;
        }

        [HttpPut]
        [System.Web.Http.Authorize(Roles = "Admin")]
        [Route("api/RequestTypes")]
        public HttpResponseMessage UpdateRequestType(RequestType requestType)
        {
            Uow.RequestTypes.Update(requestType);
            Uow.Commit();

            BroadcastRequestType(requestType);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [System.Web.Http.Authorize(Roles = "Admin")]
        [Route("api/RequestTypes")]
        public HttpResponseMessage CreateRequestType(RequestType requestType)
        {
            Uow.RequestTypes.Add(requestType);
            Uow.Commit();

            BroadcastRequestType(requestType);

            var response = Request.CreateResponse(HttpStatusCode.Created, requestType);

            //response.Headers.Location =
            //    new Uri(Url.Link(/*WebApiConfig.ControllerAndId*/"somepath", new { id = requestType.Id }));

            return response;
        }

        [HttpDelete]
        [System.Web.Http.Authorize(Roles = "Admin")]
        public HttpResponseMessage DeleteRequestType(int id)
        {
            Uow.RequestTypes.Delete(id);
            Uow.Commit();

            BroadcastRemoveRequestType(id);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        private void BroadcastRequestType(RequestType type)
        {
            GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.updateRequestType(type);
        }

        private void BroadcastRemoveRequestType(int id)
        {
            GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.removeRequestType(id);
        }
    }
}