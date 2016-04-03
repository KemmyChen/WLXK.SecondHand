 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.IDal;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.Dal
{
   
	
    public partial class AdminUserDal : BaseDal<AdminUser>, IAdminUserDal
    {

    }
 
	
    public partial class FriendLinkDal : BaseDal<FriendLink>, IFriendLinkDal
    {

    }
 
	
    public partial class GoodsDal : BaseDal<Goods>, IGoodsDal
    {

    }
 
	
    public partial class ListShopsDal : BaseDal<ListShops>, IListShopsDal
    {

    }
 
	
    public partial class MyNewsDal : BaseDal<MyNews>, IMyNewsDal
    {

    }
 
	
    public partial class MySeeDal : BaseDal<MySee>, IMySeeDal
    {

    }
 
	
    public partial class SaleInfoDal : BaseDal<SaleInfo>, ISaleInfoDal
    {

    }
 
	
    public partial class ShiWuDal : BaseDal<ShiWu>, IShiWuDal
    {

    }
 
	
    public partial class ShopingDal : BaseDal<Shoping>, IShopingDal
    {

    }
 
	
    public partial class SuggestDal : BaseDal<Suggest>, ISuggestDal
    {

    }
 
	
    public partial class UserInfoDal : BaseDal<UserInfo>, IUserInfoDal
    {

    }
 
	
    public partial class UserInfoExtDal : BaseDal<UserInfoExt>, IUserInfoExtDal
    {

    }
 
	
}