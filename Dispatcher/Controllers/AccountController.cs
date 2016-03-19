using System.Collections.Generic;
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

namespace Dispatcher.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
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

        [Authorize(Roles = "Admin")]
        [Route("UsersAndRoles")]
        [HttpGet]
        public List<UserInfoViewModel> GetUsersAndRoles()
        {
            var store = new RoleStore<IdentityRole>(new DispatcherContext());
            var roleManager = new RoleManager<IdentityRole>(store);

            var result = new List<UserInfoViewModel>();
            foreach (var user in UserManager.Users.ToList())
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

        [Authorize(Roles = "Admin")]
        [Route("UsersAndRoles")]
        [HttpPost]
        public void SaveUsersAndRoles(List<UserRoleModel> usersAndRoles)
        {
            foreach (var user in usersAndRoles)
            {
                var userId = UserManager.FindByName(user.Name).Id;
                AddOrRemoveRole(userId, "Admin", user.IsAdmin);
                AddOrRemoveRole(userId, "ObslugaZlecen", user.IsServiceProvider);
                AddOrRemoveRole(userId, "TworzenieZlecen", user.IsRequester);
            }
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
            return new DispatcherContext().Roles.Select(r => r.Name).ToList();
        }

        [Route("Users")]
        public List<string> GetUsers()
        {
            return UserManager.Users.Select(u => u.UserName).ToList();
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
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.UserName};

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);
           
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddToRoleAsync(user.Id, "ObslugaZlecen");
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

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
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        #endregion
    }
}
