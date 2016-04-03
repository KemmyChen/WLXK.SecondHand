using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.UI.Portal.Controllers
{
    public class AdminBaseController : Controller
    {
        public AdminUser CurrentLoginUser { get; set; }

        //当前控制器下面的所有的action在执行之前都来执行此方法
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);
            //校验用户是否已经登录
            if (filterContext.HttpContext.Session["adminlogin"] == null)
            {
                string oldurl = filterContext.HttpContext.Request.Path;

                filterContext.HttpContext.Response.Redirect("/Others/AdminLogin?oldurl="+oldurl);
            }
            else
            {
                CurrentLoginUser = (AdminUser)filterContext.HttpContext.Session["adminlogin"];
            }
        }

    }
}
