using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WLXK.SecondHand.Common;
using WLXK.SecondHand.UI.Portal.Common;
using WLXK.SecondHand.UI.Portal.Controllers;

namespace WLXK.SecondHand.UI.Portal
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : Spring.Web.Mvc.SpringMvcApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();

            LunceneHelper.lucenePath = HttpRuntime.AppDomainAppPath.ToString() + "lucenedir";

            #region 创建Luncene目录
            if (!Directory.Exists(LunceneHelper.lucenePath))
            {
                Directory.CreateDirectory(LunceneHelper.lucenePath);
            } 
            #endregion

            //启动Lucene线程防止文件并发问题
            IndexManager.myManager.Start();

            QuartzHelper.CreateJob();
        }

        public void Application_Error(object sender, EventArgs e)
        {
            //记录日志
            Exception ex = Server.GetLastError();

            string errorMsg = ex.ToString();

            LogHelper.WriteLog(errorMsg);

            Response.Redirect("/Others/Error");
        }
    }
}