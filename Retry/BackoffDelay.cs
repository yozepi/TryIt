using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    /// <summary>
    /// Implements the <see cref="IDelay"/> interface to create a delay that "backs off" with each attempt at retry.
    /// </summary>
    /// <remarks>
    /// With each attempt, BackoffDelay will double the previous delay time.</remarks>
    public class BackoffDelay : Delay
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="initialDelay">A <see cref="TimeSpan"/> representing the initial delay to begin with.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided TimeSpan is less than zero.</exception>
        public BackoffDelay(TimeSpan initialDelay)
        {
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("Value must be greater than a zero TimeSpan", initialDelay, "initialDelay");

            InitialDelay = initialDelay;
        }

        /// <summary>
        /// The <see cref="TimeSpan"/> of the initial delay.
        /// </summary>
        public TimeSpan InitialDelay { get; private set; }

#pragma warning disable 1591
        protected override async Task WaitAsync(int tryCount)
        {
            var ticks = (long)(InitialDelay.Ticks * Math.Pow(2, (tryCount - 1)));
            await WaitAsync(TimeSpan.FromTicks(ticks));

        }
#pragma warning restore 1591
    }
}
