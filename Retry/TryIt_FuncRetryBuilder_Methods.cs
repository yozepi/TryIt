using Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry.Builders;

namespace Retry
{
    partial class TryIt
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

        //        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        //        {
        //            return new FuncTryIt<T1, T2, T3, T4, T5, TResult>(retries, arg1, arg2, arg3, arg4, arg5, func);
        //        }

        //        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        //        {
        //            return new FuncTryIt<T1, T2, T3, T4, T5, T6, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, func);
        //        }

        //        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        //        {
        //            return new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, func);
        //        }

        //        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        //        {
        //            return new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, func);
        //        }

        //        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        //        {
        //            return new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, func);
        //        }

        #endregion //Try methods:


        #region UsingDelay, WithErrorPolicy, WithSuccessPolicy

        public static FuncRetryBuilder<TResult> UsingDelay<TResult>(this FuncRetryBuilder<TResult> builder, IDelay delay)
        {
            return builder
                .SetDelay(delay) as FuncRetryBuilder<TResult>;

        }

        public static FuncRetryBuilder<TResult> WithErrorPolicy<TResult>(this FuncRetryBuilder<TResult> builder, ErrorPolicyDelegate errorPolicy)
        {
            return builder
                .SetErrorPolicy(errorPolicy) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> WithSuccessPolicy<TResult>(this FuncRetryBuilder<TResult> builder, SuccessPolicyDelegate<TResult> successPolicy)
        {
            return builder
                .SetSuccessPolicy(successPolicy) as FuncRetryBuilder<TResult>;
        }

        #endregion //UsingDelay, WithErrorPolicy, WithSuccessPolicy


        #region ThenTry extensions:

        public static FuncRetryBuilder<TResult> ThenTry<TResult>(this FuncRetryBuilder<TResult> builder, int retries)
        {
            BaseRunner runner =
                  builder.LastRunner.GetType() == typeof(TaskWithResultRunner<TResult>)
                  ? new TaskWithResultRunner<TResult>()
                  : new FuncRunner<TResult>() as BaseRunner;

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
         }

        public static FuncRetryBuilder<TResult> ThenTry<T, TResult>(this FuncRetryBuilder<TResult> builder, T arg, int retries)
        {
            BaseRunner runner =
                builder.LastRunner.GetType() == typeof(TaskWithResultRunner<T, TResult>)
                ? new TaskWithResultRunner<T, TResult>(arg)
                : new FuncRunner<T, TResult>(arg) as BaseRunner;

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, TResult>(this FuncRetryBuilder<TResult> builder, T1 arg1, T2 arg2, int retries)
        {
            BaseRunner runner =
                builder.LastRunner.GetType() == typeof(TaskWithResultRunner<T1, T2, TResult>)
                ? new TaskWithResultRunner<T1, T2, TResult>(arg1, arg2)
                : new FuncRunner<T1, T2, TResult>(arg1, arg2) as BaseRunner;

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, T3, TResult>(this FuncRetryBuilder<TResult> builder, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            BaseRunner runner =
                builder.LastRunner.GetType() == typeof(TaskWithResultRunner<T1, T2, T3, TResult>)
                ? new TaskWithResultRunner<T1, T2, T3, TResult>(arg1, arg2, arg3)
                : new FuncRunner<T1, T2, T3, TResult>(arg1, arg2, arg3) as BaseRunner;

            return builder
                 .AddRunner(runner)
                 .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

        public static FuncRetryBuilder<TResult> ThenTry<T1, T2, T3, T4, TResult>(this FuncRetryBuilder<TResult> builder, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            BaseRunner runner =
                  builder.LastRunner.GetType() == typeof(TaskWithResultRunner<T1, T2, T3, T4, TResult>)
                  ? new TaskWithResultRunner<T1, T2, T3, T4, TResult>(arg1, arg2, arg3, arg4)
                  : new FuncRunner<T1, T2, T3, T4, TResult>(arg1, arg2, arg3, arg4) as BaseRunner;

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
