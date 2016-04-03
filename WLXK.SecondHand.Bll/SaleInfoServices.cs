using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.IBll;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.Bll
{
    public partial class SaleInfoServices : BaseServices<SaleInfo>, ISaleInfoServices
    {
        public override SaleInfo Add(SaleInfo model)
        {
            CurrentDal.Add(model);
            return model;
        }
    }
}
