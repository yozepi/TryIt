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
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try(Action action, int retries)
        {
            return new ActionTryIt(retries, action);
        }


        /// <summary>
        /// Try the provided <see cref="Action{T}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T}"/> instance to try.</param>
        /// <param name="arg">The argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T>(Action<T> action, T arg, int retries)
        {
            return new ActionTryIt<T>(retries, arg, action);
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
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int retries)
        {
            return new ActionTryIt<T1, T2>(retries, arg1, arg2, action);
        }


        /// <summary>
        /// Try the provided <see cref="Action{T1, T2, T3}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2, T3}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="arg3">The third argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return new ActionTryIt<T1, T2, T3>(retries, arg1, arg2, arg3, action);
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
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4>(retries, arg1, arg2, arg3, arg4, action);
        }


        /// <summary>
        /// Try the provided <see cref="Action{T1, T2, T3, T4, T5}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        /// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        /// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2, T3, T4, T5}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="arg3">The third argument passed into the action.</param>
        /// <param name="arg4">The fourth argument passed into the action.</param>
        /// <param name="arg5">The fifth argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5>(retries, arg1, arg2, arg3, arg4, arg5, action);
        }


        /// <summary>
        /// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        /// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        /// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        /// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="arg3">The third argument passed into the action.</param>
        /// <param name="arg4">The fourth argument passed into the action.</param>
        /// <param name="arg5">The fifth argument passed into the action.</param>
        /// <param name="arg6">The sixth argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6>(retries, arg1, arg2, arg3, arg4, arg5, arg6, action);
        }



        /// <summary>
        /// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6, T7}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        /// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        /// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        /// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        /// <typeparam name="T7">The <see cref="Type"/> of the seventh argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6, T7}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="arg3">The third argument passed into the action.</param>
        /// <param name="arg4">The fourth argument passed into the action.</param>
        /// <param name="arg5">The fifth argument passed into the action.</param>
        /// <param name="arg6">The sixth argument passed into the action.</param>
        /// <param name="arg7">The seventh argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, action);
        }


        /// <summary>
        /// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        /// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        /// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        /// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        /// <typeparam name="T7">The <see cref="Type"/> of the seventh argument passed to the action.</typeparam>
        /// <typeparam name="T8">The <see cref="Type"/> of the eighth argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6, T7, T8}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="arg3">The third argument passed into the action.</param>
        /// <param name="arg4">The fourth argument passed into the action.</param>
        /// <param name="arg5">The fifth argument passed into the action.</param>
        /// <param name="arg6">The sixth argument passed into the action.</param>
        /// <param name="arg7">The seventh argument passed into the action.</param>
        /// <param name="arg8">The eighth argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, action);
        }


        /// <summary>
        /// Try the provided <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> the specified number of times
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <typeparam name="T3">The <see cref="Type"/> of the third argument passed to the action.</typeparam>
        /// <typeparam name="T4">The <see cref="Type"/> of the fourth argument passed to the action.</typeparam>
        /// <typeparam name="T5">The <see cref="Type"/> of the fifth argument passed to the action.</typeparam>
        /// <typeparam name="T6">The <see cref="Type"/> of the sixth argument passed to the action.</typeparam>
        /// <typeparam name="T7">The <see cref="Type"/> of the seventh argument passed to the action.</typeparam>
        /// <typeparam name="T8">The <see cref="Type"/> of the eighth argument passed to the action.</typeparam>
        /// <typeparam name="T9">The <see cref="Type"/> of the nineth argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2, T3, T4, T5, T6, T7, T8, T9}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="arg3">The third argument passed into the action.</param>
        /// <param name="arg4">The fourth argument passed into the action.</param>
        /// <param name="arg5">The fifth argument passed into the action.</param>
        /// <param name="arg6">The sixth argument passed into the action.</param>
        /// <param name="arg7">The seventh argument passed into the action.</param>
        /// <param name="arg8">The eighth argument passed into the action.</param>
        /// <param name="arg9">The nineth argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, action);
        }

        #endregion //Try methods:


        #region Try Task methods:

        /// <summary>
        /// Try the provided <see cref="Func{Task}"/> the specified number of times.
        /// </summary>
        /// <param name="func">The <see cref="Func{Task}"/> to try.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        /// <remarks>
        /// TryIt treats functions that return tasks a little differently than you may expect. Instead of returning the Task, 
        /// it attempts execute it. This is because a task by itself can't be retried. And, since
        /// the task (presumably) hasn't finnished running yet when it has returned, theres no way
        /// to test it within try-retry context.
        /// </remarks>
        public static ITry Try(Func<Task> func, int retries)
        {
            return new TaskTryIt(retries, func);
        }


        /// <summary>
        /// Try the provided <see cref="Func{Task}"/> the specified number of times.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the argument passed to the action.</typeparam>
        /// <param name="func">The <see cref="Func{T, Task}"/> to try.</param>
        /// <param name="arg">The argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        /// <remarks>
        /// TryIt treats functions that return tasks a little differently than you may expect. Instead of returning the Task, 
        /// it attempts execute it. This is because a task by itself can't be retried. And, since
        /// the task (presumably) hasn't finnished running yet when it has returned, theres no way
        /// to test it within try-retry context.
        /// </remarks>
        public static ITry Try<T>(Func<T, Task> func, T arg, int retries)
        {
            return new TaskTryIt<T>(retries, arg, func);
        }


        /// <summary>
        /// Try the provided <see cref="Func{Task}"/> the specified number of times.
        /// </summary>
        /// <typeparam name="T1">The <see cref="Type"/> of the first argument passed to the action.</typeparam>
        /// <typeparam name="T2">The <see cref="Type"/> of the second argument passed to the action.</typeparam>
        /// <param name="action">The <see cref="Action{T, T2}"/> instance to try.</param>
        /// <param name="arg1">The first argument passed into the action.</param>
        /// <param name="arg2">The second argument passed into the action.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        /// <remarks>
        /// TryIt treats functions that return tasks a little differently than you may expect. Instead of returning the Task, 
        /// it attempts execute it. This is because a task by itself can't be retried. And, since
        /// the task (presumably) hasn't finnished running yet when it has returned, theres no way
        /// to test it within try-retry context.
        /// </remarks>
        public static ITry Try<T1, T2>(Func<T1, T2, Task> func, T1 arg1, T2 arg2, int retries)
        {
            return new TaskTryIt<T1, T2>(retries, arg1, arg2, func);
        }


        #endregion //Try Task methods:


        #region UsingDelay, OnError, OnSuccess

        /// <summary>
        /// Provide an optional delay policy for pausing between tries.
        /// </summary>
        /// <param name="tryit">The <see cref="ITry"/> this method extends.</param>
        /// <param name="delay">The delay policy (<see cref="IDelay"/> instance) to use.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <exception cref="ArgumentNullException">Thrown when The delay parameter is null.</exception>
        public static ITry UsingDelay(this ITry tryit, IDelay delay)
        {
            if (delay == null)
                throw new ArgumentNullException("delay");

            IInternalAccessor source = tryit as IInternalAccessor;
            source.Delay = delay;
            return tryit;

        }


        /// <summary>
        /// An optional policy (an <see cref="OnErrorDelegate"/>) you can pass to override typical retry on error behavior.
        /// </summary>
        /// <param name="tryit">The <see cref="ITry"/> this method extends.</param>
        /// <param name="onError">The <see cref="OnErrorDelegate"/> to execute when an exception occurs in you action.</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <remarks>
        /// Normally Tryit will capture an error when trying and try again. You can provide an <see cref="OnErrorDelegate"/> to override this behavior.
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
        public static ITry OnError(this ITry tryit, OnErrorDelegate onError)
        {
            var accessor = tryit as IInternalAccessor;
            accessor.OnError = onError;
            return tryit;
        }


        /// <summary>
        /// An optional policy (an <see cref="OnSuccessDelegate"/>) you can use to reject an otherwise successful try.
        /// </summary>
        /// <param name="tryit">The <see cref="ITry"/> this method extends.</param>
        /// <param name="onSuccess">The <see cref="OnSuccessDelegate"/> delegate to execute when a try succeeds (does not throw any exceptions).</param>
        /// <returns>Returns an ITry instance that you use to chain Then-try calls or to add OnError and OnSuccess policies.</returns>
        /// <remarks>Normally TryIt will consider any action that does not throw an exception to be a success. You can override this behavior to test if your action really meets a success criteria.
        /// <para>
        /// Throw an exception to override typical behavior. The success will be ignored, your exception will be added to <see cref="ITry.ExceptionList"/> and your action will be retried.
        /// </para>
        /// <para>
        /// Capture your exception in an <see cref="OnError(ITry, OnErrorDelegate)"/> policy to stop retrying and rethrow your exception.
        /// </para>
        /// </remarks>
        public static ITry OnSuccess(this ITry tryit, OnSuccessDelegate onSuccess)
        {
            var accessor = tryit as IInternalAccessor;
            accessor.OnSuccess = onSuccess;
            return tryit;
        }

        
        #endregion //UsingDelay, OnError, OnSuccess


        #region ThenTry extensions:

        public static ITry ThenTry(this ITry tryit, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            IInternalAccessor child;
            if (parent.GetType() == typeof(TaskTryIt))
            {
                child = new TaskTryIt(retries, parent.Actor as Func<Task>);
            }
            else
            {
                child = new ActionTryIt(retries, parent.Actor as Action);
            }
            child.Parent = parent;
            return child as ITry;
        }

        public static ITry ThenTry(this ITry tryit, Action altAction, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt(retries, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry(this ITry tryit, Func<Task> altFunc, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new TaskTryIt(retries, altFunc);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }


        public static ITry ThenTry<T>(this ITry tryit, T arg, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            IInternalAccessor child;
            if (parent.GetType() == typeof(TaskTryIt<T>))
            {
                child = new TaskTryIt<T>(retries, arg, parent.Actor as Func<T, Task>);
            }
            else
            {
                child = new ActionTryIt<T>(retries, arg, parent.Actor as Action<T>);
            }
            child.Parent = parent;
            return child as ITry;
        }

        public static ITry ThenTry<T>(this ITry tryit, Action<T> altAction, T arg, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T>(retries, arg, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T>(this ITry tryit, Func<T, Task> altFunc, T arg, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new TaskTryIt<T>(retries, arg, altFunc);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2>(this ITry tryit, T1 arg1, T2 arg2, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            IInternalAccessor child;
            if (parent.GetType() == typeof(TaskTryIt<T1, T2>))
            {
                child = new TaskTryIt<T1, T2>(retries, arg1, arg2, parent.Actor as Func<T1, T2, Task>);
            }
            else
            {
                child = new ActionTryIt<T1, T2>(retries, arg1, arg2, parent.Actor as Action<T1, T2>);
            }
            child.Parent = parent;
            return child as ITry;
        }

        public static ITry ThenTry<T1, T2>(this ITry tryit, Action<T1, T2> altAction, T1 arg1, T2 arg2, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2>(retries, arg1, arg2, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2>(this ITry tryit, Func<T1, T2, Task> altFunc, T1 arg1, T2 arg2, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new TaskTryIt<T1, T2>(retries, arg1, arg2, altFunc);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3>(retries, arg1, arg2, arg3, parent.Actor as Action<T1, T2, T3>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3>(this ITry tryit, Action<T1, T2, T3> altAction, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3>(retries, arg1, arg2, arg3, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4>(retries, arg1, arg2, arg3, arg4, parent.Actor as Action<T1, T2, T3, T4>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4>(this ITry tryit, Action<T1, T2, T3, T4> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4>(retries, arg1, arg2, arg3, arg4, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5>(retries, arg1, arg2, arg3, arg4, arg5, parent.Actor as Action<T1, T2, T3, T4, T5>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5>(this ITry tryit, Action<T1, T2, T3, T4, T5> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5>(retries, arg1, arg2, arg3, arg4, arg5, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6>(retries, arg1, arg2, arg3, arg4, arg5, arg6, parent.Actor as Action<T1, T2, T3, T4, T5, T6>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6>(this ITry tryit, Action<T1, T2, T3, T4, T5, T6> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6>(retries, arg1, arg2, arg3, arg4, arg5, arg6, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6, T7>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6, T7>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, parent.Actor as Action<T1, T2, T3, T4, T5, T6, T7>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6, T7>(this ITry tryit, Action<T1, T2, T3, T4, T5, T6, T7> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6, T7>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6, T7, T8>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, parent.Actor as Action<T1, T2, T3, T4, T5, T6, T7, T8>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6, T7, T8>(this ITry tryit, Action<T1, T2, T3, T4, T5, T6, T7, T8> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, parent.Actor as Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ITry tryit, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> altAction, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        #endregion //ThenTry extensions:

    }
}
