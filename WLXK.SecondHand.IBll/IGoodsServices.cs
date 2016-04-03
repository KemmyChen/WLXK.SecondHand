using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.IBll
{
    public partial interface IGoodsServices:IBaseServices<Goods>
    {
        int Add(UserInfo user, Goods good, SaleInfo sale);
        int Update(UserInfo user, Goods good, SaleInfo sale);
    }
}
