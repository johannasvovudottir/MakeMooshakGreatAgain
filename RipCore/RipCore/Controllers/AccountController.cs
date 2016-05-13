using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RipCore.Models;
using RipCore.Models.ViewModels;
using RipCore.Services;

namespace RipCore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private AccountsService service;
        private PersonService personService;

        public AccountController()
        {
            service = new AccountsService();
            personService = new PersonService();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            service = new AccountsService();
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //If this is triggered, it means a logged in user is trying to access login page through non-standard ways - let's redirect.
            if (HttpContext.Request.IsAuthenticated)
            {
                return RedirectToAction("", "User");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string userID = null;
            if (service.GetIdByUser(model.Username, ref userID))
            {
                var user = personService.GetPersonById(userID);
                if(user.CentrisUser)
                {
                    ModelState.AddModelError("", "Ïnvalid login attempt");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Ïnvalid login attempt");
                return View(model);
            }
           
            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        if(service.GetHighestUserPrivilege(userID, null) == SecurityState.ADMIN)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        return RedirectToAction("Index", "User");
                    }
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult CentrisLogin(string returnUrl)
        {
            //If this is triggered, it means a logged in user is trying to access login page through non-standard ways - let's redirect.
            if (HttpContext.Request.IsAuthenticated)
            {
                return RedirectToAction("", "User");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CentrisLogin(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string json = null;
            var isOk = service.RequestJson(model, ref json);
            if (!isOk)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            var centrisModel = service.GetCentrisUser(json);
            string userID = null;
            var id = service.GetIdByUser(centrisModel.UserName, ref userID);
            try
            {
                PersonViewModel personModel = personService.GetPersonById(userID);
                if (personModel.ID != null)
                {
                    var result = await SignInManager.PasswordSignInAsync(model.Username, "Centris1#", model.RememberMe, false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            return RedirectToAction("Index", "User");
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
            }
            catch
            {
                string email = null;
                Random rnd = new Random();
                int[] num = Enumerable.Range(0, 10).ToArray();
                for (int i = 0; i < 8; i++)
                {
                     email += num[rnd.Next(0, 9)];
                }
                email += "@centris.is";
                var user = new ApplicationUser { UserName = centrisModel.UserName, Email = email, FullName = centrisModel.FullName, Ssn = centrisModel.Ssn, CentrisUser=true };
                var result = await UserManager.CreateAsync(user, "Centris1#");
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "User");
                }
            }     
            return View(model);
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
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

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}