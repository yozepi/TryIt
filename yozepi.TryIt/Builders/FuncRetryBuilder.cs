using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry.Runners;
using System.Threading;

namespace Retry.Builders
{
    public class FuncRetryBuilder<TResult> : BaseBuilder
    {

        public TResult Go()
        {
            Run();
            return (Winner as FuncRunner<TResult>).Result;
        }

        public Task<TResult> GoAsync()
        {
            return GoAsync(CancellationToken.None);
        }

        public async Task<TResult> GoAsync(CancellationToken cancellationToken)
        {
            await RunAsync(cancellationToken);
            return (Winner as FuncRunner<TResult>).Result;
        }
    }
}
