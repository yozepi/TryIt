using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public static partial class TryIt
    {

        #region Try methods:

        public static ITry Try(Action action, int retries)
        {
            return new ActionTryIt(retries, action);
        }

        public static ITry Try<T>(Action<T> action, T arg, int retries)
        {
            return new ActionTryIt<T>(retries, arg, action);
        }

        public static ITry Try<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, int retries)
        {
            return new ActionTryIt<T1, T2>(retries, arg1, arg2, action);
        }

        public static ITry Try<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return new ActionTryIt<T1, T2, T3>(retries, arg1, arg2, arg3, action);
        }

        public static ITry Try<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4>(retries, arg1, arg2, arg3, arg4, action);
        }

        public static ITry Try<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5>(retries, arg1, arg2, arg3, arg4, arg5, action);
        }

        public static ITry Try<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6>(retries, arg1, arg2, arg3, arg4, arg5, arg6, action);
        }

        public static ITry Try<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, action);
        }

        public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, action);
        }

        public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        {
            return new ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, action);
        }

        #endregion //Try methods:


        public static ITry UsingDelay(this ITry tryit, IDelay delay)
        {
            if (delay == null)
                throw new ArgumentNullException("delay");

            IInternalAccessor source = tryit as IInternalAccessor;
            source.Delay = delay;
            return tryit;

        }

        public static ITry OnError(this ITry tryit, OnErrorDelegate onError)
        {
            var accessor = tryit as IInternalAccessor;
            accessor.OnError = onError;
            return tryit;
        }


        public static ITry OnSuccess(this ITry tryit, OnSuccessDelegate onSuccess)
        {
            var accessor = tryit as IInternalAccessor;
            accessor.OnSuccess = onSuccess;
            return tryit;
        }



        #region ThenTry extensions:

        public static ITry ThenTry(this ITry tryit, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt(retries, parent.Actor as Action);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry(this ITry tryit, Action altAction, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt(retries, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T>(this ITry tryit, T arg, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T>(retries, arg, parent.Actor as Action<T>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T>(this ITry tryit, Action<T> altAction, T arg, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T>(retries, arg, altAction);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2>(this ITry tryit, T1 arg1, T2 arg2, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2>(retries, arg1, arg2, parent.Actor as Action<T1, T2>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITry ThenTry<T1, T2>(this ITry tryit, Action<T1, T2> altAction, T1 arg1, T2 arg2, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2>(retries, arg1, arg2, altAction);
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
