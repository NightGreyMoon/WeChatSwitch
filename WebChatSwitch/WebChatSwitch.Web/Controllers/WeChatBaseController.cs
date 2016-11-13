using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebChatSwitch.Web.Controllers
{
    public class WeChatBaseController : Controller
    {
        public LoginUser CurrentUser
        {
            get
            {
                if (Session["LoginUser"] != null)
                {
                    return Session["LoginUser"] as LoginUser;
                }
                return null;
            }

            set
            {
                Session["LoginUser"] = value;
            }
        }

        public class LoginUser
        {
            public string OpenId { get; set; }
            public int Id { get; set; }
        }

        #region override
        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    base.OnException(filterContext);

        //    var ex = filterContext.Exception;
        //    while (ex != null)
        //    {
        //        log.Error("程序异常", filterContext.Exception);
        //        ex = ex.InnerException;
        //    }

        //    filterContext.ExceptionHandled = true;

        //    if (CurrentUser.Language == "en")
        //    {
        //        TempData["Info"] = "System Error";
        //    }
        //    else
        //    {
        //        TempData["Info"] = "系统异常";
        //    }
        //    filterContext.Result = new RedirectResult("~/Error/Index");

        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (db != null)
        //        {
        //            db.Dispose();
        //        }
        //    }
        //    base.Dispose(disposing);
        //}

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);

        //    var lan = Request.QueryString["lan"];

        //    if (CurrentUser == null)
        //    {
        //        CurrentUser = new LoginUser { Language = lan };
        //    }

        //    #region set culture
        //    string cultureName = null;
        //    if (!string.IsNullOrWhiteSpace(lan))
        //    {
        //        cultureName = lan;
        //    }
        //    else if (CurrentUser != null && !string.IsNullOrWhiteSpace(CurrentUser.Language))
        //        cultureName = CurrentUser.Language;
        //    else
        //    {
        //        cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
        //                Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
        //                null;

        //    }

        //    // Validate culture name
        //    cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

        //    // Modify current thread's cultures            
        //    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
        //    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        //    CurrentUser.Language = cultureName;
        //    #endregion
        //}

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    base.OnActionExecuted(filterContext);

        //    ViewBag.Language = CurrentUser.Language;
        //}

        #endregion
    }
}