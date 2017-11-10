using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Dispatcher.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Dispatcher.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dispatcher.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : AbstractBaseController
    {
        private ApplicationUserManager userManager;
        private readonly IdentityDbContext<ApplicationUser> dbContext;

        public AccountController()
        {
            dbContext = new DispatcherContext();
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            dbContext = new DispatcherContext();

            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get { return userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { userManager = value; }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; }

        [Route("User")]
        public UserInfoViewModel GetUser()
        {
            return new UserInfoViewModel
            {
                Id = User.Identity.GetUserId(),
                Name = User.Identity.GetUserName(),
                Roles = new HashSet<string>(GetRoles((ClaimsIdentity)User.Identity))
            };
        }
        
        [Route("Users")]
        [AllowAnonymous]
        [HttpGet]
        public List<UserInfoViewModel> GetUsers()
        {
            var store = new RoleStore<IdentityRole>(dbContext);
            var roleManager = new RoleManager<IdentityRole>(store);

            var result = new List<UserInfoViewModel>();
            foreach (var user in UserManager.Users.OrderBy(u => u.UserName).ToList())
            {
                var userRoleIds = new HashSet<string>(user.Roles.Select(r => r.RoleId));
                var roles = roleManager.Roles.Where(r => userRoleIds.Contains(r.Id));
                var roleNames = new HashSet<string>(roles.Select(r => r.Name));

                result.Add(new UserInfoViewModel { Id = user.Id, Name = user.UserName, Roles = roleNames });
            }
            return result;
        }

        [Authorize(Roles = "Admin")]
        [Route("Users")]
        [HttpPut]
        public HttpResponseMessage UpdateUser(UserInfoViewModel model)
        {
            var user = UserManager.FindById(model.Id);
            
            var rolesToAdd = dbContext.Roles.Select(r => r.Name).AsEnumerable().Where(model.Roles.Contains).ToArray();
            var rolesToRemove = dbContext.Roles.Select(r => r.Name).Except(rolesToAdd).ToArray();

            UserManager.AddToRoles(user.Id, rolesToAdd);
            UserManager.RemoveFromRoles(user.Id, rolesToRemove);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("Users/{id}")]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage DeleteUser(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            
            UserManager.Delete(user);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public static IEnumerable<string> GetRoles(ClaimsIdentity identity)
        {
            return identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public HttpResponseMessage ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

     
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public HttpResponseMessage Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var user = new ApplicationUser() { UserName = model.UserName};

            var result = UserManager.Create(user, model.Password);
            if (!result.Succeeded)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, string.Join("\n", result.Errors ?? Enumerable.Empty<string>()));
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
                userManager = null;
            }

            base.Dispose(disposing);
        }
    }
}
