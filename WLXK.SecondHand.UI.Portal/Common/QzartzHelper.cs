using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WLXK.SecondHand.Bll;
using WLXK.SecondHand.Common;
using WLXK.SecondHand.IBll;

namespace WLXK.SecondHand.UI.Portal.Common
{
    public static class QuartzHelper
    {
        public static void CreateJob()
        {
            //每隔一段时间执行任务
            IScheduler sched;
            ISchedulerFactory sf = new StdSchedulerFactory();
            sched = sf.GetScheduler();
            JobDetail job = new JobDetail("job1", "group1", typeof(IndexJob));//IndexJob为实现了IJob接口的类
            DateTime ts = TriggerUtils.GetNextGivenSecondDate(null, 59);//5秒后开始第一次运行
            TimeSpan interval = TimeSpan.FromHours(3);//每隔30天执行一次
            Trigger trigger = new SimpleTrigger("trigger1", "group1", "job1", "group1", ts, null,
                                                    SimpleTrigger.RepeatIndefinitely, interval);//每若干小时运行一次，小时间隔由appsettings中的IndexIntervalHour参数指定

            sched.AddJob(job, true);
            sched.ScheduleJob(trigger);
            sched.Start();
        }
    }
    public class IndexJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            IListShopsServices ListShopsServices = new ListShopsServices();
            IAdminUserServices AdminUserServices = new AdminUserServices();

            var listShenHe = ListShopsServices.LoadEntities(u => u.Status == -1).ToList();
            var listAdmin = AdminUserServices.LoadEntities(u => true).ToList();

            foreach (var admin in listAdmin)
            {

                foreach (var item in listShenHe)
                {
                    MyEmail email = new MyEmail();
                    email.Title = "有用户的开店申请操作，没有被您处理，请您及时处理";
                    email.Subject = "青职二货街-管理员";
                    //生成验证连接
                    string website = CommonHelper.GetWebSite();

                    //生成发送邮箱模版
                    string html = System.IO.File.ReadAllText(HttpRuntime.AppDomainAppPath.ToString() + "/EmailTemp/OpenShops.html");
                    html = html.Replace("@website", website).Replace("@ids", item.Id.ToString()).Replace("@RealName", item.RealName).Replace("@IdCard", item.IdCard).Replace("@ClassName", item.CalssName).Replace("@imgsrc", website + item.CardAddress);
                    email.Content = html;

                    email.SendEmail = admin.Email;
                    EmailSendToAllAdmin.SendEmail(email);
                }
            }


        }
    }
}
