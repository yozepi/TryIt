using Retry.Builders;
using Retry.Delays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public static partial class TryIt
    {

        #region ActionRetryBuilder UsingDelay, WithErrorPolicy, WithSuccessPolicy

        /// <summary>
        /// Provide an optional delay policy for pausing between tries.
        /// </summary>
        /// <param name="builder">The <see cref="ITry"/> this method extends.</param>
        /// <param name="delay">The delay policy (<see cref="IDelay"/> instance) to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The delay parameter is null.</exception>
        public static ActionRetryBuilder UsingDelay(this ActionRetryBuilder builder, IDelay delay)
        {
            builder.SetDelay(delay);
            return builder;

        }


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
        public static ActionRetryBuilder WithErrorPolicy(this ActionRetryBuilder builder, ErrorPolicyDelegate errorPolicy)
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
        /// Capture your exception in an <see cref="WithErrorPolicy(ActionRetryBuilder, ErrorPolicyDelegate)"/> policy to stop retrying and rethrow your exception.
        /// </para>
        /// </remarks>
        public static ActionRetryBuilder WithSuccessPolicy(this ActionRetryBuilder builder, SuccessPolicyDelegate successPolicy)
        {
            builder.SetSuccessPolicy(successPolicy);
            return builder;
        }


        #endregion //ActionRetryBuilder UsingDelay, WithErrorPolicy, WithSuccessPolicy


        #region FuncRetryBuilder<TResult> UsingDelay, WithErrorPolicy, WithSuccessPolicy

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

        #endregion //FuncRetryBuilder<TResult> UsingDelay, WithErrorPolicy, WithSuccessPolicy

    }
}
