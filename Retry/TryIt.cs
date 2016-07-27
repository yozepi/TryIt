using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public static class TryIt
    {

        #region Action methods:

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

        #endregion //Action methods:


        #region Func methods:

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

        #endregion //Func methods:

    }
}
