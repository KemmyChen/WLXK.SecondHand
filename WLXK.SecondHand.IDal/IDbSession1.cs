 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.Dal;

namespace WLXK.SecondHand.IDal
{
   public partial interface IDbSession
    {
	
		IAdminUserDal  AdminUserDal{get;}
	
		IFriendLinkDal  FriendLinkDal{get;}
	
		IGoodsDal  GoodsDal{get;}
	
		IListShopsDal  ListShopsDal{get;}
	
		IMyNewsDal  MyNewsDal{get;}
	
		IMySeeDal  MySeeDal{get;}
	
		ISaleInfoDal  SaleInfoDal{get;}
	
		IShiWuDal  ShiWuDal{get;}
	
		IShopingDal  ShopingDal{get;}
	
		ISuggestDal  SuggestDal{get;}
	
		IUserInfoDal  UserInfoDal{get;}
	
		IUserInfoExtDal  UserInfoExtDal{get;}
		int SaveChanges();
	}
	
}