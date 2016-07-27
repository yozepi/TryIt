using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public static class ITryItExtensions
    {
        public static ITry UsingDelay(this ITry tryit, IDelay delay)
        {
            if (delay == null)
                throw new ArgumentNullException("delay");

            IInternalAccessor source = tryit as IInternalAccessor;
            source.Delay = delay;
            return tryit;

        }

        #region Action extensions:

        public static ITry ThenTry(this ITry tryit, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt(retries, parent.Actor as Action);
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

        public static ITry ThenTry<T1, T2>(this ITry tryit, T1 arg1, T2 arg2, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2>(retries, arg1, arg2, parent.Actor as Action<T1, T2>);
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

        public static ITry ThenTry<T1, T2, T3, T4>(this ITry tryit, T1 arg1, T2 arg2, T3 arg3, T4 arg4, int retries)
        {
            IInternalAccessor parent = tryit as IInternalAccessor;
            var child = new ActionTryIt<T1, T2, T3, T4>(retries, arg1, arg2, arg3, arg4, parent.Actor as Action<T1, T2, T3, T4>);
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

        #endregion //Action extensions:

    }
}
