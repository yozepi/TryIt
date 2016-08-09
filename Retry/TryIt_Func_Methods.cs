using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    partial class TryIt
    {


        #region Try methods:

        public static ITryAndReturnValue<TResult> Try<TResult>(Func<TResult> func, int retries)
        {
            return new FuncTryIt<TResult>(retries, func);
        }

        public static ITryAndReturnValue<TResult> Try<T, TResult>(Func<T, TResult> func, T arg, int retries)
        {
            return new FuncTryIt<T, TResult>(retries, arg, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int retries)
        {
            return new FuncTryIt<T1, T2, TResult>(retries, arg1, arg2, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return new FuncTryIt<T1, T2, T3, TResult>(retries, arg1, arg2, arg3, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return new FuncTryIt<T1, T2, T3, T4, TResult>(retries, arg1, arg2, arg3, arg4, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        {
            return new FuncTryIt<T1, T2, T3, T4, T5, TResult>(retries, arg1, arg2, arg3, arg4, arg5, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        {
            return new FuncTryIt<T1, T2, T3, T4, T5, T6, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        {
            return new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        {
            return new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, func);
        }

        public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        {
            return new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, func);
        }

        #endregion //Try methods:


        public static ITryAndReturnValue<TResult> UsingDelay<TResult>(this ITryAndReturnValue<TResult> tryit, IDelay delay)
        {
            if (delay == null)
                throw new ArgumentNullException("delay");

            IInternalAccessor source = tryit as IInternalAccessor;
            source.Delay = delay;
            return tryit;

        }

        public static ITryAndReturnValue<TResult> OnError<TResult>(this ITryAndReturnValue<TResult> tryit, OnErrorDelegate onError)
        {
            var accessor = tryit as IInternalAccessor;
            accessor.OnError = onError;
            return tryit;
        }

        public static ITryAndReturnValue<TResult> OnSuccess<TResult>(this ITryAndReturnValue<TResult> tryit, OnSuccessDelegate<TResult> onSuccess)
        {
            var accessor = tryit as IInternalAccessor;
            accessor.OnSuccess = onSuccess;
            return tryit;
        }


        #region ThenTry extensions:

        public static ITryAndReturnValue<TResult> ThenTry<TResult>(this ITryAndReturnValue<TResult> tryit, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<TResult>(retries, parent.Actor as Func<TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T, TResult>(this ITryAndReturnValue<TResult> tryit, T arg, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T, TResult>(retries, arg, parent.Actor as Func<T, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, TResult>(retries, arg1, arg2, parent.Actor as Func<T1, T2, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, T3, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, T3, TResult>(retries, arg1, arg2, arg3, parent.Actor as Func<T1, T2, T3, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, T3, T4, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, T3, T4, TResult>(retries, arg1, arg2, arg3, arg4, parent.Actor as Func<T1, T2, T3, T4, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, T3, T4, T5, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, T3, T4, T5, TResult>(retries, arg1, arg2, arg3, arg4, arg5, parent.Actor as Func<T1, T2, T3, T4, T5, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, T3, T4, T5, T6, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, T3, T4, T5, T6, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, parent.Actor as Func<T1, T2, T3, T4, T5, T6, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, T3, T4, T5, T6, T7, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, parent.Actor as Func<T1, T2, T3, T4, T5, T6, T7, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, parent.Actor as Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        public static ITryAndReturnValue<TResult> ThenTry<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this ITryAndReturnValue<TResult> tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(retries, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, parent.Actor as Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>);
            ((IInternalAccessor)child).Parent = parent;
            return child;
        }

        #endregion //ThenTry extensions:

    }
}
