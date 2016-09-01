using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal class ActionRunner : BaseRunner
    {
 
        protected override async Task ExecuteActorAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                var action = GetAction();
                action();
            }, TaskCreationOptions.AttachedToParent);
        }

        protected override void HandleSuccessPolicy(int count)
        {
            if (SuccessPolicy != null)
            {
                (SuccessPolicy as SuccessPolicyDelegate)?.Invoke(count);
            }
        }

        protected virtual Action GetAction()
        {
            return Actor as Action;
        }

    }

    internal class ActionRunner<T> : ActionRunner
    {
        internal T _arg;

        public ActionRunner(T arg)
            :base()
        {
            _arg = arg;
        }

        protected override Action GetAction()
        {
            var action = Actor as Action<T>;
            return () => action(_arg);
        }
    }

    internal class ActionRunner<T1, T2> : ActionRunner
    {
        internal T1 _arg1;
        internal T2 _arg2;

        public ActionRunner(T1 arg1, T2 arg2)
            : base()
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

    internal class ActionRunner<T1, T2, T3> : ActionRunner
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;

        public ActionRunner(T1 arg1, T2 arg2, T3 arg3)
            : base()
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

    internal class ActionRunner<T1, T2, T3, T4> : ActionRunner
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;

        public ActionRunner(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            : base()
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

}
