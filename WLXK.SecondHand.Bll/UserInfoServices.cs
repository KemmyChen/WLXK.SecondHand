using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.Bll
{
    public partial class UserInfoServices : BaseServices<UserInfo>, IUserInfoServices
    {
        //public override void SetCurrentDal()
        //{
        //    CurrentDal = dbSession.UserInfoDal;
        //}
    }
}
