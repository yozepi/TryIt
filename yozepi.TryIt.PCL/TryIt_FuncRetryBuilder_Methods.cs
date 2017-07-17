using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry.Builders;
using yozepi.Retry.Delays;
using yozepi.Retry.Runners;

namespace yozepi.Retry
{
    public static partial class TryIt
    {

        public static FuncRetryBuilder<T> Try<T>(Func<T> func, int retries) 
        {
            return new FuncRetryBuilder<T>()
                .AddRunner(new FuncRunner<T>())
                .SetActor(func)
                .SetRetryCount(retries) as FuncRetryBuilder<T>;
        }


        public static FuncRetryBuilder<T> ThenTry<T>(this FuncRetryBuilder<T> builder, int retries)
        {
            BaseRunner runner = new FuncRunner<T>();

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<T>;
        }


        public static FuncRetryBuilder<T> ThenTry<T>(this FuncRetryBuilder<T> builder, Func<T> func, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<T>())
                .SetActor(func)
                .SetRetryCount(retries) as FuncRetryBuilder<T>;
        }


        #region UsingDelay, WithErrorPolicy, WithSuccessPolicy

        public static FuncRetryBuilder<T> UsingDelay<T>(this FuncRetryBuilder<T> builder, IDelay delay)
        {
            return builder
                .SetDelay(delay) as FuncRetryBuilder<T>;

        }

        public static FuncRetryBuilder<T> WithErrorPolicy<T>(this FuncRetryBuilder<T> builder, ErrorPolicyDelegate errorPolicy)
        {
            return builder
                .SetErrorPolicy(errorPolicy) as FuncRetryBuilder<T>;
        }

        public static FuncRetryBuilder<T> WithSuccessPolicy<T>(this FuncRetryBuilder<T> builder, SuccessPolicyDelegate<T> successPolicy)
        {
            return builder
                .SetSuccessPolicy(successPolicy) as FuncRetryBuilder<T>;
        }

        #endregion //UsingDelay, WithErrorPolicy, WithSuccessPolicy

    }
}
