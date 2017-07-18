using System;
using yozepi.Retry.Builders;
using yozepi.Retry.Delays;
using yozepi.Retry.Runners;

namespace yozepi.Retry
{
    /// <summary>
    /// Here's where the magic begins! Use TryIt's various static methods and extensions
    /// to Try and Re-Try your actions and methods.
    /// </summary>
    public static partial class TryIt
    {

        #region Try methods:

        /// <summary>
        /// Try the provided <see cref="Action"/> the specified number of times
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to try.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static MethodRetryBuilder Try(Action action, int retries)
        {
            return new MethodRetryBuilder()
                .AddRunner(new ActionRunner())
                .SetActor(action)
                .SetRetryCount(retries) as MethodRetryBuilder;
        }


        #endregion //Try methods:


        #region ThenTry extensions:

        public static MethodRetryBuilder ThenTry(this MethodRetryBuilder builder, int retries)
        {
            BaseRunner runner = new ActionRunner();
            builder.AddRunner(runner);
            builder.SetRetryCount(retries);
            return builder;
        }

        public static MethodRetryBuilder ThenTry(this MethodRetryBuilder builder, Action action, int retries)
        {
            return builder
                .AddRunner(new ActionRunner())
                .SetActor(action)
                .SetRetryCount(retries) as MethodRetryBuilder;
        }


        #endregion //ThenTry extensions:


        #region UsingDelay, WithErrorPolicy, WithSuccessPolicy

        /// <summary>
        /// Provide an optional delay policy for pausing between tries.
        /// </summary>
        /// <param name="builder">The <see cref="MethodRetryBuilder"/> this method extends.</param>
        /// <param name="delay">The delay policy (<see cref="IDelay"/> instance) to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The delay parameter is null.</exception>
        public static MethodRetryBuilder UsingDelay(this MethodRetryBuilder builder, IDelay delay)
        {
            builder.SetDelay(delay);
            return builder;

        }


        /// <summary>
        /// An optional policy (an <see cref="ErrorPolicyDelegate"/>) you can pass to override typical retry on error behavior.
        /// </summary>
        /// <param name="builder">The <see cref="MethodRetryBuilder"/> this method extends.</param>
        /// <param name="errorPolicy">The <see cref="ErrorPolicyDelegate"/> to execute when an exception occurs in you action.</param>
        /// <returns></returns>
        /// <remarks>
        /// Normally TryIt will capture an error when trying and try again. You can provide an <see cref="ErrorPolicyDelegate"/> to override this behavior.
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
        public static MethodRetryBuilder WithErrorPolicy(this MethodRetryBuilder builder, ErrorPolicyDelegate errorPolicy)
        {
            builder.SetErrorPolicy(errorPolicy);
            return builder;
        }


        /// <summary>
        /// An optional policy (an <see cref="SuccessPolicyDelegate"/>) you can use to reject an otherwise successful try.
        /// </summary>
        /// <param name="builder">The <see cref="MethodRetryBuilder"/> this method extends.</param>
        /// <param name="successPolicy">The <see cref="SuccessPolicyDelegate"/> delegate to execute when a try succeeds (does not throw any exceptions).</param>
        /// <returns></returns>
        /// <remarks>Normally TryIt will consider any action that does not throw an exception to be a success. You can override this behavior to test if your action really meets a success criteria.
        /// <para>
        /// Throw an exception to override typical behavior. The success will be ignored, your exception will be added to <see cref="MethodRetryBuilder.ExceptionList"/> and your action will be retried.
        /// </para>
        /// <para>
        /// Capture your exception in an <see cref="WithErrorPolicy(MethodRetryBuilder, ErrorPolicyDelegate)"/> policy to stop retrying and rethrow your exception.
        /// </para>
        /// </remarks>
        public static MethodRetryBuilder WithSuccessPolicy(this MethodRetryBuilder builder, SuccessPolicyDelegate successPolicy)
        {
            builder.SetSuccessPolicy(successPolicy);
            return builder;
        }


        #endregion //UsingDelay, WithErrorPolicy, WithSuccessPolicy


    }
}
