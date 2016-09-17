using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal class ActionRunner : BaseRunner
    {
 
        protected internal override async Task ExecuteActorAsync(CancellationToken cancelationToken)
        {
            await Task.Run(() =>
            {
                var action = GetAction();
                action();
            }, cancelationToken);
        }

        protected internal override void HandleSuccessPolicy(int count)
        {
            if (SuccessPolicy != null)
            {
                (SuccessPolicy as SuccessPolicyDelegate)?.Invoke(count);
            }
        }

        protected internal virtual Action GetAction()
        {
            return Actor as Action;
        }

    }

    internal class ActionRunner<T> : ActionRunner, IRunnerArgSource
    {
        internal T _arg;

        public ActionRunner(T arg)
            :base()
        {
            _arg = arg;
        }

        object[] IRunnerArgSource.RunnerArgs
        {
            get { return new object[] { _arg }; }
        }

        protected internal override Action GetAction()
        {
            var action = Actor as Action<T>;
            return () => action(_arg);
        }
    }

    internal class ActionRunner<T1, T2> : ActionRunner, IRunnerArgSource
    {
        internal T1 _arg1;
        internal T2 _arg2;

        public ActionRunner(T1 arg1, T2 arg2)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        object[] IRunnerArgSource.RunnerArgs
        {
            get { return new object[] { _arg1, _arg2 }; }
        }

        protected internal override Action GetAction()
        {
            var action = Actor as Action<T1, T2>;
            return () => action(_arg1, _arg2);
        }
    }

    internal class ActionRunner<T1, T2, T3> : ActionRunner, IRunnerArgSource
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

        object[] IRunnerArgSource.RunnerArgs
        {
            get { return new object[] { _arg1, _arg2, _arg3 }; }
        }

        protected internal override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3>;
            return () => action(_arg1, _arg2, _arg3);
        }
    }

    internal class ActionRunner<T1, T2, T3, T4> : ActionRunner, IRunnerArgSource
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

        object[] IRunnerArgSource.RunnerArgs
        {
            get { return new object[] { _arg1, _arg2, _arg3, _arg4 }; }
        }

        protected internal override Action GetAction()
        {
            var action = Actor as Action<T1, T2, T3, T4>;
            return () => action(_arg1, _arg2, _arg3, _arg4);
        }
    }

}
