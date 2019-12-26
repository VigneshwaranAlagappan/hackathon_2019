namespace Bounty.Base
{
    using System;
    using System.Reflection;
    using System.Security.Principal;
    using System.Web;
    using System.Net;
    using Bounty.DataClasses;

    public class ServerPrincipal : IPrincipal
    {
        public ServerPrincipal()
        {
        }

        public ServerPrincipal(IIdentity identity)
        {
            var userId = Guid.Empty;
            Users userDetail = null;
            try
            {
                userDetail = new BugManagement().GetUser(HttpContext.Current.User.Identity.Name);

                if (userDetail != null && userDetail.IsActive)
                {
                    this.UserId = userDetail.Id;
                    this.UserEmail = userDetail.Email;
                    this.Identity = identity;
                }

            }
            catch (Exception ex)
            {
                //LogExtension.LogError(string.Empty, 
                //    "Logged Username: " + HttpContext.Current.User.Identity.Name,
                //    ex,
                //    MethodBase.GetCurrentMethod());
            }
        }

        public IIdentity Identity
        {
            get;
            private set;
        }

        public string UserEmail
        {
            get;
            private set;
        }

        public Guid UserId
        {
            get;
            private set;
        }

        public string UserName
        {
            get;
            private set;
        }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}