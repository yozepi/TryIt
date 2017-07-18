using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Builders
{
    public class TaskRetryBuilder : BaseAsyncBuilder
    {
        public Task Go()
        {
            return Go(CancellationToken.None);
        }


        public Task Go(CancellationToken cancellationToken)
        {
            return RunAsync(cancellationToken);
        }
    }
}
