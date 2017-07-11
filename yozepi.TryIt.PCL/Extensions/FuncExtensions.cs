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


        public static ActionRetryBuilder Try(this Func<Task> func, int retries)
        {
            return TryIt.TryTask(func, retries);
        }


        public static FuncRetryBuilder<TResult> Try<TResult>(this Func<Task<TResult>> func, int retries)
        {
            return TryIt.TryTask(func, retries);
        }


        public static FuncRetryBuilder<TResult> Try<T, TResult>(this Func<T, Task<TResult>> func,T arg, int retries)
        {
            return TryIt.TryTask(func, arg, retries);
        }


        public static FuncRetryBuilder<TResult> Try<T1, T2, TResult>(this Func<T1, T2, Task<TResult>> func, T1 arg1, T2 arg2, int retries)
        {
            return TryIt.TryTask(func, arg1, arg2, retries);
        }


        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, TResult>(this Func<T1, T2, T3, Task<TResult>> func, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return TryIt.TryTask(func, arg1, arg2, arg3, retries);
        }


        public static FuncRetryBuilder<TResult> Try<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, Task<TResult>> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return TryIt.TryTask(func, arg1, arg2, arg3, arg4, retries);
        }


    }
}
