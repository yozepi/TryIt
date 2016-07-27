using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public static class FuncExcensions
    {

        public static ITryAndReturnValue<TResult> Try<TResult>(this Func<TResult> func, int retries)
        {
            return TryIt.Try(func, retries);
        }


        public static ITryAndReturnValue<TResult> Try<T, TResult>(this Func<T, TResult> func, T arg, int retries)
        {
            return TryIt.Try(func, arg, retries);
        }
    }
}
