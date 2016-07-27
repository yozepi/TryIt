using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class BackoffDelay : Delay
    {
        public BackoffDelay(TimeSpan startTime)
        {
            if (startTime < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("Value must be greater than a zero TimeSpan", startTime, "pauseTime");

            StartTime = startTime;
        }
        public TimeSpan StartTime { get; private set; }
        public override async Task WaitAsync(int tryCount)
        {
            var ticks = (long)(StartTime.Ticks * Math.Pow(2, (tryCount - 1)));
            await WaitAsync(TimeSpan.FromTicks(ticks));

        }
    }
}
