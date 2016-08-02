using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class TimedDelay : Delay
    {
        public TimedDelay(TimeSpan pauseTime)
        {
            if (pauseTime < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("Value must be greater than a zero TimeSpan", pauseTime, "pauseTime");

            PauseTime = pauseTime;
        }
        public TimeSpan PauseTime { get; private set; }
        protected override async Task WaitAsync(int tryCount)
        {
            await WaitAsync(PauseTime);
        }
    }
}
