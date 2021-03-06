﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yozepi.Retry.Delays
{
    /// <summary>
    /// Implements the <see cref="IDelay"/> interface to create a delay that pauses for the duration of the provided TimeSpan.
    /// </summary>
    public class BasicDelay : Delay
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="pauseTime">A <see cref="TimeSpan"/> representing the length of time to delay.</param>
        public BasicDelay(TimeSpan pauseTime)
        {
            if (pauseTime < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("Value must be greater than a zero TimeSpan", pauseTime, "pauseTime");

            PauseTime = pauseTime;
        }

        /// <summary>
        /// The <see cref="TimeSpan"/> of the delay.
        /// </summary>
        public TimeSpan PauseTime { get; private set; }

#pragma warning disable 1591
        protected override async Task WaitAsync(int tryCount)
        {
            await WaitAsync(PauseTime);
        }
#pragma warning restore 1591
    }
}
