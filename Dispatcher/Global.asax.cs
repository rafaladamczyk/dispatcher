using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Dispatcher.Models;
using Microsoft.AspNet.SignalR;

namespace Dispatcher
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                Task.Run(
                    async () =>
                    {
                        while (!cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            IDispatcherContext db = DispatcherContext.Create();

                            try
                            {
                                DateTime expiryDate = DateTime.UtcNow - TimeSpan.FromHours(1);
                                var overdueRequests = db.Requests.Where(
                                    r => r.Active && r.PickedUpDate.HasValue && !r.Type.ForSelf
                                         && DbFunctions.CreateDateTime(
                                             r.PickedUpDate.Value.Year,
                                             r.PickedUpDate.Value.Month,
                                             r.PickedUpDate.Value.Day,
                                             r.PickedUpDate.Value.Hour,
                                             r.PickedUpDate.Value.Minute,
                                             r.PickedUpDate.Value.Second) < DbFunctions.CreateDateTime(
                                             expiryDate.Year,
                                             expiryDate.Month,
                                             expiryDate.Day,
                                             expiryDate.Hour,
                                             expiryDate.Minute,
                                             expiryDate.Second)).ToArray();

                                if (overdueRequests.Any())
                                {
                                    foreach (var request in overdueRequests)
                                    {
                                        request.PickedUpDate = null;
                                        request.ProvidingUserName = null;
                                    }

                                    db.SaveChanges();
                                    var activeRequests = db.Requests.Where(r => r.Active).ToList();
                                    GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.updateActiveRequests(activeRequests);
                                }
                            }
                            catch
                            {
                            }

                            await Task.Delay(TimeSpan.FromMinutes(5), cancellationTokenSource.Token);
                        }
                    },
                    cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
            }

            catch (OperationCanceledException)
            {
            }
        }

        protected void Application_End()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
