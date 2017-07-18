using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry;
using yozepi.Retry.Builders;

namespace yozepi.Retry
{
    public static class FuncExcensions
    {

        public static MethodRetryBuilder<T> Try<T>(this Func<T> func, int retries)
        {
            return TryIt.Try(func, retries);
        }

        public static TaskRetryBuilder<T> TryAsync<T>(this Func<T> func, int retries)
        {
            return TryIt.TryAsync(func, retries);
        }
    }
}
