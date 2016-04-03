using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WLXK.SecondHand.Common;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;
using WLXK.SecondHand.Model.Enum;
using WLXK.SecondHand.UI.Portal.Common;

namespace WLXK.SecondHand.UI.Portal.Controllers
{
    public class AccountController : Controller
    {
        public IUserInfoServices UserInfoServices { get; set; }
        public IUserInfoExtServices UserInfoExtServices { get; set; }

        short vaildYes = (short)IsVaild.Vailded;
        short vaildNo = (short)IsVaild.VaildIng;
        short deleteNo = (short)DelFlag.Normal;
        short deleteYes = (short)DelFlag.Delete;
        short boy = (short)Gender.Boy;


        #region 登录模块
        public ActionResult Login(string oldurl)
        {
            if (Session["model"] != null)
            {
                return Redirect("/OrderCenter/Home");
            }

            HttpCookie cookie = Request.Cookies["uid"];
            if (cookie != null)
            {
                ViewData["username"] = cookie.Value;
            }

            if (!string.IsNullOrEmpty(oldurl))
            {
                ViewData["oldurl"] = oldurl;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(string txtUserName, string txtPassWord, string Remeber, string oldurl)
        {
            if (string.IsNullOrEmpty(txtPassWord) && string.IsNullOrEmpty(txtPassWord))
            {
                return Content("请输入用户名或者密码");
            }

            //密码加密
            txtPassWord = CommonHelper.GetStringMD5(txtPassWord);

            //用户名不区分大小写
            txtUserName = txtUserName.ToLower();

            UserInfo model = UserInfoServices.LoadEntities(u => (u.Uid == txtUserName || u.Email == txtUserName) && u.Pwd == txtPassWord && u.DelFalg == deleteNo).FirstOrDefault();

            if (model == null)
            {
                return Content("用户名或者密码错误");
            }
            else
            {
                if (model.IsValid == vaildNo)
                {
                    return Content("302");
                }

                Session["model"] = model;

                #region Cookie
                HttpCookie cookie = new HttpCookie("uid", txtUserName);
                if (Remeber == null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-10);
                }
                else
                {
                    cookie.Expires = DateTime.Now.AddDays(10);
                }
                Response.Cookies.Add(cookie);
                #endregion

                if (string.IsNullOrEmpty(oldurl))
                {
                    return Content("ok|/OrderCenter/Home");
                }
                else
                {
                    return Content("ok|"+oldurl);
                }

                
            }

        }
        #endregion

        #region 注册模块
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string Uid, string Pwd1, string Pwd2, string Email, string code)
        {
            Uid = Uid.ToLower();
            Email = Email.ToLower();

            #region 验证用户名是否符合格式
            if (string.IsNullOrEmpty(Uid) || Uid.Length < 4)
            {
                return Content("用户名不能为空或者不能小于4位");
            }

            string uidState = CheckIsExistUid(Uid);
            if (uidState == "1")
            {
                return Content("用户名已经存在，请更换用户名");
            }
            #endregion

            #region 验证密码是否符合格式
            if (string.IsNullOrEmpty(Pwd1) || Pwd1.Length < 6)
            {
                return Content("密码不能为空或者不能小于6位");
            }

            if (Pwd1 != Pwd2)
            {
                return Content("两次密码输入不一致");
            }
            #endregion

            #region 验证邮箱是否符合格式

            if (!Regex.IsMatch(Email, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                return Content("请输入正确的邮箱格式");
            }

            string emailState = CheckEmail(Email);

            if (emailState == "1")
            {
                return Content("邮箱已经存在，请更换绑定邮箱");
            }
            #endregion

            #region 验证验证码是否符合格式
            if (string.IsNullOrEmpty(code))
            {
                return Content("请输入验证码");
            }

            if (Session["ValidateCode"].ToString() != code)
            {
                return Content("验证码错误，请重新输入");
            }
            #endregion

            Pwd1 = CommonHelper.GetStringMD5(Pwd1);


            UserInfo user = new UserInfo();
            user.Uid = Uid;
            user.Pwd = Pwd1;
            user.DelFalg = (short)DelFlag.Normal;
            user.IsValid = (short)IsVaild.VaildIng;
            user.SubTime = DateTime.Now;
            user.Email = Email;
            user.DelFalg = deleteYes;


            UserInfoServices.Add(user);

            UserInfoExt ext = new UserInfoExt();

            ext.Address = "";
            ext.Birthday = DateTime.Now;
            ext.Gender = boy;
            ext.NickName = "小二";
            ext.RealName = "";
            ext.TouXiang = "/Upload/default/default.jpg";
            ext.UserInfoId = user.Id;

            UserInfoExtServices.Add(ext);

            if (user.Id < 0)
            {
                return Content("注册用户失败");
            }
            
            return Content("ok");
        }
        #endregion

        #region 邮箱验证模块

        //验证邮箱跳转进入发送邮件界面
        public ActionResult EmailVaild(string email, string uid, string state)
        {

            UserInfo user = UserInfoServices.LoadEntities(u => u.Email == email && u.Uid == uid).FirstOrDefault();
            if (user == null)
            {

                return Redirect("/others/ShowMsg?OldUrl=/Account/Register&NewUrl=/Account/BinDingEmailByUidAndEmail&PointMsg=+" + Server.UrlEncode("邮箱验证码已过期，请重新绑定邮箱") + "&NewTitle=" + Server.UrlEncode("绑定邮箱界面"));
            }
            ViewData["state"] = state;
            ViewData["Uid"] = uid;
            ViewData["myemail"] = email;

            Match match = Regex.Match(email, "^([-a-zA-z0-9_.]+@([-a-zA-Z0-9_]+(.[a-zA-Z0-9.]+)+))$");
            string url = "http://mail." + match.Groups[2].Value;
            ViewData["email"] = url;

            return View();
        }


        //发送注册邮件
        public ActionResult ReSendEmail(string emailurl, string Uid)
        {
            #region 验证用户名邮箱是否正确
            UserInfo user = UserInfoServices.LoadEntities(u => u.Email == emailurl && u.Uid == Uid && u.IsValid == vaildNo).FirstOrDefault();
            if (user == null)
            {
                return Content("0");
            }
            #endregion

            MyEmail email = new MyEmail();
            email.SendEmail = emailurl;
            email.Title = Uid + "你好！完成最后一步，您在青职二货街的注册就成功了";
            email.Subject = "青职二货街";

            string vaildCode = Guid.NewGuid().ToString();
            vaildCode = CommonHelper.GetStringMD5(vaildCode);

            //生成验证连接
            string website = CommonHelper.GetWebSite();
            string href = website + "/account/binDingEmail?uid=" + Uid + "&code=" + vaildCode;
            //生成发送邮箱模版
            string html = System.IO.File.ReadAllText(Server.MapPath("/EmailTemp/register.html"));
            html = html.Replace("@username", Uid).Replace("@link", href).Replace("@website", website);

            email.Content = html;

            try
            {
                EmailHelper.SendMail(email);

                #region 邮件发送成功之后进行存储数据
                user.SendEmailTime = DateTime.Now;
                user.EmailCode = vaildCode;
                UserInfoServices.Update(user);

                return Content("1");

                #endregion


            }
            catch (Exception)
            {
                return Content("0");
            }
        }


        //验证并绑定邮箱
        public ActionResult binDingEmail(string uid, string code)
        {
            UserInfo user = UserInfoServices.LoadEntities(u => u.Uid == uid && u.IsValid == vaildNo && u.EmailCode == code).FirstOrDefault();

            DateTime sendEmail = (DateTime)user.SendEmailTime;

            int hours = DateTime.Now.Hour - sendEmail.Hour;

            if (user == null || hours >= 24)
            {

                return Redirect("/others/ShowMsg?OldUrl=/Account/BinDingEmailByUidAndEmail&NewUrl=/Account/Register&PointMsg=+" + Server.UrlEncode("邮箱验证码已过期，请重新绑定邮箱") + "&NewTitle=" + Server.UrlEncode("绑定邮箱界面"));
            }

            user.DelFalg = deleteNo;
            user.IsValid = vaildYes;
            user.EmailCode = "";

            //if (UserInfoServices.Update(user))
            //{

            UserInfoServices.Update(user);
            Session["model"] = user;
            return Redirect("/OrderCenter/Home");
            //}
            //else
            //{
            //    return Redirect("/others/ShowMsg?OldUrl=/Account/Register&NewUrl=/Account/Register&PointMsg=+" + Server.UrlEncode("邮箱验证码已过期，请重新绑定邮箱") + "&NewTitle=" + Server.UrlEncode("绑定邮箱界面"));
            //}
        }


        #endregion

        #region 通过用户名邮箱绑定页面
        public ActionResult BinDingEmailByUidAndEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BinDingEmailByUidAndEmail(string UserName, string Email, string code)
        {
            if (Session["ValidateCode"].ToString() != code)
            {
                return Content("验证码输入不正确");
            }

            UserName = UserName.ToLower();
            Email = Email.ToLower();

            UserInfo user = UserInfoServices.LoadEntities(u => u.Uid == UserName && u.Email == Email && u.DelFalg == deleteNo).FirstOrDefault();

            if (user == null)
            {
                return Content("用户名或者邮箱输入错误，请核对后重新输入");
            }

            return Content("ok");
        }

        //绑定邮箱
        public ActionResult ReBindingEmail(string emailurl, string Uid)
        {
            #region 验证用户名邮箱是否正确
            UserInfo user = UserInfoServices.LoadEntities(u => u.Email == emailurl && u.Uid == Uid && u.IsValid == vaildNo).FirstOrDefault();
            if (user == null)
            {
                return Content("0");
            }
            #endregion

            MyEmail email = new MyEmail();
            email.SendEmail = emailurl;
            email.Title = "青职二货街绑定邮件";
            email.Subject = "青职二货街";

            string vaildCode = Guid.NewGuid().ToString();
            vaildCode = CommonHelper.GetStringMD5(vaildCode);

            //生成验证连接
            string website = CommonHelper.GetWebSite();
            string href = website + "/account/binDingEmail?uid=" + Uid + "&code=" + vaildCode;
            //生成发送邮箱模版
            string html = System.IO.File.ReadAllText(Server.MapPath("/EmailTemp/banding.html"));
            html = html.Replace("@username", Uid).Replace("@link", href).Replace("@website", website);

            email.Content = html;

            try
            {
                EmailHelper.SendMail(email);

                #region 邮件发送成功之后进行存储数据
                user.SendEmailTime = DateTime.Now;
                user.EmailCode = vaildCode;
                UserInfoServices.Update(user);

                return Content("1");

                #endregion


            }
            catch (Exception)
            {
                return Content("0");
            }
        }

        #endregion

        #region 找回密码
        public ActionResult forget()
        {
            return View();
        }


        [HttpPost]
        public ActionResult forget(string UserName, string Email, string code)
        {
            if (code != Session["ValidateCode"].ToString())
            {
                return Content("验证码不正确");
            }

            UserInfo user = UserInfoServices.LoadEntities(u => u.IsValid == vaildYes && u.DelFalg == deleteNo && u.Uid == UserName && u.Email == Email).FirstOrDefault();

            if (user == null)
            {
                return Content("用户名和邮箱不匹配，请重新输入");
            }
            return Content("ok");
        }

        [HttpPost]
        public ActionResult ReFindEmail(string emailurl, string Uid)
        {
            #region 验证用户名邮箱是否正确
            UserInfo user = UserInfoServices.LoadEntities(u => u.Email == emailurl && u.Uid == Uid && u.IsValid == vaildYes).FirstOrDefault();
            if (user == null)
            {
                return Content("0");
            }
            #endregion

            MyEmail email = new MyEmail();
            email.SendEmail = emailurl;
            email.Title = "青职二货街忘记密码提示";
            email.Subject = "青职二货街";

            string vaildCode = Guid.NewGuid().ToString();
            vaildCode = CommonHelper.GetStringMD5(vaildCode);

            //生成验证连接
            string website = CommonHelper.GetWebSite();
            string href = website + "/account/ReGetPwd?uid=" + Uid + "&code=" + vaildCode;
            //生成发送邮箱模版
            string html = System.IO.File.ReadAllText(Server.MapPath("/EmailTemp/forgettemp.html"));
            html = html.Replace("@username", Uid).Replace("@link", href).Replace("@website", website);

            email.Content = html;

            try
            {
                EmailHelper.SendMail(email);

                #region 邮件发送成功之后进行存储数据
                user.SendEmailTime = DateTime.Now;
                user.EmailCode = vaildCode;
                UserInfoServices.Update(user);

                return Content("1");

                #endregion


            }
            catch (Exception)
            {
                return Content("0");
            }
        }

        public ActionResult ReGetPwd(string uid, string code)
        {
            UserInfo user = UserInfoServices.LoadEntities(u => u.Uid == uid && u.EmailCode == code && u.DelFalg == deleteNo && u.IsValid == vaildYes).FirstOrDefault();


            if (user == null)
            {
                return Redirect("/others/ShowMsg?OldUrl=/Account/forget&NewUrl=/Account/forget&PointMsg=+" + Server.UrlEncode("邮箱验证码已过期，请重新进行找回密码操作") + "&NewTitle=" + Server.UrlEncode("找回密码界面"));
            }
            int spantime = 5;
            spantime = DateTime.Now.Hour - ((DateTime)user.SendEmailTime).Hour;

            if (spantime >= 2)
            {
                return Redirect("/others/ShowMsg?OldUrl=/Account/forget&NewUrl=/Account/forget&PointMsg=+" + Server.UrlEncode("邮箱验证码已过期，请重新进行找回密码操作") + "&NewTitle=" + Server.UrlEncode("找回密码界面"));
            }

            ViewData["UserName"] = uid;
            return View();
        }

        [HttpPost]

        public ActionResult ReGetPwd(string uid, string Pwd1, string Pwd2, string code)
        {
            if (string.IsNullOrEmpty(Pwd1))
            {
                return Content("请输入密码");
            }

            if (Pwd1 != Pwd2)
            {
                return Content("两次密码输入不一致");
            }

            if (Session["ValidateCode"].ToString() != code)
            {
                return Content("验证码输入不一致");
            }

            UserInfo user = UserInfoServices.LoadEntities(u => u.Uid == uid && u.IsValid == vaildYes && u.DelFalg == deleteNo).FirstOrDefault();

            if (user == null)
            {
                return Content("重置密码失败");
            }

            user.Pwd = CommonHelper.GetStringMD5(Pwd1.ToLower());
            user.EmailCode = "";
            UserInfoServices.Update(user);

            Session["model"] = user;

            return Content("ok");
        } 
        #endregion

        #region 其他相关模块

        /// <summary>
        /// 验证用户名是否存在
        /// </summary>
        /// <param name="uid">用户名</param>
        /// <returns></returns>
        public ActionResult CheckUid(string uid)
        {
            return Content(CheckIsExistUid(uid));
        }


        /// <summary>
        /// 验证邮箱是否存在
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public ActionResult CheckExistEmail(string email)
        {
            return Content(CheckEmail(email));
        }

        //Controller Action
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
        #endregion

        #region 方法
        /// <summary>
        /// 判断邮箱是否绑定
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string CheckEmail(string email)
        {
            UserInfo user = UserInfoServices.LoadEntities(u => u.Email == email && u.IsValid == vaildYes).FirstOrDefault();

            if (user == null)
            {
                return "0";
            }
            else
            {
                return "1";
            }
        }


        public string CheckIsExistUid(string uid)
        {

            UserInfo user = UserInfoServices.LoadEntities(u => u.Uid == uid && u.IsValid == vaildYes&&u.DelFalg==deleteYes).FirstOrDefault();

            if (user == null)
            {
                return "0";
            }
            else
            {
                return "1";
            }
        }
        #endregion
    }
}
