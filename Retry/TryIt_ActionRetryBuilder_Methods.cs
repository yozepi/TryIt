using Retry.Builders;
using Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
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
        public static ActionRetryBuilder Try(Action action, int retries)
        {
            return new ActionRetryBuilder()
                .AddRunner(new ActionRunner())
                .SetActor(action)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }


        /// <summary>
        /// Try the provided <see cref="Action{T}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T}"/> instance to try.</param>
        /// <param name="arg">The argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ActionRetryBuilder Try<T>(Action<T> action, T arg, int retries)
        {
            return new ActionRetryBuilder()
               .AddRunner(new ActionRunner<T>(arg))
               .SetActor(action)
               .SetRetryCount(retries) as ActionRetryBuilder;
        }


        /// <summary>
        /// Try the provided <see cref="Action{T1, T2}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ActionRetryBuilder Try<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int retries)
        {
            return new ActionRetryBuilder()
                .AddRunner(new ActionRunner<T1, T2>(arg1, arg2))
                .SetActor(action)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }


        ///// <summary>
        ///// Try the provided <see cref="Action{T1, T2, T3}"/> the specified number of times
        ///// </summary>
        ///// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        ///// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        ///// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        ///// <param name="action">The <see cref="Action{T, T2, T3}"/> instance to try.</param>
        ///// <param name="arg1">The first argument passed into the action.</param>
        ///// <param name="arg2">The second argument passed into the action.</param>
        ///// <param name="arg3">The third argument passed into the action.</param>
        ///// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        ///// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        ///// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        ///// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ActionRetryBuilder Try<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return new ActionRetryBuilder()
                 .AddRunner(new ActionRunner<T1, T2, T3>(arg1, arg2, arg3))
                 .SetActor(action)
                 .SetRetryCount(retries) as ActionRetryBuilder;
        }


        /// <summary>
        /// Try the provided <see cref="Action{T1, T2, T3, T4}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        /// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2, T3, T4}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="arg3">The third argument passed into the action.</param>
        /// <param name="arg4">The fourth argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ActionRetryBuilder Try<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return new ActionRetryBuilder()
                   .AddRunner(new ActionRunner<T1, T2, T3, T4>(arg1, arg2, arg3, arg4))
                   .SetActor(action)
                   .SetRetryCount(retries) as ActionRetryBuilder;
        }


        ///// <summary>
        ///// Try the provided <see cref="Action{T1, T2, T3, T4, T5}"/> the specified number of times
        ///// </summary>
        ///// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        ///// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        ///// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        ///// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        ///// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        ///// <param name="action">The <see cref="Action{T, T2, T3, T4, T5}"/> instance to try.</param>
        ///// <param name="arg1">The first argument passed into the action.</param>
        ///// <param name="arg2">The second argument passed into the action.</param>
        ///// <param name="arg3">The third argument passed into the action.</param>
        ///// <param name="arg4">The fourth argument passed into the action.</param>
        ///// <param name="arg5">The fifth argument passed into the action.</param>
        ///// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        ///// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        ///// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        ///// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        //public static ITry Try<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        //{
        //    return new ActionTryIt<T1, T2, T3, T4, T5>(retries, arg1, arg2, arg3, arg4, arg5, action);
        //}


        ///// <summary>
        ///// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6}"/> the specified number of times
        ///// </summary>
        ///// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        ///// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        ///// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        ///// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        ///// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        ///// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        ///// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6}"/> instance to try.</param>
        ///// <param name="arg1">The first argument passed into the action.</param>
        ///// <param name="arg2">The second argument passed into the action.</param>
        ///// <param name="arg3">The third argument passed into the action.</param>
        ///// <param name="arg4">The fourth argument passed into the action.</param>
        ///// <param name="arg5">The fifth argument passed into the action.</param>
        ///// <param name="arg6">The sixth argument passed into the action.</param>
        ///// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        ///// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        ///// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        ///// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        //public static ITry Try<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        //{
        //    return new ActionTryIt<T1, T2, T3, T4, T5, T6>(retries, arg1, arg2, arg3, arg4, arg5, arg6, action);
        //}



        ///// <summary>
        ///// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6, T7}"/> the specified number of times
        ///// </summary>
        ///// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        ///// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        ///// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        ///// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        ///// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        ///// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        ///// <typeparam name="T7">The <see cref="Type"/> of the seventh argument passed to the action.</typeparam>
        ///// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6, T7}"/> instance to try.</param>
        ///// <param name="arg1">The first argument passed into the action.</param>
        ///// <param name="arg2">The second argument passed into the action.</param>
        ///// <param name="arg3">The third argument passed into the action.</param>
        ///// <param name="arg4">The fourth argument passed into the action.</param>
        ///// <param name="arg5">The fifth argument passed into the action.</param>
        ///// <param name="arg6">The sixth argument passed into the action.</param>
        ///// <param name="arg7">The seventh argument passed into the action.</param>
        ///// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        ///// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        ///// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        ///// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        //public static ITry Try<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        //{
        //    return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, action);
        //}


        ///// <summary>
        ///// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8}"/> the specified number of times
        ///// </summary>
        ///// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        ///// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        ///// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        ///// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        ///// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        ///// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        ///// <typeparam name="T7">The <see cref="Type"/> of the seventh argument passed to the action.</typeparam>
        ///// <typeparam name="T8">The <see cref="Type"/> of the eighth argument passed to the action.</typeparam>
        ///// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6, T7, T8}"/> instance to try.</param>
        ///// <param name="arg1">The first argument passed into the action.</param>
        ///// <param name="arg2">The second argument passed into the action.</param>
        ///// <param name="arg3">The third argument passed into the action.</param>
        ///// <param name="arg4">The fourth argument passed into the action.</param>
        ///// <param name="arg5">The fifth argument passed into the action.</param>
        ///// <param name="arg6">The sixth argument passed into the action.</param>
        ///// <param name="arg7">The seventh argument passed into the action.</param>
        ///// <param name="arg8">The eighth argument passed into the action.</param>
        ///// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        ///// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        ///// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        ///// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        //public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        //{
        //    return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, action);
        //}


        ///// <summary>
        ///// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> the specified number of times
        ///// </summary>
        ///// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        ///// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        ///// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        ///// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        ///// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        ///// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        ///// <typeparam name="T7">The <see cref="Type"/> of the seventh argument passed to the action.</typeparam>
        ///// <typeparam name="T8">The <see cref="Type"/> of the eighth argument passed to the action.</typeparam>
        ///// <typeparam name="T9">The <see cref="Type"/> of the nineth argument passed to the action.</typeparam>
        ///// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance to try.</param>
        ///// <param name="arg1">The first argument passed into the action.</param>
        ///// <param name="arg2">The second argument passed into the action.</param>
        ///// <param name="arg3">The third argument passed into the action.</param>
        ///// <param name="arg4">The fourth argument passed into the action.</param>
        ///// <param name="arg5">The fifth argument passed into the action.</param>
        ///// <param name="arg6">The sixth argument passed into the action.</param>
        ///// <param name="arg7">The seventh argument passed into the action.</param>
        ///// <param name="arg8">The eighth argument passed into the action.</param>
        ///// <param name="arg9">The nineth argument passed into the action.</param>
        ///// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        ///// <returns>Returns an ITry instance that you use to chain Then-try calls or to add error and success policies.</returns>
        ///// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        ///// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        //public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        //{
        //    return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, action);
        //}


        #endregion //Try methods:


        #region Try Task methods:

        public static ActionRetryBuilder TryTask(Func<Task> func, int retries)
        {

            return new ActionRetryBuilder()
                .AddRunner(new TaskRunner())
                .SetActor(func)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder TryTask<T>(Func<T, Task> func, T arg, int retries)
        {

            return new ActionRetryBuilder()
                .AddRunner(new TaskRunner<T>(arg))
                .SetActor(func)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder TryTask<T1, T2>(Func<T1, T2, Task> func, T1 arg1, T2 arg2, int retries)
        {

            return new ActionRetryBuilder()
                .AddRunner(new TaskRunner<T1, T2>(arg1, arg2))
                .SetActor(func)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder TryTask<T1, T2, T3>(Func<T1, T2, T3, Task> func, T1 arg1, T2 arg2, T3 arg3, int retries)
        {

            return new ActionRetryBuilder()
                .AddRunner(new TaskRunner<T1, T2, T3>(arg1, arg2, arg3))
                .SetActor(func)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder TryTask<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {

            return new ActionRetryBuilder()
                .AddRunner(new TaskRunner<T1, T2, T3, T4>(arg1, arg2, arg3, arg4))
                .SetActor(func)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }


        #endregion //Try Task methods:


        #region UsingDelay, WithErrorPolicy, WithSuccessPolicy

        /// <summary>
        /// Provide an optional delay policy for pausing between tries.
        /// </summary>
        /// <param name="builder">The <see cref="ITry"/> this method extends.</param>
        /// <param name="delay">The delay policy (<see cref="IDelay"/> instance) to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The delay parameter is null.</exception>
        public static ActionRetryBuilder UsingDelay(this ActionRetryBuilder builder, IDelay delay)
        {
            if (delay == null)
                throw new ArgumentNullException("delay");

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


        #endregion //UsingDelay, WithErrorPolicy, WithSuccessPolicy


        #region ThenTry extensions:

        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, int retries)
        {
            BaseRunner runner = builder.LastRunner.Actor.GetType() == typeof(Action)
                ? new ActionRunner() as BaseRunner
                : new TaskRunner() as BaseRunner;

            builder.AddRunner(runner);
            builder.SetRetryCount(retries);
            return builder;
        }

        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, Action altAction, int retries)
        {
            return builder
                .ThenTry(retries)
                .SetActor(altAction)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }


        public static ActionRetryBuilder ThenTry<T>(this ActionRetryBuilder builder, T arg, int retries)
        {
            BaseRunner runner = builder.LastRunner.Actor.GetType() == typeof(Action<T>)
                ? new ActionRunner<T>(arg) as BaseRunner
                : new TaskRunner<T>(arg) as BaseRunner;

            return builder.AddRunner(runner)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T>(this ActionRetryBuilder builder, Action<T> altAction, T arg, int retries)
        {
            BaseRunner runner = builder.LastRunner.Actor.GetType() == typeof(Action<T>)
               ? new ActionRunner<T>(arg) as BaseRunner
               : new TaskRunner<T>(arg) as BaseRunner;

            return builder.AddRunner(runner)
                .SetActor(altAction)
                 .SetRetryCount(retries) as ActionRetryBuilder;
        }


        public static ActionRetryBuilder ThenTry<T1, T2>(this ActionRetryBuilder builder, T1 arg1, T2 arg2, int retries)
        {
            BaseRunner runner = builder.LastRunner.Actor.GetType() == typeof(Action<T1, T2>)
               ? new ActionRunner<T1, T2>(arg1, arg2) as BaseRunner
               : new TaskRunner<T1, T2>(arg1, arg2) as BaseRunner;

            return builder.AddRunner(runner)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2>(this ActionRetryBuilder builder, Action<T1, T2> altAction, T1 arg1, T2 arg2, int retries)
        {
            return builder
                .AddRunner(new ActionRunner<T1, T2>(arg1, arg2))
                .SetActor(altAction)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3>(this ActionRetryBuilder builder, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            BaseRunner runner = builder.LastRunner.Actor.GetType() == typeof(Action<T1, T2, T3>)
                 ? new ActionRunner<T1, T2, T3>(arg1, arg2, arg3)
                 : new TaskRunner<T1, T2, T3>(arg1, arg2, arg3) as BaseRunner;

            return builder.AddRunner(runner)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3>(this ActionRetryBuilder builder, Action<T1, T2, T3> altAction, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return builder
               .AddRunner(new ActionRunner<T1, T2, T3>(arg1, arg2, arg3))
               .SetActor(altAction)
               .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3, T4>(this ActionRetryBuilder builder, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            BaseRunner runner = builder.LastRunner.Actor.GetType() == typeof(Action<T1, T2, T3, T4>)
             ? new ActionRunner<T1, T2, T3, T4>(arg1, arg2, arg3, arg4)
             : new TaskRunner<T1, T2, T3, T4>(arg1, arg2, arg3, arg4) as BaseRunner;

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3, T4>(this ActionRetryBuilder builder, Action<T1, T2, T3, T4> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return builder
               .AddRunner(new ActionRunner<T1, T2, T3, T4>(arg1, arg2, arg3, arg4))
               .SetActor(altAction)
               .SetRetryCount(retries) as ActionRetryBuilder;
        }


        #endregion //ThenTry extensions:


        #region ThenTry Alternate Task extensions:
        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, Func<Task> altFunc, int retries)
        {
            return builder
                .AddRunner(new TaskRunner())
                .SetActor(altFunc)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T>(this ActionRetryBuilder builder, Func<T, Task> altFunc, T arg, int retries)
        {
            return builder
                .AddRunner(new TaskRunner<T>(arg))
                .SetActor(altFunc)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2>(this ActionRetryBuilder builder, Func<T1, T2, Task> altFunc, T1 arg1, T2 arg2, int retries)
        {
            return builder
                .AddRunner(new TaskRunner<T1, T2>(arg1, arg2))
                .SetActor(altFunc)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3>(this ActionRetryBuilder builder, Func<T1, T2, T3, Task> altFunc, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return builder
                .AddRunner(new TaskRunner<T1, T2, T3>(arg1, arg2, arg3))
                .SetActor(altFunc)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3, T4>(this ActionRetryBuilder builder, Func<T1, T2, T3, T4, Task> altFunc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return builder
                .AddRunner(new TaskRunner<T1, T2, T3, T4>(arg1, arg2, arg3, arg4))
                .SetActor(altFunc)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        #endregion //ThenTry Alternate Task extensions:
    }
}
