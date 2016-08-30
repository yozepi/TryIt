using Retry.Builders;
using Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    partial class TryIt
    {


        #region Try methods:

        public static FuncRetryBuilder<TResult> Try<TResult>(Func<Task<TResult>> func, int retries)
        {
            return new FuncRetryBuilder<TResult>()
            .AddRunner(new TaskWithResultRunner<TResult>())
            .SetActor(func)
            .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }


        public static FuncRetryBuilder<TResult> Try<T, TResult>(Func<T, Task<TResult>> func, T arg, int retries)
        {
            return new FuncRetryBuilder<TResult>()
            .AddRunner(new TaskWithResultRunner<T, TResult>(arg))
            .SetActor(func)
            .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> Try<T1, T2, TResult>(Func<T1, T2, Task<TResult>> func, T1 arg1, T2 arg2, int retries)
        {
            return new FuncRetryBuilder<TResult>()
            .AddRunner(new TaskWithResultRunner<T1, T2, TResult>(arg1, arg2))
            .SetActor(func)
            .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, TResult>(Func<T1, T2, T3, Task<TResult>> func, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return new FuncRetryBuilder<TResult>()
            .AddRunner(new TaskWithResultRunner<T1, T2, T3, TResult>(arg1, arg2, arg3))
            .SetActor(func)
            .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, Task<TResult>> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return new FuncRetryBuilder<TResult>()
            .AddRunner(new TaskWithResultRunner<T1, T2, T3, T4, TResult>(arg1, arg2, arg3, arg4))
            .SetActor(func)
            .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        #endregion //Try methods:
    }
}
