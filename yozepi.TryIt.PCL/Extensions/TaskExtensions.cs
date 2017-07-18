using yozepi.Retry.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry;

namespace yozepi.Retry
{
    public static class TaskExtensions
    {
        public static TaskRetryBuilder TryAsync(this Func<Task> task, int retries)
        {
            return TryIt.TryAsync(task, retries);
        }

  
        public static TaskRetryBuilder<T> TryAsync<T>(this Func<Task<T>> action, int retries)
        {
            return TryIt.TryAsync<T>(action, retries);
        }

    }
}
