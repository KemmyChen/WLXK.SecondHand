using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using WLXK.SecondHand.Model;

namespace WLXK.SecondHand.Dal
{
    public static class EFContextFactory
    {
        public static DbContext GetCurrentEFContext()
        {
            DbContext db = (DbContext)CallContext.GetData("EFContext");
            if (db == null)
            {
                db = new qtcSecondHandEntities();
                CallContext.SetData("EFContext", db);
            }
            return db;
        }
    }
}
