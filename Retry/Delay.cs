using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry
{
    public abstract class Delay : IDelay
    {

        static Delay()
        {
            DefaultDelay = NoPause();
        }

        static IDelay DefaultDelay { get; set; }

        #region static methods:

        public static IDelay Default()
        {
            var delay = DefaultDelay;
            if (delay == null)
                delay = NoPause();
            return delay;
        }

        public static IDelay NoPause()
        {
            return new NoDelay();
        }

        public static IDelay Timed(TimeSpan pauseTime)
        {
            return new TimedDelay(pauseTime);
        }

        public static IDelay Backoff(TimeSpan startTime)
        {
            return new BackoffDelay(startTime);
        }

        #endregion //static methods:


        public abstract Task WaitAsync(int tryCount);

        protected virtual async Task WaitAsync(TimeSpan waitTime)
        {
            await Task.Delay(waitTime);
        }
    }
}
