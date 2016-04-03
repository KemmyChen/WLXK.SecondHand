using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SecondHand.DalFactory;
using WLXK.SecondHand.IDal;

namespace WLXK.SecondHand.Bll
{
    public abstract class BaseServices<T> where T : class,new()
    {
        public IDbSession dbSession = DbSessionFactory.GetCurrentDbSession();

        public IBaseDal<T> CurrentDal;

        public BaseServices()
        {
            SetCurrentDal();
        }

        public abstract void SetCurrentDal();

        public int SaveChanges()
        {
            return dbSession.SaveChanges();
        }

        public virtual T Add(T model)
        {
            CurrentDal.Add(model);
            dbSession.SaveChanges();
            return model;
        }
        public virtual bool Delete(T model)
        {
            CurrentDal.Delete(model);
            return dbSession.SaveChanges()>0;
        }

        public virtual int Delete(params int[] ids)
        {
            CurrentDal.Delete(ids);
            return dbSession.SaveChanges();
        }

        public virtual IQueryable<T> LoadEntities(Func<T, bool> whereLambda)
        {
            return CurrentDal.LoadEntities(whereLambda);
        }

        public virtual IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int total, Func<T, bool> whereLambda, Func<T, S> orderByLambda, bool isAsc)
        {
            return CurrentDal.LoadPageEntities(pageIndex, pageSize, out total, whereLambda, orderByLambda, isAsc);
        }

        public virtual bool Update(T model)
        {
            CurrentDal.Update(model);
            return dbSession.SaveChanges()>0;
        }


    }
}
