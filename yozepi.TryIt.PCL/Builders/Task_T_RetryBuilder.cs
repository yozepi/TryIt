using yozepi.Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Builders
{
    public class TaskRetryBuilder<T>: BaseAsyncBuilder
    {
        public Task<T> Go()
        {
            return Go(CancellationToken.None);
        }


        public async Task<T> Go(CancellationToken cancellationToken)
        {
            await RunAsync(cancellationToken);
            return (Winner as TaskRunner<T>).Result;
        }
    }
}
