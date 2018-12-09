using JiraCloneMVC.Web.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JiraCloneMVC.Web.Controllers
{
    public class AccountController : Controller
    {
        protected ApplicationUserManager UserManager { get { return HttpContext.GetOwinContext().Get<ApplicationUserManager>(); } }
        protected ApplicationRoleManager RoleManager { get { return HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); } }
        protected ApplicationSignInManager SignInManager { get { return HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); } }
        

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}