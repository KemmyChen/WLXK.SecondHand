 

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using WLXK.SecondHand.Dal;
using WLXK.SecondHand.IDal;

namespace WLXK.SecondHand.IDal
{
    public partial class DbSession:IDbSession
    {
	
		public IAdminUserDal AdminUserDal
        {
            get
            {
                return new AdminUserDal();
            }
        }
	
		public IFriendLinkDal FriendLinkDal
        {
            get
            {
                return new FriendLinkDal();
            }
        }
	
		public IGoodsDal GoodsDal
        {
            get
            {
                return new GoodsDal();
            }
        }
	
		public IListShopsDal ListShopsDal
        {
            get
            {
                return new ListShopsDal();
            }
        }
	
		public IMyNewsDal MyNewsDal
        {
            get
            {
                return new MyNewsDal();
            }
        }
	
		public IMySeeDal MySeeDal
        {
            get
            {
                return new MySeeDal();
            }
        }
	
		public ISaleInfoDal SaleInfoDal
        {
            get
            {
                return new SaleInfoDal();
            }
        }
	
		public IShiWuDal ShiWuDal
        {
            get
            {
                return new ShiWuDal();
            }
        }
	
		public IShopingDal ShopingDal
        {
            get
            {
                return new ShopingDal();
            }
        }
	
		public ISuggestDal SuggestDal
        {
            get
            {
                return new SuggestDal();
            }
        }
	
		public IUserInfoDal UserInfoDal
        {
            get
            {
                return new UserInfoDal();
            }
        }
	
		public IUserInfoExtDal UserInfoExtDal
        {
            get
            {
                return new UserInfoExtDal();
            }
        }
		public int SaveChanges()
        {
            DbContext db = EFContextFactory.GetCurrentEFContext();
            return db.SaveChanges();
        }
	}
	
}