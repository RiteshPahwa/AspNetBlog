using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ASPNetBlog.Models;
using ASPNetBlog.App_Common.Session;
using Microsoft.Net.Http.Server;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
// For Latest Blog Info visit http://AspNetBlog.com , For repository visit http://GitHub.com/RiteshPahwa/AspNetBlog

namespace ASPNetBlog.App_Common.Base
{
    public class BaseController : Controller
    {
        [FromServices]
        public ApplicationDbContext Db { get; set; }

        public BaseController() { }

        // Need suggestions on improvising the session settings
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(User.Identity.IsAuthenticated && AppSession.AppUserId == null)
                SetAppSession();
            
            //Set any values if you have for all the action requests
            ActionStartup();
        }

        protected void SetAppSession()
        {
            if (User?.Identity?.Name == null) return; //C#6.0

            //If Session is not set the return
            if (AppSession.Session == null) return;

            //If it is already set by other request thread then return
            if (AppSession.AppUserId != null) return;

            // Add Current AppUserID and other Required User Info to Session
            var appUserInfo = Db.Users.Where(u => u.UserName == User.Identity.Name).Select(u => new { u.AppUserId, u.FullName, u.Email, u.PhoneNumber }).SingleOrDefault();
            if (appUserInfo == null) throw new Exception($"Could not locate the user {User.Identity.Name} in store");

            AppSession.AppUserId = appUserInfo.AppUserId;
            AppSession.FullName = appUserInfo.FullName;
            AppSession.Email = appUserInfo.Email;
            AppSession.PhoneNumber = appUserInfo.PhoneNumber;

            return;
        }

        protected virtual void ActionStartup() {}

    }
}
