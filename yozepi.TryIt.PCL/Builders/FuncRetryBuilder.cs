using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry.Runners;
using System.Threading;

namespace yozepi.Retry.Builders
{
    public class FuncRetryBuilder<TResult> : BaseSyncBuilder
    {

        public TResult Go()
        {
            return Go(CancellationToken.None);
        }

        public TResult Go(CancellationToken cancellationToken)
        {
            Run(cancellationToken);
            return (Winner as FuncRunner<TResult>).Result;
        }
    }
}
