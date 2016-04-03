using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WLXK.SecondHand.Common;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;
using WLXK.SecondHand.Model.Enum;
using WLXK.SecondHand.UI.Portal.Common;

namespace WLXK.SecondHand.UI.Portal.Controllers
{
    public class HomeController : Controller
    {
        public IFriendLinkServices FriendLinkServices { get; set; }
        public IUserInfoServices UserInfoServices { get; set; }
        public ISaleInfoServices SaleInfoServices { get; set; }
        public IGoodsServices GoodsServices { get; set; }
        public IMySeeServices MySeeServices { get; set; }
        public IShopingServices ShopingServices { get; set; }
        public IListShopsServices ListShopsServices { get; set; }
        public IAdminUserServices AdminUserServices { get; set; }
        public IUserInfoExtServices UserInfoExtServices { get; set; }


        short normal = (short)DelFlag.Normal;
        short News = (short)WLXK.SecondHand.Model.Enum.Type.News;
        short Nine = (short)WLXK.SecondHand.Model.Enum.Type.Nine;
        short Eigh = (short)WLXK.SecondHand.Model.Enum.Type.Eigh;
        short Seven = (short)WLXK.SecondHand.Model.Enum.Type.Seven;
        short Six = (short)WLXK.SecondHand.Model.Enum.Type.Six;
        short Others = (short)WLXK.SecondHand.Model.Enum.Type.Others;

        #region 主页

