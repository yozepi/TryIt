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


        public static TaskRetryBuilder<T> TryAsync<T>(Func<Task<T>> task, int retries)
        {
            return new TaskRetryBuilder<T>()
            .AddRunner(new TaskRunner<T>())
            .SetActor(task)
            .SetRetryCount(retries) as TaskRetryBuilder<T>;
        }

        public static TaskRetryBuilder<T> TryAsync<T>(Func<T> func, int retries)
        {
            var ft = new Func<Task<T>>(() => new Task<T>(func));
            return TryAsync(ft, retries);
        }


        #region ThenTry Task<T> extensions:

        public static TaskRetryBuilder<T> ThenTry<T>(this TaskRetryBuilder<T> builder, int retries)
        {
            BaseRunner runner = new TaskRunner<T>();
            builder.AddRunner(runner);
            builder.SetRetryCount(retries);
            return builder;
        }

        public static TaskRetryBuilder<T> ThenTry<T>(this TaskRetryBuilder<T> builder, Func<Task<T>> task, int retries)
        {
            return builder
                .AddRunner(new TaskRunner<T>())
                .SetActor(task)
                .SetRetryCount(retries) as TaskRetryBuilder<T>;
        }

        public static TaskRetryBuilder<T> ThenTry<T>(this TaskRetryBuilder<T> builder, Func<T> func, int retries)
        {
            var ft = new Func<Task<T>>(() => new Task<T>(func));
            return ThenTry(builder, ft, retries);
        }

        #endregion //ThenTry Task<T> extensions:


        #region TaskRetryBuilder<T> Delay, WithErrorPolicy, WithSuccessPolicy

        #region Delays

        /// <summary>
        /// Apply a delay that will pause for the duration of the provided pause time with each retry.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pauseTime">The TimeSpan for the delay.</param>
        /// <returns></returns>
        public static TaskRetryBuilder<T> UsingDelay<T>(this TaskRetryBuilder<T> builder, TimeSpan pauseTime)
        {
            return builder.SetDelay(new BasicDelay(pauseTime))
                as TaskRetryBuilder<T>;
        }

        /// <summary>
        /// Apply a delay that will double the value of the initial delay with each retry'
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initialDelay">The TimeSpan for the initial delay.</param>
        /// <returns></returns>
        public static TaskRetryBuilder<T> UsingBackoffDelay<T>(this TaskRetryBuilder<T> builder, TimeSpan initialDelay)
        {
            return builder.SetDelay(new BackoffDelay(initialDelay))
                as TaskRetryBuilder<T>;
        }

        /// <summary>
        /// Apply a delay that will increase according to the Fibonacci sequence with each retry.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initialDelay">The TimeSpan for the initial delay.</param>
        /// <returns></returns>
        public static TaskRetryBuilder<T> UsingFibonacciDelay<T>(this TaskRetryBuilder<T> builder, TimeSpan initialDelay)
        {
            return builder.SetDelay(new FibonacciDelay(initialDelay))
                as TaskRetryBuilder<T>;
        }

        public static TaskRetryBuilder<T> UsingNoDelay<T>(this TaskRetryBuilder<T> builder)
        {
            return builder.SetDelay(new NoDelay())
                as TaskRetryBuilder<T>;
        }


        /// <summary>
        /// Provide an optional delay policy for pausing between tries.
        /// </summary>
        /// <param name="builder">The <see cref="MethodRetryBuilder"/> this method extends.</param>
        /// <param name="delay">The delay policy (<see cref="IDelay"/> instance) to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The delay parameter is null.</exception>
        public static TaskRetryBuilder<T> UsingDelay<T>(this TaskRetryBuilder<T> builder, IDelay delay)
        {
            return builder.SetDelay(delay)
                as TaskRetryBuilder<T>;
        }

        #endregion //Delays


        public static TaskRetryBuilder<T> WithErrorPolicy<T>(this TaskRetryBuilder<T> builder, ErrorPolicyDelegate errorPolicy)
        {
            return builder
                .SetErrorPolicy(errorPolicy) as TaskRetryBuilder<T>;
        }

        public static TaskRetryBuilder<T> WithSuccessPolicy<T>(this TaskRetryBuilder<T> builder, SuccessPolicyDelegate<T> successPolicy)
        {
            return builder
                .SetSuccessPolicy(successPolicy) as TaskRetryBuilder<T>;
        }

        #endregion //TaskRetryBuilder<T> Delay, WithErrorPolicy, WithSuccessPolicy
    }
}
