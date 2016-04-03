using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using WLXK.SecondHand.Common;

namespace WLXK.SecondHand.UI.Portal.Common
{
    public static class EmailSendToAllAdmin
    {
        private static Queue<MyEmail> EmialSend = new Queue<MyEmail>();

        public static void SendEmail(MyEmail email)
        {
            lock (EmialSend)
            {
                EmialSend.Enqueue(email);
            }
        }

        static EmailSendToAllAdmin()
        {
            ThreadPool.QueueUserWorkItem(u =>
            {
                while (true)
                {
                    if (EmialSend == null)
                    {
                        continue;
                    }

                    if (EmialSend.Count() > 0)
                    {
                        MyEmail email = EmialSend.Dequeue();

                        EmailHelper.SendMail(email);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                }
            });
        }

    }
}