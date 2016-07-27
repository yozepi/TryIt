using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class NoDelay : Delay
    {
        public NoDelay()
            : base() { }

        public override async Task WaitAsync(int tryCount)
        {
            return;
        }
    }
}