        [OutputCache(Duration = 60, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]  
        public ActionResult Index()
        {
            if (Session["model"] != null)
            {
                UserInfo user = Session["model"] as UserInfo;
                var ext = UserInfoExtServices.LoadEntities(u => u.UserInfoId == user.Id).FirstOrDefault();
                ViewData["ext"] = ext;
            }

            //获取前四名热门商品

            int total = 0;
            ViewData["SlideSales"] = GoodsServices.LoadPageEntities(1, 4, out total, u => u.DelGlag == normal, u => u.SeeCount, false).ToList();

            //获取第五名到第二十名的商品
            var sales = GoodsServices.LoadPageEntities(2, 4, out total, u => u.DelGlag == normal, u => u.SeeCount, false).ToList();
            var sales1 = GoodsServices.LoadPageEntities(3, 4, out total, u => u.DelGlag == normal, u => u.SeeCount, false).ToList();
            var sales2 = GoodsServices.LoadPageEntities(4, 4, out total, u => u.DelGlag == normal, u => u.SeeCount, false).ToList();
            sales.AddRange(sales1);
            sales.AddRange(sales2);
            ViewData["SecondSales"] = sales;

            //获取最新商品
            ViewData["NewGoods"] = GoodsServices.LoadPageEntities(1, 11, out total, u => u.DelGlag == normal, u => u.SubTime, false).ToList();


            //获取全新商品
            ViewData["NewsGoods"] = GoodsServices.LoadPageEntities(1, 11, out total, u => u.DelGlag == normal && u.Type == News, u => u.SubTime, false).ToList();

            //获得九成新商品
            ViewData["NineGoods"] = GoodsServices.LoadPageEntities(1, 11, out total, u => u.DelGlag == normal && u.Type == Nine, u => u.SubTime, false).ToList();

            //获得八成新商品
            ViewData["EighGoods"] = GoodsServices.LoadPageEntities(1, 11, out total, u => u.DelGlag == normal && u.Type == Eigh, u => u.SubTime, false).ToList();

            //获得七成新商品
            ViewData["SevenGoods"] = GoodsServices.LoadPageEntities(1, 11, out total, u => u.DelGlag == normal && u.Type == Seven, u => u.SubTime, false).ToList();

            //获得六成新商品
            ViewData["SixGoods"] = GoodsServices.LoadPageEntities(1, 11, out total, u => u.DelGlag == normal && u.Type == Six, u => u.SubTime, false).ToList();

            //获得其它商品
            ViewData["OthersGoods"] = GoodsServices.LoadPageEntities(1, 11, out total, u => u.DelGlag == normal && u.Type == Others, u => u.SubTime, false).ToList();

            ViewData["FriendLink"] = FriendLinkServices.LoadEntities(u => true).ToList();



            return View();
        }
        #endregion

        #region 搜索
        public ActionResult Search(string key, int? pageIndex)
        {
            if (key == null)
            {
                key = "";
            }

            #region 侧边栏和底部栏目
            //获取前五名热门商品

            int total = 0;
            ViewData["SlideSales"] = GoodsServices.LoadPageEntities(1, 5, out total, u => u.DelGlag == normal, u => u.SeeCount, false).ToList();

            //获取相关商品
            var listGoods = GoodsServices.LoadEntities(u => u.DelGlag == normal).ToList();
            int length = listGoods.Count();
            int[] goodid = new int[length];

            for (int i = 0; i < listGoods.Count(); i++)
            {
                goodid[i] = listGoods[i].Id;
            }


            Random rand = new Random();
            List<Goods> listAbout = new List<Goods>();

            if (listGoods.Count > 1)
            {

                for (int i = 0; i < 5; i++)
                {
                    int id = rand.Next(0, listGoods.Count());
                    Goods good = GoodsServices.LoadEntities(u => u.Id == goodid[id]).FirstOrDefault();
                    listAbout.Add(good);
                }
            }

            ViewData["listAbout"] = listAbout;
            #endregion

            #region 搜索部分
            ViewData["key"] = key;

            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            ViewData["pageIndex"] = pageIndex;

            List<JobSerach> list = new List<JobSerach>();

            if (!string.IsNullOrEmpty(key))
            {
                int index = ((int)pageIndex - 1) * 16 + 1;
                list = LunceneHelper.SearchContent(key, index, 16);
                ViewData["searchs"] = list;
            }
            ViewData["searchs"] = list;

            list = LunceneHelper.SearchContent(key);

            ViewData["Count"] = list.Count();
            #endregion

            return View();
        }
        #endregion

        #region 浏览商品
        [OutputCache(Duration = int.MaxValue, VaryByParam = "id")]  
        public ActionResult ShowGoods(int? id)
        {
            #region 浏览量控制
            Goods oldgoods = GoodsServices.LoadEntities(u => u.Id == id).FirstOrDefault();
            oldgoods.SeeCount = oldgoods.SeeCount + 1;
            GoodsServices.Update(oldgoods);
            #endregion

            #region 如果已登录用户添加足迹
            if (Session["model"] != null)
            {
                UserInfo user = (UserInfo)Session["model"];

                //判断是否添加足迹到表
                MySee dbSee = MySeeServices.LoadEntities(u => u.GoodsId == id && u.UserInfoId == user.Id).FirstOrDefault();

                if (dbSee != null)
                {
                    dbSee.SubTime = DateTime.Now;
                    MySeeServices.Update(dbSee);
                }
                else
                {
                    //添加我的足迹
                    MySee see = new MySee();
                    see.GoodsId = (int)id;
                    see.UserInfoId = ((UserInfo)Session["model"]).Id;
                    see.SubTime = DateTime.Now;

                    MySeeServices.Add(see);
                }

            }
            #endregion

            #region 主要商品信息
            Goods good = GoodsServices.LoadEntities(u => u.Id == id && u.DelGlag == normal).FirstOrDefault();

            ViewData["Goods"] = good;

            var saleinfo = SaleInfoServices.LoadEntities(u => u.GoodsId == id).FirstOrDefault();
            ViewData["SaleInfo"] = saleinfo;
            #endregion
            int total = 0;

            var listAbout = GoodsServices.LoadPageEntities(1, 8, out total, u => u.UserInfoId == good.UserInfoId, u => u.SubTime, false).ToList();
            ViewData["AboutGoods"] = listAbout;

            ViewBag.Title = good.Title + "青职二货街";

            return View();
        }
        #endregion

        public ActionResult AddShouCang(int userId, int goodId)
        {
            var good = GoodsServices.LoadEntities(u => u.Id == goodId).FirstOrDefault();
            if (good == null)
            {
                return Content("数据有误");
            }

            Shoping shop = new Shoping();
            shop.GoodsId = good.Id;
            shop.UserInfoId = userId;
            shop.DelFlag = normal;
            shop.SubTime = DateTime.Now;

            try
            {
                ShopingServices.Add(shop);
                return Content("加入收藏成功");
            }
            catch (Exception)
            {
                return Content("错误，请重试");
            }
        }



        /*---------------------------下方为失物招领------------------------------------*/


        public IShiWuServices ShiWuServices { get; set; }

        public short find = (short)ShiWuType.Find;
        public short get = (short)ShiWuType.Get;

        public ActionResult Get()
        {
            int total = 0;
            var list = ShiWuServices.LoadPageEntities(1, 10, out total, u => u.Type == get, u => u.SubTime, false).ToList();

            ViewData["list"] = list;

            return View();
        }

        public ActionResult Find()
        {
            int total = 0;
            var list = ShiWuServices.LoadPageEntities(1, 10, out total, u => u.Type == find, u => u.SubTime, false).ToList();

            ViewData["list"] = list;

            return View();
        }
        public ActionResult GetMsg(string pageindex, string type)
        {

            short types = (short)(int.Parse(type));

            if (pageindex != null)
            {
                int index = int.Parse(pageindex);

                int total = 0;

                var list = ShiWuServices.LoadPageEntities(index, 10, out total, u => u.Type == types, u => u.SubTime, false).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Content("0");
            }
        }

        public ActionResult AddSales(int id)
        {
            if (id == find)
            {
                ViewData["title"] = "发布寻物信息";
                ViewData["Type"] = 0;
            }
            else
            {
                ViewData["title"] = "发布招领信息";
                ViewData["Type"] = 1;
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddSales(ShiWu sale)
        {
            sale.SubTime = DateTime.Now;

            try
            {
                ShiWuServices.Add(sale);
                return Redirect("/Home/Show/" + sale.Id);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
        public ActionResult Show(int id)
        {
            var sale = ShiWuServices.LoadEntities(u => u.Id == id).FirstOrDefault();

            if (id == find)
            {
                ViewData["title"] = "寻物信息";
            }
            else
            {
                ViewData["title"] = "招领信息";
            }

            ViewData["sale"] = sale;

            return View();
        }

    }
}
