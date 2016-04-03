using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.Bll
{
    public partial class GoodsServices : BaseServices<Goods>, IGoodsServices
    {
        public override Goods Add(Goods model)
        {
            CurrentDal.Add(model);
            return model;
        }

        public int Add(UserInfo user, Goods good, SaleInfo sale)
        {

            Add(good);

            SaleInfoServices saleBll = new SaleInfoServices();
            saleBll.Add(sale);

            good.UserInfoId = user.Id;

            sale.Goods = good;

            return SaveChanges();
        }

        public int Update(UserInfo user, Goods good, SaleInfo sale)
        {
            good.UserInfoId = user.Id;
            
            Update(good);

            sale.GoodsId = good.Id;

            SaleInfoServices saleBll = new SaleInfoServices();
            saleBll.Update(sale);

            return SaveChanges();
        }


    }
}
