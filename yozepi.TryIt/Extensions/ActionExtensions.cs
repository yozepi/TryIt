using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry.Builders;

namespace Retry
{
    public static class ActionExtensions
    {
        public static ActionRetryBuilder Try(this Action action, int retries)
        {
            return TryIt.Try(action, retries);
        }

        public static ActionRetryBuilder Try<T>(this Action<T> action, T arg, int retries)
        {
            return TryIt.Try(action, arg, retries);
        }

        public static ActionRetryBuilder Try<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2, int retries)
        {
            return TryIt.Try(action, arg1, arg2, retries);
        }

        public static ActionRetryBuilder Try<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, int retries)
        {
            return TryIt.Try(action, arg1, arg2, arg3, retries);
        }

        public static ActionRetryBuilder Try<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            return TryIt.Try(action, arg1, arg2, arg3, arg4, retries);
        }

        //public static ITry Try<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, int retries)
        //{
        //    return TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, retries);
        //}

        //public static ITry Try<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, int retries)
        //{
        //    return TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, retries);
        //}

        //public static ITry Try<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, int retries)
        //{
        //    return TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, retries);
        //}

        //public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, int retries)
        //{
        //    return TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, retries);
        //}

        //public static ITry Try<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, int retries)
        //{
        //    return TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, retries);
        //}
    }
}
