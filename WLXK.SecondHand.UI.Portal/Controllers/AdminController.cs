using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WLXK.SecondHand.Common;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;
using WLXK.SecondHand.Model.Enum;
using WLXK.SecondHand.UI.Portal.Common;

namespace WLXK.SecondHand.UI.Portal.Controllers
{
    public class AdminController : AdminBaseController
    {
        public IFriendLinkServices FriendLinkServices { get; set; }
        public IListShopsServices ListShopsServices { get; set; }
        public IMyNewsServices MyNewsServices { get; set; }
        public IUserInfoServices UserInfoServices { get; set; }
        public IGoodsServices GoodsServices { get; set; }
        public IAdminUserServices AdminUserServices { get; set; }
        public IUserInfoExtServices UserInfoExtServices { get; set; }


        public short refuse = 0;
        public short allow = 1;

        //消息是否已看
        public short noSeeMyNews = (short)MyNewsIsSee.No;
        public short yesSeeMyNews = (short)MyNewsIsSee.Yes;
        short deleteNo = (short)DelFlag.Normal;
        short deleteYes = (short)DelFlag.Delete;

        public ActionResult Home()
        {
            return View();
        }

        #region 开店相关
        public ActionResult RefuseOpenShops(int id)
        {
            ListShops shops = ListShopsServices.LoadEntities(u => u.Id == id).FirstOrDefault();

            if (shops.IsShenHe == 1)
            {
                ViewData["alert"] = "已有其他管理员操作";
                return View();
            }

            shops.IsShenHe = 1;

            shops.Status = refuse;
            ListShopsServices.Update(shops);

            #region 更新验证码
            var dbUser = UserInfoServices.LoadEntities(u => u.Id == shops.UserInfoId).FirstOrDefault();
            dbUser.EmailCode = "";

            UserInfoServices.Update(dbUser);

            #endregion

            MyNews news = new MyNews();
            news.IsSee = noSeeMyNews;
            news.SubTime = DateTime.Now;
            news.Title = "您的店铺申请失败，请认真核查您的信息，进行重新申请操作";
            news.UserInfoId = shops.UserInfoId;

            MyNewsServices.Add(news);

            ViewData["alert"] = "拒绝开店成功";

            return View();
        }
        public ActionResult AllowOpenShops(int id)
        {
            ListShops shops = ListShopsServices.LoadEntities(u => u.Id == id).FirstOrDefault();

            if (shops.IsShenHe == 1)
            {
                ViewData["alert"] = "已有其他管理员操作";
                return View();
            }

            shops.IsShenHe = 1;

            shops.SheHeAdminId = ((AdminUser)Session["adminlogin"]).Id;

            shops.Status = allow;
            ListShopsServices.Update(shops);

            #region 更新验证码
            var dbUser = UserInfoServices.LoadEntities(u => u.Id == shops.UserInfoId).FirstOrDefault();
            dbUser.EmailCode = "";

            UserInfoServices.Update(dbUser);

            #endregion

            MyNews news = new MyNews();
            news.IsSee = noSeeMyNews;
            news.SubTime = DateTime.Now;
            news.Title = "您的店铺申请成功，开启您的二手市场之旅吧";
            news.UserInfoId = shops.UserInfoId;

            MyNewsServices.Add(news);

            ViewData["alert"] = "允许开店成功";

            return View();
        }
        #endregion

        #region 用户表
        public ActionResult ListUser()
        {
            ViewData.Model = UserInfoServices.LoadEntities(u => u.DelFalg==deleteNo);
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserInfo user)
        {
            user.Pwd = CommonHelper.GetStringMD5(user.Pwd);

            UserInfoServices.Add(user);

            UserInfoExt ext = new UserInfoExt();

            ext.Address = "";
            ext.Birthday = DateTime.Now;
            ext.Gender = 1;
            ext.NickName = "小二";
            ext.RealName = "";
            ext.TouXiang = "/Upload/default/default.jpg";
            ext.UserInfoId = user.Id;

            UserInfoExtServices.Add(ext);


            return Redirect("/Admin/ListUser");
        }

        public ActionResult Edit(int id)
        {
            ViewData.Model = UserInfoServices.LoadEntities(u => u.Id == id).FirstOrDefault();

            return View();
        }

        [HttpPost]
        public ActionResult Edit(UserInfo user)
        {
            user.Pwd = CommonHelper.GetStringMD5(user.Pwd);

            UserInfoServices.Update(user);

            return Redirect("/Admin/ListUser");
        }

        public ActionResult DeleteUser(int id)
        {
            var user =UserInfoServices.LoadEntities(u => u.Id == id).FirstOrDefault();

            user.DelFalg = 0;
            user.IsValid = 0;

            UserInfoServices.Update(user);

            return Redirect("/Admin/ListUser");
        }
        #endregion

        #region 商品表
        public ActionResult ListGoods()
        {
            ViewData.Model = GoodsServices.LoadEntities(u => u.DelGlag==deleteNo);

            return View();
        }

