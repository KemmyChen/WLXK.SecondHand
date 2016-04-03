using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WLXK.SecondHand.IDal
{
    public interface IBaseDal<T> where T:class,new()    
    {
        T Add(T model);

        bool Delete(T model);

        int Delete(params int[] ids);

        bool Update(T model);

        IQueryable<T> LoadEntities(Func<T, bool> whereLambda);

        IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int total, Func<T, bool> whereLambda, Func<T, S> orderByLambda, bool isAsc);
    }
}
