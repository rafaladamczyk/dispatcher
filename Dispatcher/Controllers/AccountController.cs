﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Dispatcher.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;

namespace Dispatcher.Controllers
{
    [System.Web.Http.Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager userManager;
        private readonly IdentityDbContext<ApplicationUser> idContext;
        private readonly IDispatcherContext db;

        public AccountController()
        {
            var context = new DispatcherContext();
            db = context;
            idContext =context;
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            var context = new DispatcherContext();
            db = context;
            idContext = context;

            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            return new UserInfoViewModel
            {
                Name = User.Identity.GetUserName(),
                Roles = GetRoles((ClaimsIdentity)User.Identity).ToList()
            };
        }
        
        [Route("UsersAndRoles")]
        [AllowAnonymous]
        [HttpGet]
        public List<UserInfoViewModel> GetUsersAndRoles()
        {
            var store = new RoleStore<IdentityRole>(new DispatcherContext());
            var roleManager = new RoleManager<IdentityRole>(store);

            var result = new List<UserInfoViewModel>();
            foreach (var user in UserManager.Users.OrderBy(u => u.UserName).ToList())
            {
                var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();
                var roles = roleManager.Roles.Where(r => userRoleIds.Contains(r.Id)).ToList();
                var roleNames = roles.Select(r => r.Name).ToList();

                result.Add(new UserInfoViewModel
                           {
                               Name = user.UserName,
                               Roles = roleNames
                           });
            }
            return result;
        }

        [System.Web.Http.Authorize(Roles = "Admin")]
        [Route("UserAndRoles")]
        [HttpPost]
        public void SaveUsersAndRoles(UserRoleModel userAndRoles)
        {
            var user = UserManager.FindByName(userAndRoles.Name);
            AddOrRemoveRole(user.Id, "Admin", userAndRoles.IsAdmin);
            AddOrRemoveRole(user.Id, "ObslugaZlecen", userAndRoles.IsServiceProvider);
            AddOrRemoveRole(user.Id, "TworzenieZlecen", userAndRoles.IsRequester);

            BroadcastUsersAndRoles();
        }

        [HttpDelete]
        [Route("User/{name}")]
        [System.Web.Http.Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteUser(string name)
        {
            ApplicationUser user = UserManager.FindByName(name);
            if (user == null)
            {
                NotFound();
            }
            
            UserManager.Delete(user);

            BroadcastUsersAndRoles();
            return Ok();
        }

        private void AddOrRemoveRole(string userId, string role, bool add)
        {
            if (add)
            {
                UserManager.AddToRole(userId, role);
            }
            else
            {
                UserManager.RemoveFromRole(userId, role);
            }
        }

        [Route("Roles")]
        public List<string> GetRoles()
        {
            return idContext.Roles.Select(r => r.Name).ToList();
        }

        [Route("Users")]
        public List<string> GetUsers()
        {
            return UserManager.Users.OrderBy(u => u.UserName).Select(u => u.UserName).ToList();
        } 


        public static IEnumerable<string> GetRoles(ClaimsIdentity identity)
        {
            return identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

     
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage ?? "Nieznany błąd");
            }

            var user = new ApplicationUser() { UserName = model.UserName};

            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddToRoleAsync(user.Id, "ObslugaZlecen");
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            BroadcastUsersAndRoles();

            return Ok();
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

        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors?.FirstOrDefault() ?? "");
            }

            return null;
        }

        private void BroadcastUsersAndRoles()
        {
            GlobalHost.ConnectionManager.GetHubContext<RequestsHub>().Clients.All.updateUsersAndRoles(GetUsersAndRoles());
        }

        #endregion
    }
}
