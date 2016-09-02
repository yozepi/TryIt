using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry.Builders;

namespace Retry
{
    public static class FuncExcensions
    {

        public static FuncRetryBuilder<TResult> Try<TResult>(this Func<TResult> func, int retries)
        {
            return TryIt.Try(func, retries);
        }


        public static FuncRetryBuilder<TResult> Try<T, TResult>(this Func<T, TResult> func, T arg, int retries)
        {
            return TryIt.Try(func, arg, retries);
        }


        public static FuncRetryBuilder<TResult> Try<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2, int retries)
        {
            return TryIt.Try(func, arg1, arg2, retries);
        }


        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return TryIt.Try(func, arg1, arg2, arg3, retries);
        }

        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return TryIt.Try(func, arg1, arg2, arg3, arg4, retries);
        }

        //public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        //{
        //    return TryIt.Try(func, arg1, arg2, arg3, arg4, arg5, retries);
        //}

        //public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        //{
        //    return TryIt.Try(func, arg1, arg2, arg3, arg4, arg5, arg6, retries);
        //}

        //public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        //{
        //    return TryIt.Try(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, retries);
        //}

        //public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        //{
        //    return TryIt.Try(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, retries);
        //}

        //public static ITryAndReturnValue<TResult> Try<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        //{
        //    return TryIt.Try(func, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, retries);
        //}
    }
}
