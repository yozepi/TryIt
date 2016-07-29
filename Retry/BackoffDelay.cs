using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class BackoffDelay : Delay
    {
        public BackoffDelay(TimeSpan initialDelay)
        {
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("Value must be greater than a zero TimeSpan", initialDelay, "initialDelay");

            InitialDelay = initialDelay;
        }
        public TimeSpan InitialDelay { get; private set; }
        public override async Task WaitAsync(int tryCount)
        {
            var ticks = (long)(InitialDelay.Ticks * Math.Pow(2, (tryCount - 1)));
            await WaitAsync(TimeSpan.FromTicks(ticks));

        }
    }
}