        public ActionResult EditGoods(int id)
        {
            ViewData.Model = GoodsServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public ActionResult EditGoods(Goods goods)
        {
            GoodsServices.Update(goods);
            return Redirect("/Admin/ListGoods");
        }

        public ActionResult DetailsGoods(int id)
        {
            ViewData.Model = GoodsServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            return View();
        }

        public ActionResult DeleteGoods(int id)
        {
            var good = GoodsServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            good.DelGlag = 0;

            GoodsServices.Update(good);

            return Redirect("/Admin/ListGoods");
        }

        #endregion

        #region 店铺管理
        public ActionResult ListDianpu()
        {
            ViewData.Model = ListShopsServices.LoadEntities(u => u.Status == 1).ToList();

            return View();
        }
        public ActionResult DeleteDianPu(int id)
        {
            ListShops shop = ListShopsServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            shop.Status = 0;
            shop.IsShenHe = 0;
            ListShopsServices.Update(shop);

            #region 给用户发送消息
            MyNews news = new MyNews();
            news.SubTime = DateTime.Now;
            news.Title = "您的店铺由于违反相关法律法规已被管理员关闭，如果您想再次开店请先进行开店申请操作";
            news.UserInfoId = shop.UserInfoId;
            news.IsSee = 0;
            MyNewsServices.Add(news);
            #endregion

            #region 删除用户所有商品信息
            var listGoods = GoodsServices.LoadEntities(u => u.UserInfoId == shop.UserInfoId).ToList();
            foreach (var item in listGoods)
            {
                item.DelGlag = 0;
                GoodsServices.Update(item);
            }
            #endregion


            return Redirect("/Admin/ListDianpu");
        }

        public ActionResult CreateDianPu()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDianPu(ListShops list)
        {
            ListShopsServices.Add(list);
            return Redirect("/Admin/ListDianpu");
        }
        #endregion

        #region 管理员添加与管理
        public ActionResult AdminUserList()
        {
            if (CurrentLoginUser.Uid != "admin")
            {
                return Redirect("/Admin/Home");
            }

            ViewData.Model = AdminUserServices.LoadEntities(u => true);
            return View();
        }

        public ActionResult CreateAdminUser()
        {

            if (CurrentLoginUser.Uid != "admin")
            {
                return Redirect("/Admin/Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdminUser(AdminUser user)
        {
            int count =AdminUserServices.LoadEntities(u => u.Uid == user.Uid).Count();

            if (count > 0)
            {
                ViewData["message"] = "用户名已存在";
                return View();
            }

            string pwd = user.Pwd;


            if (CurrentLoginUser.Uid != "admin")
            {
                return Redirect("/Admin/Home");
            }
            user.Pwd = CommonHelper.GetStringMD5(user.Pwd);
            AdminUserServices.Add(user);

            MyEmail email = new MyEmail();
            email.SendEmail = user.Email;
            email.Title = "您在青职二货街的系统管理员账户已开通";
            email.Subject = "青职二货街-系统管理员";
            string website = CommonHelper.GetWebSite();

            string html = "恭喜！您的青职二货街管理账户已开通！ 用户名:" + user.Uid + "，密码：" + pwd + " 登录网址为" + website + "/others/adminlogin 。为了系统安全，请您及时修改密码。（此邮件由系统自动发出）";

            email.Content = html;

            EmailSendToAllAdmin.SendEmail(email);


            return Redirect("/Admin/AdminUserList");
        }

        public ActionResult LookDianPu(int id)
        {

            if (CurrentLoginUser.Uid != "admin")
            {
                return Redirect("/Admin/Home");
            }
            ViewData.Model = ListShopsServices.LoadEntities(u => u.SheHeAdminId == id).ToList();
            return View();
        }

        public ActionResult EditAdmin(int id)
        {

            if (CurrentLoginUser.Uid != "admin")
            {
                return Redirect("/Admin/Home");
            }

            ViewData.Model = AdminUserServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public ActionResult EditAdmin(AdminUser user)
        {
            
            if (CurrentLoginUser.Uid != "admin")
            {
                return Redirect("/Admin/Home");
            }
            user.Pwd = CommonHelper.GetStringMD5(user.Pwd);

            AdminUserServices.Update(user);
            return Redirect("/Admin/AdminUserList");
        }
        public ActionResult DeleteAdmin(int id)
        {

            if (CurrentLoginUser.Uid != "admin")
            {
                return Redirect("/Admin/Home");
            }
            AdminUserServices.Delete(id);

            return Redirect("/Admin/AdminUserList");
        } 
        #endregion

        #region 密码修改
        public ActionResult AdminPwd()
        {
            ViewData.Model = AdminUserServices.LoadEntities(u => u.Id == CurrentLoginUser.Id).FirstOrDefault();

            return View();
        }
        [HttpPost]
        public ActionResult AdminPwd(AdminUser user)
        {
            user.Pwd = CommonHelper.GetStringMD5(user.Pwd);

            AdminUserServices.Update(user);

            return Redirect("/Admin/Home");
        } 
        #endregion

        public ActionResult FriendLink()
        {
            ViewData.Model = FriendLinkServices.LoadEntities(u => true).ToList();

            return View();
        }
        public ActionResult CreateLink()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateLink(FriendLink friend)
        {

            FriendLinkServices.Add(friend);

            return Redirect("/Admin/FriendLink");
        }

        public ActionResult DeleteLink(int id)
        {
            FriendLinkServices.Delete(id);

            return Redirect("/Admin/FriendLink");
        }


    }
}
