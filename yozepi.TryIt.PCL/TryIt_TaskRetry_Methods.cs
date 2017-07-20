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

        public static TaskRetryBuilder TryAsync(Func<Task> task, int retries)
        {
            return new TaskRetryBuilder()
            .AddRunner(new TaskRunner())
            .SetActor(task)
            .SetRetryCount(retries) as TaskRetryBuilder;
        }

        public static TaskRetryBuilder TryAsync(Action action, int retries)
        {
            var ft = new Func<Task>(() => new Task(action));
            return TryAsync(ft, retries);
        }

        #region ThenTry Task extensions:

        public static TaskRetryBuilder ThenTry(this TaskRetryBuilder builder, int retries)
        {
            BaseRunner runner = new TaskRunner();
            builder.AddRunner(runner);
            builder.SetRetryCount(retries);
            return builder;
        }

        public static TaskRetryBuilder ThenTry(this TaskRetryBuilder builder, Func<Task> task, int retries)
        {
            return builder
                .AddRunner(new TaskRunner())
                .SetActor(task)
                .SetRetryCount(retries) as TaskRetryBuilder;
        }

        public static TaskRetryBuilder ThenTry(this TaskRetryBuilder builder, Action action, int retries)
        {
            var ft = new Func<Task>(() => new Task(action));
            return ThenTry(builder, ft, retries);
        }


        #endregion //ThenTry Task extensions:


        #region TaskRetryBuilder Delay, WithErrorPolicy, WithSuccessPolicy

        #region Delays

        /// <summary>
        /// Apply a delay that will pause for the duration of the provided pause time with each retry.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pauseTime">The TimeSpan for the delay.</param>
        /// <returns></returns>
        public static TaskRetryBuilder UsingDelay(this TaskRetryBuilder builder, TimeSpan pauseTime)
        {
            return builder.SetDelay(new BasicDelay(pauseTime))
                as TaskRetryBuilder;
        }

        /// <summary>
        /// Apply a delay that will double the value of the initial delay with each retry'
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initialDelay">The TimeSpan for the initial delay.</param>
        /// <returns></returns>
        public static TaskRetryBuilder UsingBackoffDelay(this TaskRetryBuilder builder, TimeSpan initialDelay)
        {
            return builder.SetDelay(new BackoffDelay(initialDelay))
                as TaskRetryBuilder;
        }

        /// <summary>
        /// Apply a delay that will increase according to the Fibonacci sequence with each retry.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initialDelay">The TimeSpan for the initial delay.</param>
        /// <returns></returns>
        public static TaskRetryBuilder UsingFibonacciDelay(this TaskRetryBuilder builder, TimeSpan initialDelay)
        {
            return builder.SetDelay(new FibonacciDelay(initialDelay))
                as TaskRetryBuilder;
        }

        public static TaskRetryBuilder UsingNoDelay(this TaskRetryBuilder builder)
        {
            return builder.SetDelay(new NoDelay())
                as TaskRetryBuilder;
        }


        /// <summary>
        /// Provide an optional delay policy for pausing between tries.
        /// </summary>
        /// <param name="builder">The <see cref="MethodRetryBuilder"/> this method extends.</param>
        /// <param name="delay">The delay policy (<see cref="IDelay"/> instance) to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The delay parameter is null.</exception>
        public static TaskRetryBuilder UsingDelay(this TaskRetryBuilder builder, IDelay delay)
        {
            return builder.SetDelay(delay)
                as TaskRetryBuilder;
        }

        #endregion //Delays


        /// <summary>
        /// An optional policy (an <see cref="ErrorPolicyDelegate"/>) you can pass to override typical retry on error behavior.
        /// </summary>
        /// <param name="builder">The <see cref="ITry"/> this method extends.</param>
        /// <param name="errorPolicy">The <see cref="ErrorPolicyDelegate"/> to execute when an exception occurs in you action.</param>
        /// <returns></returns>
        /// <remarks>
        /// Normally Tryit will capture an error when trying and try again. You can provide an <see cref="ErrorPolicyDelegate"/> to override this behavior.
        /// <para>
        ///     Returning false from this delegate will cause the Try-ThenTry chain to stop and the exception to be thrown.
        /// </para>
        /// <para>
        ///     Raising a new exception will also cause the Try-ThenTry chain to stop and the exception to be thrown.
        /// </para>
        /// <para>
        ///     Returning true will cause normal behavior - recording the exception and retrying.
        /// </para>
        /// </remarks>
        public static TaskRetryBuilder WithErrorPolicy(this TaskRetryBuilder builder, ErrorPolicyDelegate errorPolicy)
        {
            builder.SetErrorPolicy(errorPolicy);
            return builder;
        }


        /// <summary>
        /// An optional policy (an <see cref="SuccessPolicyDelegate"/>) you can use to reject an otherwise successful try.
        /// </summary>
        /// <param name="builder">The <see cref="ITry"/> this method extends.</param>
        /// <param name="successPolicy">The <see cref="SuccessPolicyDelegate"/> delegate to execute when a try succeeds (does not throw any exceptions).</param>
        /// <returns></returns>
        /// <remarks>Normally TryIt will consider any action that does not throw an exception to be a success. You can override this behavior to test if your action really meets a success criteria.
        /// <para>
        /// Throw an exception to override typical behavior. The success will be ignored, your exception will be added to <see cref="ITry.ExceptionList"/> and your action will be retried.
        /// </para>
        /// <para>
        /// Capture your exception in an <see cref="WithErrorPolicy(MethodRetryBuilder, ErrorPolicyDelegate)"/> policy to stop retrying and rethrow your exception.
        /// </para>
        /// </remarks>
        public static TaskRetryBuilder WithSuccessPolicy(this TaskRetryBuilder builder, SuccessPolicyDelegate successPolicy)
        {
            builder.SetSuccessPolicy(successPolicy);
            return builder;
        }


        #endregion //TaskRetryBuilder Delay, WithErrorPolicy, WithSuccessPolicy

    }
}
