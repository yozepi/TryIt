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



        public static FuncRetryBuilder<TResult> Try<TResult>(Func<TResult> func, int retries)
        {
            return new FuncRetryBuilder<TResult>()
                .AddRunner(new FuncRunner<TResult>())
                .SetActor(func)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }


        public static FuncRetryBuilder<TResult> ThenTry<TResult>(this FuncRetryBuilder<TResult> builder, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner);

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }


        public static FuncRetryBuilder<TResult> ThenTry<TResult>(this FuncRetryBuilder<TResult> builder, Func<TResult> altFunc, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<TResult>())
                .SetActor(altFunc)
                .SetRetryCount(retries) as FuncRetryBuilder<TResult>;
        }

    }
}
