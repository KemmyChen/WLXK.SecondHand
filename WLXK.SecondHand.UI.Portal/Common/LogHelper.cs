using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WLXK.SecondHand.UI.Portal.Common
{
    public static class LogHelper
    {
        public static Queue<string> ErrorMsg = new Queue<string>();
        public static void WriteLog(string msg)
        {
            lock (ErrorMsg)
            {
                ErrorMsg.Enqueue(msg);
            }
        }

        static LogHelper()
        {
            ThreadPool.QueueUserWorkItem(u =>
            {
                while (true)
                {
                    if (ErrorMsg == null)
                    {
                        continue;
                    }

                    string str = string.Empty;
                    lock (ErrorMsg)
                    {
                        if (ErrorMsg.Count() > 0)
                        {
                            str = ErrorMsg.Dequeue();
                        }
                    }

                    if (!string.IsNullOrEmpty(str))
                    {
                        //写日志文件
                        ILog log = log4net.LogManager.GetLogger("test");
                        log.Error(str);
                    }
                    if (ErrorMsg.Count() < 0)
                    {
                        Thread.Sleep(100);
                    } 
                }
            });
        }
    }
}
