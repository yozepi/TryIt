using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry;
using yozepi.Retry.Builders;

namespace yozepi.Retry
{
    public static class ActionExtensions
    {
        public static ActionRetryBuilder Try(this Action action, int retries)
        {
            return TryIt.Try(action, retries);
        }

        public static TaskRetryBuilder TryAsync(this Action action, int retries)
        {
            return TryIt.TryAsync(action, retries);
        }
    }
}
