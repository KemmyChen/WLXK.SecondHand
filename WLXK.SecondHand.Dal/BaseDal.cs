using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.Dal
{
    public class BaseDal<T> where T:class,new()
    {
        DbContext dbContext = EFContextFactory.GetCurrentEFContext();
        public T Add(T model)
        {
            dbContext.Set<T>().Add(model);
            //dbContext.SaveChanges();
            return model;
        }

        public bool Delete(T model)
        {
            dbContext.Entry(model).State = System.Data.EntityState.Deleted;
            return true;
        }

        public int Delete(params int[] ids)
        {
            for (int i = 0; i < ids.Count(); i++)
            {
                T model = dbContext.Set<T>().Find(ids[i]);
                dbContext.Entry(model).State = System.Data.EntityState.Deleted;
            }
            return ids.Count();
        }

        public IQueryable<T> LoadEntities(Func<T, bool> whereLambda)
        {
            return dbContext.Set<T>().Where(whereLambda).AsQueryable();
        }

        public IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int total, Func<T, bool> whereLambda, Func<T, S> orderByLambda, bool isAsc)
        {
            total = dbContext.Set<T>().Where(whereLambda).Count();
            if (isAsc)
            {
                return dbContext.Set<T>().Where(whereLambda).OrderBy(orderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable();
            }
            else
            {
                return dbContext.Set<T>().Where(whereLambda).OrderByDescending(orderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable();
            }
        }



        public bool Update(T model)
        {
            dbContext.Entry(model).State = System.Data.EntityState.Modified;
            return true;
        }
    }
}
