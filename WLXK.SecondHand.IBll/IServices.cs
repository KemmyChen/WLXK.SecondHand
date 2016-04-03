 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.IBll
{
   
	
    public partial interface IAdminUserServices:IBaseServices<AdminUser>
    {
    }
 
	
    public partial interface IFriendLinkServices:IBaseServices<FriendLink>
    {
    }
 
	
    public partial interface IGoodsServices:IBaseServices<Goods>
    {
    }
 
	
    public partial interface IListShopsServices:IBaseServices<ListShops>
    {
    }
 
	
    public partial interface IMyNewsServices:IBaseServices<MyNews>
    {
    }
 
	
    public partial interface IMySeeServices:IBaseServices<MySee>
    {
    }
 
	
    public partial interface ISaleInfoServices:IBaseServices<SaleInfo>
    {
    }
 
	
    public partial interface IShiWuServices:IBaseServices<ShiWu>
    {
    }
 
	
    public partial interface IShopingServices:IBaseServices<Shoping>
    {
    }
 
	
    public partial interface ISuggestServices:IBaseServices<Suggest>
    {
    }
 
	
    public partial interface IUserInfoServices:IBaseServices<UserInfo>
    {
    }
 
	
    public partial interface IUserInfoExtServices:IBaseServices<UserInfoExt>
    {
    }
 
	
}