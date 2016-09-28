using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal class TaskRunner : FuncRunner<Task>
    {
        public TaskRunner() : base() { }

        protected internal override async Task ExecuteActorAsync(CancellationToken cancelationToken)
        {
            var task = GetTask();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

            await task;
        }

        protected internal virtual Task GetTask()
        {
            var actor = Actor as Func<Task>;
            return actor();

        }

        protected internal override void HandleSuccessPolicy(int count)
        {
            (SuccessPolicy as SuccessPolicyDelegate)?.Invoke(count);
        }
    }


    internal class TaskRunner<T> : TaskRunner, IRunnerArgSource
    {
        internal T _arg;

        public TaskRunner(T arg)
            : base()
        {
            _arg = arg;
        }

        object[] IRunnerArgSource.RunnerArgs
        {
            get { return new object[] { _arg }; }
        }

        protected internal override Task GetTask()
        {
            var actor = Actor as Func<T, Task>;
            return actor(_arg);
        }
    }


    internal class TaskRunner<T1, T2> : TaskRunner, IRunnerArgSource
    {
        internal T1 _arg1;
        internal T2 _arg2;

        public TaskRunner(T1 arg1, T2 arg2)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        object[] IRunnerArgSource.RunnerArgs
        {
            get { return new object[] { _arg1, _arg2 }; }
        }

        protected internal override Task GetTask()
        {
            var actor = Actor as Func<T1, T2, Task>;
            return actor(_arg1, _arg2);
        }
    }


    internal class TaskRunner<T1, T2, T3> : TaskRunner, IRunnerArgSource
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;

        public TaskRunner(T1 arg1, T2 arg2, T3 arg3)
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

        protected override internal Task GetTask()
        {
            var actor = Actor as Func<T1, T2, T3, Task>;
            return actor(_arg1, _arg2, _arg3);
        }
    }


    internal class TaskRunner<T1, T2, T3, T4> : TaskRunner, IRunnerArgSource
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;

        public TaskRunner(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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

        protected override internal Task GetTask()
        {
            var actor = Actor as Func<T1, T2, T3, T4, Task>;
            return actor(_arg1, _arg2, _arg3, _arg4);
        }
    }

}

