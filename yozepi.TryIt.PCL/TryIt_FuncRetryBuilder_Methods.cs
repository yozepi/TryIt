using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry;
using Retry.Builders;
using Retry.Delays;
using Retry.Runners;

namespace Retry
{
    public static partial class TryIt
    {


        #region Try methods:

        public static FuncRetryBuilder<TResult> Try<TResult>(Func<TResult> func, int retries)
        {
            return new FuncRetryBuilder<TResult>()
                .AddRunner(new FuncRunner<TResult>())
                .SetActor(func)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> Try<T, TResult>(Func<T, TResult> func, T arg, int retries)
        {
            return new FuncRetryBuilder<TResult>()
                .AddRunner(new FuncRunner<T, TResult>(arg))
                .SetActor(func)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> Try<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int retries)
        {
            return new FuncRetryBuilder<TResult>()
                   .AddRunner(new FuncRunner<T1, T2, TResult>(arg1, arg2))
                   .SetActor(func)
                   .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return new FuncRetryBuilder<TResult>()
                       .AddRunner(new FuncRunner<T1, T2, T3, TResult>(arg1, arg2, arg3))
                       .SetActor(func)
                       .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return new FuncRetryBuilder<TResult>()
                         .AddRunner(new FuncRunner<T1, T2, T3, T4, TResult>(arg1, arg2, arg3, arg4))
                         .SetActor(func)
                         .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        #endregion //Try methods:


        #region ThenTry extensions:

        public static FuncRetryBuilder<TResult> ThenTry<TResult>(this FuncRetryBuilder<TResult> builder, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner);

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
         }

        public static FuncRetryBuilder<TResult> ThenTry<T, TResult>(this FuncRetryBuilder<TResult> builder, T arg, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner, arg);

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, TResult>(this FuncRetryBuilder<TResult> builder, T1 arg1, T2 arg2, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner, arg1, arg2);

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, T3, TResult>(this FuncRetryBuilder<TResult> builder, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner, arg1, arg2, arg3);

            return builder
                 .AddRunner(runner)
                 .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, T3, T4, TResult>(this FuncRetryBuilder<TResult> builder, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner, arg1, arg2, arg3, arg4);

            return builder
                 .AddRunner(runner)
                 .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        #endregion //ThenTry extensions:


        #region ThenTry using Alt Func extensions:

        public static FuncRetryBuilder<TResult> ThenTry<TResult>(this FuncRetryBuilder<TResult> builder, Func<TResult> altFunc, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<TResult>())
                .SetActor(altFunc)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T, TResult>(this FuncRetryBuilder<TResult> builder, Func<T, TResult> altFunc, T arg, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<T, TResult>(arg))
                .SetActor(altFunc)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, TResult>(this FuncRetryBuilder<TResult> builder, Func<T1, T2, TResult> altFunc, T1 arg1, T2 arg2, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<T1, T2, TResult>(arg1, arg2))
                .SetActor(altFunc)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, T3, TResult>(this FuncRetryBuilder<TResult> builder, Func<T1, T2, T3, TResult> altFunc, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<T1, T2, T3, TResult>(arg1, arg2, arg3))
                .SetActor(altFunc)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, T3, T4, TResult>(this FuncRetryBuilder<TResult> builder, Func<T1, T2, T3, T4, TResult> altFunc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<T1, T2, T3, T4, TResult>(arg1, arg2, arg3, arg4))
                .SetActor(altFunc)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        #endregion //ThenTry using Alt Func extensions:

    }
}
