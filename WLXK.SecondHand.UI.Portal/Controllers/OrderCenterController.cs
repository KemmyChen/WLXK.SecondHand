using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    public class OrderCenterController : BaseController
    {
        public IUserInfoServices UserInfoServices { get; set; }
        public IGoodsServices GoodsServices { get; set; }
        public ISaleInfoServices SaleInfoServices { get; set; }
        public IUserInfoExtServices UserInfoExtServices { get; set; }
        public IShopingServices ShopingServices { get; set; }
        public IMySeeServices MySeeServices { get; set; }
        public IListShopsServices ListShopsServices { get; set; }
        public IMyNewsServices MyNewsServices { get; set; }
        public IAdminUserServices AdminUserServices { get; set; }


        //数据状态
        public short normal = (short)DelFlag.Normal;
        public short delete = (short)DelFlag.Delete;

        //店铺状态
        public short success = (short)OpenShopStatus.Success;
        public short filed = (short)OpenShopStatus.Filed;
        public short auditing = (short)OpenShopStatus.Auditing;

        //消息是否已看
        public short noSeeMyNews = (short)MyNewsIsSee.No;
        public short yesSeeMyNews = (short)MyNewsIsSee.Yes;

        #region 我的小二
        #region 主页
        public ActionResult Home(int? pageindex)
        {
            if (pageindex == null || pageindex < 1)
            {
                pageindex = 1;
            }

            //未读消息数量
            int weiduNews = MyNewsServices.LoadEntities(u => u.IsSee == noSeeMyNews && u.UserInfoId == CurrentLoginUser.Id).Count();

            ViewData["weiduNews"] = weiduNews;

            ViewData.Model = CurrentLoginUser;

            var userext = UserInfoExtServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();
            ViewData["userext"] = userext;

            int total = 0;
            IQueryable<Goods> good = GoodsServices.LoadPageEntities((int)pageindex, 9, out total, u => u.DelGlag == normal, u => u.Id, false);

            ViewData["RandGoods"] = good;
            return View();
        }
        #endregion

        #region 显示收藏夹
        public ActionResult Shop(int? pageIndex)
        {
            var goodIds = ShopingServices.LoadEntities(s => s.UserInfoId == CurrentLoginUser.Id && s.DelFlag == normal);

            if (pageIndex == null || pageIndex <= 0)
            {
                pageIndex = 1;
            }

            List<Shops> list = new List<Shops>();

            int total = 0;

            if (goodIds != null)
            {

                foreach (var item in goodIds)
                {
                    Shops shop = new Shops();
                    int goodid = item.GoodsId;
                    Goods good = GoodsServices.LoadPageEntities((int)pageIndex, 5, out total, u => u.Id == goodid
                        , u => u.Id, true).FirstOrDefault();

                    shop.ShopId = item.Id;
                    shop.Goods = good;
                    list.Add(shop);
                }
            }

            ViewData["allGoods"] = list;
            ViewData["pageIndex"] = pageIndex;
            ViewData["Count"] = total;
            return View();
        }
        #endregion

        #region 删除收藏夹
        [HttpPost]
        public ActionResult DeleteShop(int id)
        {
            Shoping shop = ShopingServices.LoadEntities(s => s.Id == id).FirstOrDefault();

            if (shop == null)
            {
                return Content("数据错误，请刷新本页面");
            }
            shop.DelFlag = delete;

            try
            {
                ShopingServices.Update(shop);
                return Content("ok");
            }
            catch (Exception)
            {
                return Content("数据库错误，请联系管理员");
                throw;
            }

        }

        #endregion

        #region 我的足迹
        public ActionResult MySee(int? pageIndex)
        {
            if (pageIndex <= 0 || pageIndex == null)
            {
                pageIndex = 1;
            }

            int total = 0;

            var mysee = MySeeServices.LoadPageEntities((int)pageIndex, 9, out total, u => u.UserInfoId == CurrentLoginUser.Id, u => u.SubTime, false).ToList();
            ViewData["mysee"] = mysee;

            ViewData["pageIndex"] = pageIndex;
            ViewData["Count"] = total;

            return View();
        }
        #endregion

        #region 我的消息
        public ActionResult MyNews(int? pageIndex)
        {
            if (pageIndex == null || pageIndex <= 0)
            {
                pageIndex = 1;
            }
            int total = 0;

            var allnews = MyNewsServices.LoadPageEntities((int)pageIndex, 8, out total, u => u.UserInfoId == CurrentLoginUser.Id, u => u.SubTime, false).ToList();

            var news = allnews.OrderBy(u => u.IsSee);

            ViewData["mynews"] = allnews;

            ViewData["pageIndex"] = pageIndex;
            ViewData["Count"] = total;

            return View();
        }
        #endregion

        #region 消息标为已读
        public ActionResult SetSee(int id)
        {
            var myNews = MyNewsServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            myNews.IsSee = yesSeeMyNews;

            try
            {
                MyNewsServices.Update(myNews);

                Session["model"] = UserInfoServices.LoadEntities(u => u.Id == CurrentLoginUser.Id).FirstOrDefault();

                return Content("设置成功");
            }
            catch (Exception)
            {
                return Content("数据库错误，请联系管理员");
            }

        }
        #endregion

        #endregion

        #region 卖家中心

        #region 增加宝贝
        public ActionResult AddGoods()
        {
            if (IsOpenShops())
            {
                ViewData["message"] = "1";
            }
            else
            {
                ViewData["message"] = "0";
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddGoods(Goods good, SaleInfo sale)
        {

            UserInfo user = CurrentLoginUser;

            if (user == null)
            {
                return Redirect("/Account/login");
            }

            good.DelGlag = normal;
            good.SubTime = DateTime.Now;

            var shops = ListShopsServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();
            good.ShopingId = shops.Id;

            try
            {
                GoodsServices.Add(user, good, sale);

                #region Luncene创建索引
                JobSerach search = new JobSerach();
                search.Id = good.Id;
                search.Title = good.Title;
                search.Price = (double)good.Price;
                search.ImageAddress = good.SmallImageAddress;
                search.Content = good.Descript;
                search.MaiDian = good.MaiDian;

                IndexManager.myManager.Add(search);
                #endregion



                return Content("ok");
            }
            catch (Exception)
            {
                return Content("数据错误，请联系管理员");
                throw;
            }
        }
        #endregion

        #region 展示所有自己的宝贝
        public ActionResult ShowGoods(int? pageIndex)
        {
            if (IsOpenShops())
            {
                ViewData["message"] = "1";
                if (pageIndex == null)
                {
                    pageIndex = 1;
                }

                UserInfo user = CurrentLoginUser;

                if (user == null)
                {
                    return Redirect("/Account/Login");
                }

                int total = 0;
                var goods = GoodsServices.LoadPageEntities((int)pageIndex, 5, out total, u => u.DelGlag == normal && u.UserInfoId == user.Id, o => o.Id, false).AsQueryable();

                if (goods == null)
                {
                    return Content("数据库数据错误！请联系管理员");
                }

                ViewData["allGoods"] = goods;

                ViewData["pageIndex"] = pageIndex;

                ViewData["Count"] = total;
            }
            else
            {
                ViewData["message"] = "0";
            }
            return View();
        }
        #endregion

        #region 修改商品信息
        public ActionResult EditGoods(int? id)
        {
            if (id == null)
            {
                return Content("参数错误");
            }
            UserInfo user = CurrentLoginUser;

            if (user == null)
            {
                return Redirect("/Account/Login");
            }

            var good = GoodsServices.LoadEntities(u => u.DelGlag == normal && u.UserInfoId == user.Id && u.Id == id).FirstOrDefault();


            var sale = SaleInfoServices.LoadEntities(u => u.GoodsId == id).FirstOrDefault();

            if (good == null || sale == null)
            {
                return Content("参数错误");
            }


            ViewBag.Sale = sale;

            return View(good);
        }

        [HttpPost]
        public ActionResult EditGoods(Goods good, SaleInfo sale, int GoodId, int SaleId)
        {

            var oldgood = GoodsServices.LoadEntities(u => u.Id == GoodId).FirstOrDefault();

            oldgood.Title = good.Title;
            oldgood.Price = good.Price;
            oldgood.MaiDian = good.MaiDian;
            oldgood.LaiYuan = good.LaiYuan;
            oldgood.SmallImageAddress = good.SmallImageAddress;
            oldgood.BigImageAddress = good.BigImageAddress;
            oldgood.Count = good.Count;
            oldgood.Descript = good.Descript;
            oldgood.Type = good.Type;
            good.Id = GoodId;

            var oldsale = SaleInfoServices.LoadEntities(u => u.GoodsId == GoodId).FirstOrDefault();

            oldsale.PhoneNum = sale.PhoneNum;
            oldsale.QQ = sale.QQ;
            oldsale.RealName = sale.RealName;
            oldsale.Address = sale.Address;
            oldsale.Id = SaleId;

            try
            {
                SaleInfoServices.Update(oldsale);

                GoodsServices.Update(oldgood);

                #region Luncene创建索引
                JobSerach search = new JobSerach();
                search.Id = good.Id;
                search.Title = good.Title;
                search.Price = (double)good.Price;
                search.ImageAddress = good.SmallImageAddress;
                search.Content = good.Descript;
                search.MaiDian = good.MaiDian;

                IndexManager.myManager.Add(search);
                #endregion

                return Content("信息更新成功");
            }
            catch (Exception)
            {
                return Content("信息更新失败，请按照格式输入");
            }
        }
        #endregion

        #region 删除商品
        public ActionResult DeleteGoods(int id)
        {
            UserInfo user = CurrentLoginUser;

            var good = GoodsServices.LoadEntities(u => u.Id == id && u.UserInfoId == user.Id).FirstOrDefault();

            good.DelGlag = delete;

            try
            {
                GoodsServices.Update(good);
                IndexManager.myManager.Delete(good.Id);
                return Content("ok");
            }
            catch (Exception)
            {
                return Content("删除失败！请联系管理员");
                throw;
            }

        }
        #endregion

        #region 回收箱
        public ActionResult DrawBack(int? pageIndex)
        {
            if (IsOpenShops())
            {
                ViewData["message"] = "1";
                if (pageIndex == null)
                {
                    pageIndex = 1;
                }

                UserInfo user = CurrentLoginUser;

                if (user == null)
                {
                    return Redirect("/Account/Login");
                }

                int total = 0;
                var goods = GoodsServices.LoadPageEntities((int)pageIndex, 5, out total, u => u.DelGlag == delete && u.UserInfoId == user.Id, o => o.Id, true).AsQueryable();

                if (goods == null)
                {
                    return Content("数据库数据错误！请联系管理员");
                }

                ViewData["allGoods"] = goods;

                ViewData["pageIndex"] = pageIndex;

                ViewData["Count"] = total;
            }
            else
            {
                ViewData["message"] = "0";
            }
            return View();
        }
        public ActionResult RemeoveDraw(int id)
        {
            UserInfo user = CurrentLoginUser;

            var good = GoodsServices.LoadEntities(u => u.Id == id && u.UserInfoId == user.Id).FirstOrDefault();

            good.DelGlag = normal;

            try
            {
                GoodsServices.Update(good);

                #region Lucnen创建索引
                JobSerach search = new JobSerach();
                search.Id = good.Id;
                search.Title = good.Title;
                search.Price = (double)good.Price;
                search.ImageAddress = good.SmallImageAddress;
                search.Content = good.Descript;
                search.MaiDian = good.MaiDian;

                IndexManager.myManager.Add(search);
                #endregion

                return Content("ok");
            }
            catch (Exception)
            {
                return Content("删除失败！请联系管理员");
            }

        }
        #endregion

        #region 开店流程
        public ActionResult OpenShop()
        {
            var shops = ListShopsServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();
            if (shops == null)
            {
                ViewData["ShopStatus"] = "0";
            }
            else
            {
                ViewData["ShopStatus"] = shops.Status;
            }

            ViewData["userid"] = CurrentLoginUser.Id;
            return View();
        }

        [HttpPost]
        public ActionResult OpenShop(ListShops shop, string IsCode, string IsKouHao)
        {
            ViewData["realname"] = shop.RealName;
            ViewData["classname"] = shop.CalssName;
            ViewData["idcard"] = shop.IdCard;

            if (IsCode != "1" || IsKouHao != "1")
            {
                ViewData["message"] = "请按照步骤完成任务操作";
                ViewData["ShopStatus"] = "0";
                return View();
            }


            string imagefull =string.Empty;

           #region 学生证
		 HttpPostedFileBase jpeg_image_upload = Request.Files[0];

            //获取文件名
            string filename = Path.GetFileName(jpeg_image_upload.FileName);

            //获取文件类型
            string fileext = Path.GetExtension(filename);

            if (fileext == ".jpg" || fileext == ".png")
            {
                string guid = Guid.NewGuid().ToString();

                string bigdir = "/Upload/" + "big/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/";
                Directory.CreateDirectory(Path.GetDirectoryName(Server.MapPath(bigdir)));
                string full = bigdir + guid + "_大图" + fileext;
                jpeg_image_upload.SaveAs(Server.MapPath(full));
                imagefull = full;
            } 
	#endregion
            shop.CardAddress = imagefull;
            

            if(string.IsNullOrEmpty(imagefull))
            {
                ViewData["message"] = "请上传学生证照片";
                ViewData["ShopStatus"] = "0";
                return View();
            }

            var shops = ListShopsServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();

            if (shops != null)
            {
                if (shops.Status == auditing)
                {
                    ViewData["message"] = "您已提交开店申请，正在审核中，请耐心等候";
                    ViewData["ShopStatus"] = auditing;
                    return View();
                }
                else if (shops.Status == success)
                {
                    ViewData["message"] = "您已开通店铺，请进行使用吧";
                    ViewData["ShopStatus"] = success;
                    return View();
                }
                else if (shops.Status == filed)
                {
                    var dbShop = ListShopsServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();

                    dbShop.SubTime = DateTime.Now;
                    dbShop.Status = auditing;
                    dbShop.IdCard = shop.IdCard;
                    dbShop.CalssName = shop.CalssName;
                    dbShop.CardAddress = shop.CardAddress;
                    dbShop.RealName = shop.RealName;
                    ListShopsServices.Update(dbShop);
                }
            }
            else
            {

                shop.UserInfoId = CurrentLoginUser.Id;
                shop.SubTime = DateTime.Now;
                shop.Status = auditing;
                shop.CardAddress = imagefull;
                ListShopsServices.Add(shop);
            }

            var adminList = AdminUserServices.LoadEntities(u => true).ToList();

            foreach (var item in adminList)
            {
                MyEmail email = new MyEmail();
                email.SendEmail = item.Email;
                email.Title = CurrentLoginUser.Uid + "正在进行申请进行开店操作";
                email.Subject = "青职二货街";
                //生成验证连接
                string website = CommonHelper.GetWebSite();
                //生成发送邮箱模版
                string html = System.IO.File.ReadAllText(Server.MapPath("/EmailTemp/OpenShops.html"));
                html = html.Replace("@website", website).Replace("@ids", shop.Id.ToString()).Replace("@RealName", shop.RealName).Replace("@IdCard", shop.IdCard).Replace("@ClassName", shop.CalssName).Replace("@imgsrc", website + shop.CardAddress);

                email.Content = html;

                EmailSendToAllAdmin.SendEmail(email);

            }


            ViewData["message"] = "您已提交开店申请，正在审核中，请耐心等候";
            ViewData["ShopStatus"] = "-1";

            return View();
        }
        #endregion

        #endregion

        #region 账户设置

        #region 个人资料管理
        public ActionResult MyInfoUp()
        {

            var dbUserExt = UserInfoExtServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();

            ViewData["MyInfo"] = dbUserExt;

            return View();
        }

        [HttpPost]
        public ActionResult MyInfoUp(UserInfoExt ext)
        {
            var userext = UserInfoExtServices.LoadEntities(u => u.Id == ext.Id).FirstOrDefault();

            if (userext != null)
            {
                userext.NickName = ext.NickName;
                userext.RealName = ext.RealName;
                userext.Birthday = ext.Birthday;
                userext.Gender = ext.Gender;
                userext.Address = ext.Address;

                try
                {
                    UserInfoExtServices.Update(userext);
                    return Content("更新成功");
                }
                catch (Exception)
                {

                    return Content("请按照格式输入");
                }

            }
            return Content("更新失败");
        }
        #endregion

        #region 修改头像
        public ActionResult ImageUpdate()
        {
            var userext = UserInfoExtServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();
            ViewData["userext"] = userext;

            return View();
        }
        [HttpPost]
        public ActionResult ImageUpdate(UserInfoExt ext)
        {
            var userext = UserInfoExtServices.LoadEntities(u => u.Id == ext.Id).FirstOrDefault();

            if (ext.TouXiang == null)
            {
                ViewData["userext"] = userext;
                ViewData["msg"] = "请进行上传头像";
                return View();
            }

            userext.TouXiang = ext.TouXiang;

            try
            {
                UserInfoExtServices.Update(userext);
                ViewData["userext"] = userext;
                ViewData["msg"] = "修改成功";
                return View();
            }
            catch (Exception)
            {
                ViewData["msg"] = "修改失败";
                ViewData["userext"] = userext;
                return View();
            }
        }
        #endregion

        #endregion

        #region 其他相关操作
        //判断时候开通店铺
        public bool IsOpenShops()
        {
            var shops = ListShopsServices.LoadEntities(u => u.UserInfoId == CurrentLoginUser.Id).FirstOrDefault();

            if (shops == null)
            {
                return false;
            }
            else
            {
                if (shops.Status != success)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }
        #endregion
    }
}
