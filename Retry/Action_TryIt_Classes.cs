using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class ActionTryIt : TryItBase
    {
        internal ActionTryIt(int retries, Action action)
           : base(retries, action)
        {
        }

        protected ActionTryIt(int retries, object action)
         : base(retries, action)
        {
        }

        async protected override Task ExecuteActor()
        {
            await Task.Factory.StartNew(() =>
           {
               var action = GetAction();
               action();
           }, TaskCreationOptions.AttachedToParent);
        }

        protected virtual Action GetAction()
        {
            return Actor as Action;
        }

        protected override bool HandleOnError(Delegate onError, Exception ex, int retryCount)
        {
            var theDelegate = onError as OnErrorDelegate;
            if (theDelegate == null)
                return true;

            return theDelegate(ex, retryCount);
        }
    }

    public class ActionTryIt<T> : ActionTryIt
    {
        internal T _arg;
        internal ActionTryIt(int retries, T arg, Action<T> action)
           : base(retries, action)
        {
            _arg = arg;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T>;
            return () => action(_arg);
        }
      }

    public class ActionTryIt<T1, T2> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, Action<T1, T2> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2>;
            return () => action(_arg1, _arg2);
        }
    }

    public class ActionTryIt<T1, T2, T3> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, Action<T1, T2, T3> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3>;
            return () => action(_arg1, _arg2, _arg3);
        }
    }

    public class ActionTryIt<T1, T2, T3, T4> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<T1, T2, T3, T4> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3, T4>;
            return () => action(_arg1, _arg2, _arg3, _arg4);
        }
    }

    public class ActionTryIt<T1, T2, T3, T4, T5> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Action<T1, T2, T3, T4, T5> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3, T4, T5>;
            return () => action(_arg1, _arg2, _arg3, _arg4, _arg5);
        }
    }

    public class ActionTryIt<T1, T2, T3, T4, T5, T6> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Action<T1, T2, T3, T4, T5, T6> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3, T4, T5, T6>;
            return () => action(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6);
        }
    }

    public class ActionTryIt<T1, T2, T3, T4, T5, T6, T7> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;
        internal T7 _arg7;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Action<T1, T2, T3, T4, T5, T6, T7> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3, T4, T5, T6, T7>;
            return () => action(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7);
        }
    }

    public class ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;
        internal T7 _arg7;
        internal T8 _arg8;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
            _arg8 = arg8;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3, T4, T5, T6, T7, T8>;
            return () => action(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8);
        }
    }

    public class ActionTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ActionTryIt
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;
        internal T7 _arg7;
        internal T8 _arg8;
        internal T9 _arg9;

        internal ActionTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        : base(retries, action)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
            _arg8 = arg8;
            _arg9 = arg9;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>;
            return () => action(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8, _arg9);
        }
    }
}
