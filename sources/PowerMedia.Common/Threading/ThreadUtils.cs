using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PowerMedia.Common.Threading
{
    public static class ThreadUtils
    {

        /// <summary>
        /// wait for end of execution or timeout
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="method"></param>
        /// <returns>true if method executed in less than timeout</returns>
        public static bool ExecuteMethodWithTimeoutSync(int millisecondsTimeout, Action method)
        {
            AutoResetEvent timeoutEvent = new AutoResetEvent(false);
            var executed = false;
            Thread methodInvoker = new Thread(delegate()
            {
                method();
                timeoutEvent.Set();
                executed = true;
            });

            timeoutEvent.Reset();
            methodInvoker.Start();

            if (timeoutEvent.WaitOne(millisecondsTimeout, false) == false)
            {
                methodInvoker.Abort();
            }
            else
            {
                methodInvoker.Join();
            }
            return executed;
        }


        /// <summary>
        /// fire and forget
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="method"></param>
        public static void ExecuteMethodWithTimeout(TimeSpan timeout, Action method)
        {
            ExecuteMethodWithTimeout((int)timeout.TotalMilliseconds, method);
        }
        
        /// <summary>
        /// fire and forget
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="method"></param>
        public static void ExecuteMethodWithTimeout(int millisecondsTimeout, Action method)
        {
            AutoResetEvent timeoutEvent = new AutoResetEvent(false);
            Thread methodInvoker = new Thread(delegate()
            {
                method();
                timeoutEvent.Set();
            });

            timeoutEvent.Reset();
            methodInvoker.Start();

            if (timeoutEvent.WaitOne(millisecondsTimeout, false) == false)
            {
                methodInvoker.Abort();
                throw new TimeoutException();
            }
        }
    }
}
