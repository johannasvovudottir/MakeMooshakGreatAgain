using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    /// <summary>
    /// A controller that all other controllers inherit from
    /// contains the HandleError attribute and a function that 
    /// checks what language to  use
    /// </summary>
    [HandleError]
    public class BaseController : Controller
    {
        protected string Language { get; set; }
        /// <summary>
        /// A function that checks what languague the page is supposed
        /// to be displayed in
        /// </summary>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.RouteData.Values.ContainsKey("lang"))
                Language = filterContext.RouteData.Values["lang"].ToString().ToLower();
            else
                Language = "is";

            ViewBag.language = Language;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Language);
        }

    }
}