﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    /// <summary>
    /// An instance of Delay that returns without delaying.
    /// </summary>
    public class NoDelay : Delay
    {
#pragma warning disable 1591
        public NoDelay()
            : base() { }

        protected override async Task WaitAsync(int tryCount)
        {
            await Task.Factory.StartNew(() => { });
        }
#pragma warning restore 1591
    }
}
