 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.Bll
{
   
	
    public partial class AdminUserServices:BaseServices<AdminUser>,IAdminUserServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.AdminUserDal;
        }
    }
 
	
    public partial class FriendLinkServices:BaseServices<FriendLink>,IFriendLinkServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.FriendLinkDal;
        }
    }
 
	
    public partial class GoodsServices:BaseServices<Goods>,IGoodsServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.GoodsDal;
        }
    }
 
	
    public partial class ListShopsServices:BaseServices<ListShops>,IListShopsServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.ListShopsDal;
        }
    }
 
	
    public partial class MyNewsServices:BaseServices<MyNews>,IMyNewsServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.MyNewsDal;
        }
    }
 
	
    public partial class MySeeServices:BaseServices<MySee>,IMySeeServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.MySeeDal;
        }
    }
 
	
    public partial class SaleInfoServices:BaseServices<SaleInfo>,ISaleInfoServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.SaleInfoDal;
        }
    }
 
	
    public partial class ShiWuServices:BaseServices<ShiWu>,IShiWuServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.ShiWuDal;
        }
    }
 
	
    public partial class ShopingServices:BaseServices<Shoping>,IShopingServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.ShopingDal;
        }
    }
 
	
    public partial class SuggestServices:BaseServices<Suggest>,ISuggestServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.SuggestDal;
        }
    }
 
	
    public partial class UserInfoServices:BaseServices<UserInfo>,IUserInfoServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.UserInfoDal;
        }
    }
 
	
    public partial class UserInfoExtServices:BaseServices<UserInfoExt>,IUserInfoExtServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.UserInfoExtDal;
        }
    }
 
	
}