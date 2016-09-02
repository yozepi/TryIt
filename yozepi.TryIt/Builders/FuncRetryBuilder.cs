using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry.Runners;

namespace Retry.Builders
{
    public class FuncRetryBuilder<TResult> : BaseBuilder
    {

        public TResult Go()
        {
            Run();
            return (Winner as FuncRunner<TResult>).Result;
        }

        public async Task<TResult> GoAsync()
        {
            await RunAsync();
            return (Winner as FuncRunner<TResult>).Result;
        }
    }
}
