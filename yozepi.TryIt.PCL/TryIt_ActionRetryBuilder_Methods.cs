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


        #region ThenTry extensions:

        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner);
            builder.AddRunner(runner);
            builder.SetRetryCount(retries);
            return builder;
        }

        public static ActionRetryBuilder ThenTry<T>(this ActionRetryBuilder builder, T arg, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner, arg);
            return builder.AddRunner(runner)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2>(this ActionRetryBuilder builder, T1 arg1, T2 arg2, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner, arg1, arg2);
            return builder.AddRunner(runner)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3>(this ActionRetryBuilder builder, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner, arg1, arg2, arg3);

            return builder.AddRunner(runner)
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

        #endregion //ThenTry extensions:


        #region ThenTry Alternate Action extensions:

        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, Action altAction, int retries)
        {
            return builder
                .AddRunner(new ActionRunner())
                .SetActor(altAction)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T>(this ActionRetryBuilder builder, Action<T> altAction, T arg, int retries)
        {
            return builder
                .AddRunner(new ActionRunner<T>(arg))
                .SetActor(altAction)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2>(this ActionRetryBuilder builder, Action<T1, T2> altAction, T1 arg1, T2 arg2, int retries)
        {
            return builder
                .AddRunner(new ActionRunner<T1, T2>(arg1, arg2))
                .SetActor(altAction)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3>(this ActionRetryBuilder builder, Action<T1, T2, T3> altAction, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return builder
               .AddRunner(new ActionRunner<T1, T2, T3>(arg1, arg2, arg3))
               .SetActor(altAction)
               .SetRetryCount(retries) as ActionRetryBuilder;
        }

        public static ActionRetryBuilder ThenTry<T1, T2, T3, T4>(this ActionRetryBuilder builder, Action<T1, T2, T3, T4> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return builder
               .AddRunner(new ActionRunner<T1, T2, T3, T4>(arg1, arg2, arg3, arg4))
               .SetActor(altAction)
               .SetRetryCount(retries) as ActionRetryBuilder;
        }

        #endregion //ThenTry Alternate Action extensions:


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
