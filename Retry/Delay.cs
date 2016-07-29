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

        private static IDelay _defaultDelay;
        public static IDelay DefaultDelay
        {
            get
            {
                if (_defaultDelay == null)
                    _defaultDelay = new NoDelay();

                return _defaultDelay;
            }
            set { _defaultDelay = value; }
        }

        #region static methods:

        public static IDelay Default()
        {
            return DefaultDelay;
        }

        public static IDelay NoDelay()
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
