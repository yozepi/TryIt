using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Delays
{
    /// <summary>
    /// Provides a base class for creating custom delay classes.
    /// </summary>
    public abstract class Delay : IDelay
    {

        private static Delay _defaultDelay;

        /// <summary>
        /// Gets/sets the default delay to use when no delay is provided to the <see cref="TryIt"/> Try methods. 
        /// When not set, no delay will be used.
        /// </summary>
        public static Delay DefaultDelay
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

        /// <summary>
        /// Returns an instance of the <see cref="NoDelay"/> class.
        /// </summary>
        /// <returns></returns>
        public static Delay NoDelay()
        {
            return new NoDelay();
        }

        /// <summary>
        /// Returns an instance of the <see cref="BasicDelay"/> class.
        /// </summary>
        /// <param name="pauseTime">A <see cref="TimeSpan"/> representing the delay time.</param>
        /// <returns></returns>
        public static Delay Basic(TimeSpan pauseTime)
        {
            return new BasicDelay(pauseTime);
        }

        /// <summary>
        /// Returns an instance of the <see cref="BackoffDelay" /> class.
        /// </summary>
        /// <param name="startTime">A <see cref="TimeSpan"/> representing the first delay of the backoff.</param>
        /// <returns></returns>
        public static Delay Backoff(TimeSpan startTime)
        {
            return new BackoffDelay(startTime);
        }

        /// <summary>
        /// Returns an instance of the <see cref="FibonacciDelay" /> class.
        /// </summary>
        /// <param name="startTime">A <see cref="TimeSpan"/> representing the first delay of the backoff.</param>
        /// <returns></returns>
        public static Delay Fibonacci(TimeSpan startTime)
        {
            return new FibonacciDelay(startTime);
        }

        #endregion //static methods:


        /// <summary>
        /// Implementors override to provide a <see cref="TimeSpan"/> for delaying
        /// </summary>
        /// <param name="tryCount"></param>
        /// <returns></returns>
        protected abstract Task WaitAsync(int tryCount);


        /// <summary>
        /// This helper method enables implementors to easily create Wait Time <see cref="Task"/>'s that will work correctly with the TryIt infrastructure.
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        protected virtual async Task WaitAsync(TimeSpan waitTime)
        {
            await Task.Delay(waitTime);
        }

        Task IDelay.WaitAsync(int tryCount)
        {
            return this.WaitAsync(tryCount);
        }
    }
}
