using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry.Builders;
using yozepi.Retry.Delays;
using yozepi.Retry.Exceptions;
using yozepi.Retry.Runners;

namespace yozepi.Retry
{
    public static partial class TryIt
    {

        public static MethodRetryBuilder<T> Try<T>(Func<T> func, int retries) 
        {
            VerifyNotTask<T>();
            return new MethodRetryBuilder<T>()
                .AddRunner(new FuncRunner<T>())
                .SetActor(func)
                .SetRetryCount(retries) as MethodRetryBuilder<T>;
        }


        public static MethodRetryBuilder<T> ThenTry<T>(this MethodRetryBuilder<T> builder, int retries)
        {
            BaseRunner runner = new FuncRunner<T>();

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as MethodRetryBuilder<T>;
        }


        public static MethodRetryBuilder<T> ThenTry<T>(this MethodRetryBuilder<T> builder, Func<T> func, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<T>())
                .SetActor(func)
                .SetRetryCount(retries) as MethodRetryBuilder<T>;
        }


        #region UsingDelay, WithErrorPolicy, WithSuccessPolicy

        #region Delays

        /// <summary>
        /// Apply a delay that will pause for the duration of the provided pause time with each retry.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pauseTime">The TimeSpan for the delay.</param>
        /// <returns></returns>
        public static MethodRetryBuilder<T> UsingDelay<T>(this MethodRetryBuilder<T> builder, TimeSpan pauseTime)
        {
            return builder.SetDelay(new BasicDelay(pauseTime))
                as MethodRetryBuilder<T>;
        }

        /// <summary>
        /// Apply a delay that will double the value of the initial delay with each retry'
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initialDelay">The TimeSpan for the initial delay.</param>
        /// <returns></returns>
        public static MethodRetryBuilder<T> UsingBackoffDelay<T>(this MethodRetryBuilder<T> builder, TimeSpan initialDelay)
        {
            return builder.SetDelay(new BackoffDelay(initialDelay))
                as MethodRetryBuilder<T>;
        }

        /// <summary>
        /// Apply a delay that will increase according to the Fibonacci sequence with each retry.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initialDelay">The TimeSpan for the initial delay.</param>
        /// <returns></returns>
        public static MethodRetryBuilder<T> UsingFibonacciDelay<T>(this MethodRetryBuilder<T> builder, TimeSpan initialDelay)
        {
            return builder.SetDelay(new FibonacciDelay(initialDelay))
                as MethodRetryBuilder<T>;
        }

        public static MethodRetryBuilder<T> UsingNoDelay<T>(this MethodRetryBuilder<T> builder)
        {
            return builder.SetDelay(new NoDelay())
                as MethodRetryBuilder<T>;
        }


        /// <summary>
        /// Provide an optional delay policy for pausing between tries.
        /// </summary>
        /// <param name="builder">The <see cref="MethodRetryBuilder"/> this method extends.</param>
        /// <param name="delay">The delay policy (<see cref="IDelay"/> instance) to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The delay parameter is null.</exception>
        public static MethodRetryBuilder<T> UsingDelay<T>(this MethodRetryBuilder<T> builder, IDelay delay)
        {
            return builder.SetDelay(delay)
                as MethodRetryBuilder<T>;
        }

        #endregion //Delays

        public static MethodRetryBuilder<T> WithErrorPolicy<T>(this MethodRetryBuilder<T> builder, ErrorPolicyDelegate errorPolicy)
        {
            return builder
                .SetErrorPolicy(errorPolicy) as MethodRetryBuilder<T>;
        }

        public static MethodRetryBuilder<T> WithSuccessPolicy<T>(this MethodRetryBuilder<T> builder, SuccessPolicyDelegate<T> successPolicy)
        {
            return builder
                .SetSuccessPolicy(successPolicy) as MethodRetryBuilder<T>;
        }

        #endregion //UsingDelay, WithErrorPolicy, WithSuccessPolicy


        private static TypeInfo TaskInfo = typeof(Task).GetTypeInfo();
        internal const string TaskErrorMessage = "The Try method does not support Tasks. Use TryAsync instead";

        private static void VerifyNotTask<T>()
        {
            var TResultTypeInfo = typeof(T).GetTypeInfo();

            if (TResultTypeInfo.IsAssignableFrom(TaskInfo)
                || TResultTypeInfo.IsSubclassOf(typeof(Task)))
            {
                throw new TaskNotAllowedException(TaskErrorMessage);
            }
        }
    }
}
