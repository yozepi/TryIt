using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public static class ITryAndReturnValueExtensions
    {
        public static ITryAndReturnValue<TResult> UsingDelay<TResult>(this ITryAndReturnValue<TResult> tryit, IDelay delay)
        {
            if (delay == null)
                throw new ArgumentNullException("delay");

            IInternalAccessor source = tryit as IInternalAccessor;
            source.Delay = delay;
            return tryit;

        }

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

    }
}
