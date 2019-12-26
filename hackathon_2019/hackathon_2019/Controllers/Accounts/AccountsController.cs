namespace hackathon_2019.Controllers.Accounts
{
    using Bounty.Base;
    using hackathon_2019.Models;
    using Microsoft.Owin.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AccountsController : Controller
    {
        private readonly AccountModel _accountModel;

        public AccountsController()
        {
            _accountModel = new AccountModel();
        }

        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                GlobalAppSettings.SetTimeZone();

                if (Request["returnUrl"] != null)
                {
                    return new RedirectResult(Request["returnUrl"]);
                }

                return RedirectToAction("Index", "Home");
            }

            var passwordMessage = Request.QueryString["Message"];
            TempData["password"] = "none";
            TempData["username"] = "none";
            if (passwordMessage == "success")
            {
                TempData["User"] = "Password has been changed successfully.";
            }

            ViewBag.ReturnURL = Request["returnUrl"] ?? (HttpContext.Response.Cookies["mobile_cookie"] != null ? HttpContext.Response.Cookies["mobile_cookie"].Value : string.Empty);
            ViewBag.PostAction = Url.Action("ValidateSyncfusionUser", "Accounts");
            ViewBag.ExternalLoginController = "Accounts";
            ViewBag.LoginLogo = CloudAppConfig.SyncfusionLoginLogoUrl;

            Response.Cookies.Add(new HttpCookie("user_session")
            {
                Value = null,
                Expires = DateTime.UtcNow.AddDays(-1),
                Domain = "localhost"
            });

            return View();
        }

        public ActionResult Logout()
        {
            if (Request.IsAuthenticated)
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
                Request.GetOwinContext().Authentication.SignOut("Cookies");
                Response.Cookies.Add(new HttpCookie("user_session")
                {
                    Value = null,
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Domain = "localhost"
                });
            }

            if (Request.QueryString["returnOAuth"] == "1")
            {
                if (HttpContext.Session["AccessDeniedForTenantReturnOAuth"] != null && bool.Parse(HttpContext.Session["AccessDeniedForTenantReturnOAuth"].ToString()))
                {
                    HttpContext.Session["AccessDeniedForTenantReturnOAuth"] = false;
                    return new RedirectResult(HttpContext.Session["returnUrl"].ToString());
                }

                var returnUrl = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["returnurl"];
                returnUrl = returnUrl.Replace("/authorize&", "/authorize?");
                returnUrl = new Uri(Request.Url.ToString()).GetLeftPart(UriPartial.Authority) + returnUrl;
                var state = Request.QueryString["state"];
                returnUrl = returnUrl.Replace(state.ToLower(), state);
                return new RedirectResult(returnUrl);
            }

            return RedirectToAction("login", "accounts");
        }

        [HttpPost]
        [AllowAnonymous]
        public bool ValidateCookie()
        {
            try
            {
                var authority = Request.UrlReferrer != null ? Request.UrlReferrer.Authority.ToLowerInvariant() : string.Empty;
                if (authority.EndsWith("localhost"))
                {
                    return HttpContext.User.Identity.IsAuthenticated;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //LogExtension.LogError(string.Empty, "Failure to validate cookie consent", ex, MethodBase.GetCurrentMethod());
                return false;
            }
        }

        [HttpPost]
        public JsonResult ValidateEmail(string email, string callBackUri)
        {
            try
            {
                var user = new BugManagement().GetUser(email);
                return Json(new { Status = user.IsActive });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false });
            }
        }

        public ActionResult SyncfusionLogin()
        {
            var clientId = Request["returnUrl"] == null || !Request.Url.PathAndQuery.Contains("redirect_uri") ? ViewBag.ClientId : HttpUtility.ParseQueryString(new Uri(HttpUtility.UrlDecode(Request["returnUrl"])).Query).Get("client_id");
            ViewBag.ClientId = clientId;
            TempData["validAccount"] = false;
            TempData["isActivated"] = false;
            TempData["privacyStatus"] = false;
            TempData["userName"] = string.Empty;
            TempData["errorType"] = string.Empty;
            TempData["returnUrl"] = Request["returnUrl"]?.ToString() ?? string.Empty;
            TempData["accessDeniedForTenant"] = false;

            return View("../shared/syncfusionlogin");
        }

        [HttpPost]
        public ActionResult ValidateSyncfusionUser(string email, string password, string remember, string returnUrl, string clientId)
        {
            var ipAddress = Request.UserHostAddress;
            var host = Request.Url.GetLeftPart(UriPartial.Authority);
            returnUrl = returnUrl == null ? string.Empty : Request["returnUrl"]?.ToString() ?? returnUrl;
            clientId = string.IsNullOrWhiteSpace(clientId) ? null : clientId;

            var response = _accountModel.ValidateSyncfusionUser(email, password, remember, ipAddress, host, clientId);
            TempData["isActivated"] = response.Status;
            TempData["userName"] = email;
            TempData["returnUrl"] = returnUrl;
            TempData["privacyStatus"] = response.HavePrivacyConsent;
            TempData["validAccount"] = false;
            TempData["errorType"] = string.Empty;
            TempData["accessDeniedForTenant"] = false;

            ViewBag.ClientId = clientId;

            switch (response.Message.ToLower())
            {
                case "valid account":
                    HttpContext.Session["syncfusionLoginRequest"] = true;
                    TempData["validAccount"] = true;
                    if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == Url.Content("~/"))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    break;
                case "not activated":
                    TempData["errorUserName"] = email;
                    TempData["errorPassword"] = response.Message;
                    TempData["errorUserStatus"] = "inline-block";
                    break;
                case "incorrect password":
                    TempData["currentValue"] = email;
                    TempData["errorUserName"] = string.Empty;
                    TempData["errorPassword"] = response.Message;
                    TempData["errorUserStatus"] = string.Empty;
                    TempData["errorPasswordStatus"] = "inline-block";
                    ViewBag.ReturnURL = returnUrl;
                    break;
                case "empty records":
                case "connection issue":
                    TempData["errorUserName"] = email;
                    TempData["errorPassword"] = response.Message;
                    TempData["errorUserStatus"] = "inline-block";
                    break;
            }

            ViewBag.ReturnURL = returnUrl;
            return View("../accounts/login");
        }
    }
}