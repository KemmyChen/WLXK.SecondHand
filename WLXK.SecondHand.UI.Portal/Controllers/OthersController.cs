using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WLXK.SecondHand.Common;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.UI.Portal.Controllers
{
    public class OthersController : Controller
    {
        public ISuggestServices SuggestServices { get; set; }
        public IUserInfoServices UserInfoServices { get; set; }
        public IAdminUserServices AdminUserServices { get; set; }

        #region 错误处理页面
        public ActionResult ShowMsg(string OldUrl, string NewUrl, string PointMsg, string NewTitle)
        {
            if (OldUrl == null || NewUrl == null || PointMsg == null || NewTitle == null)
            {
                return RedirectToAction("error");
            }
            ViewData["OldUrl"] = OldUrl;
            ViewData["NewUrl"] = NewUrl;
            ViewData["PointMsg"] = PointMsg;
            ViewData["NewTitle"] = NewTitle;
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
        #endregion

        #region 反馈意见
        public ActionResult Suggest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Suggest(Suggest suggest)
        {
            suggest.SubTime = DateTime.Now;

            try
            {
                SuggestServices.Add(suggest);
                return Content("ok");
            }
            catch (Exception)
            {
                return Content("数据库错误！请联系管理员");
            }
        }
        public ActionResult SuggestSuccess()
        {
            return View();
        }
        #endregion

        #region 开店相关
        public ActionResult GetCodeByEmail(int? id)
        {
            if (id == null)
            {
                return Content("请进行登录之后在进行操作");
            }
            UserInfo user = UserInfoServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            if (user == null)
            {
                return Content("请进行登陆之后在进行操作");
            }


            string code = CommonHelper.GetStringMD5(Guid.NewGuid().ToString());


            MyEmail email = new MyEmail();
            email.Content = "亲爱的青职二货街用户，您好：您的验证码为" + code + "本邮件是系统自动发出的，请勿回复。祝您使用愉快！";
            email.Subject = "青职二货街";
            email.Title = "青职二货街用户验证码";
            email.SendEmail = user.Email;

            EmailHelper.SendMail(email);

            user.EmailCode = code;

            UserInfoServices.Update(user);

            return Content("发送验证码成功！请查收！");
        }


        [HttpPost]
        //比较验证码
        public ActionResult CompareCode(int? id, string code)
        {

            UserInfo user = UserInfoServices.LoadEntities(u => u.Id == id).FirstOrDefault();

            if (code == user.EmailCode)
            {
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }


        //比较口号
        [HttpPost]
        public ActionResult CompareKouHao(string code)
        {
            if (code == "为了梦想努力奋斗")
            {
                return Content("ok");
            }
            else
            {
                return Content("no");
            }
        }
        #endregion

        #region 关于我们
        public ActionResult AboutUs()
        {
            return View();
        }
        #endregion

        #region 在线客服
        public ActionResult SubMit()
        {
            ViewData["qq"] = CommonHelper.GetAdminQQ();

            return View();
        }
        #endregion

        #region 登录
        public ActionResult AdminLogin(string oldurl)
        {
            if (!string.IsNullOrEmpty(oldurl))
            {
                ViewData["oldurl"] = oldurl;
            }
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(string uid, string pwd, string oldurl)
        {
            pwd = CommonHelper.GetStringMD5(pwd);
            var userAdmin = AdminUserServices.LoadEntities(u => u.Uid == uid && u.Pwd == pwd).FirstOrDefault();

            if (userAdmin == null)
            {
                return View();
            }

            Session["adminlogin"] = userAdmin;

            if (string.IsNullOrEmpty(oldurl))
            {
                return Redirect("/Admin/Home");
            }
            else
            {
                return Redirect(oldurl);
            }
        }
        #endregion

        public ActionResult Phone()
        {
            return View();
        }

        public ActionResult SignOut()
        {
            if (Session["model"] != null)
            {
                Session["model"] = null;
            }
            return Redirect("/Home/Index");
        }

    }
}
