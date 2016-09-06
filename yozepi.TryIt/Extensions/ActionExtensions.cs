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
    }
}
