using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal class TaskWithResultRunner<TResult> : FuncRunner<TResult>
    {
        public TaskWithResultRunner() : base() { }

        protected internal override async Task ExecuteActorAsync(CancellationToken cancelationToken)
        {
            var task = GetTask();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

           Result =  await task;
        }

        protected virtual Task<TResult> GetTask()
        {
            var actor = Actor as Func<Task<TResult>>;
            return actor();

        }
    }

    internal class TaskWithResultRunner<T, TResult>: TaskWithResultRunner<TResult>
    {
        internal T _arg;


        public TaskWithResultRunner(T arg) 
            :base()
        {
            _arg = arg;
        }

        protected override Task<TResult> GetTask()
        {
            var actor = Actor as Func<T, Task<TResult>>;
            return actor(_arg);
        }
    }

    internal class TaskWithResultRunner<T1, T2, TResult> : TaskWithResultRunner<TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;

        public TaskWithResultRunner(T1 arg1, T2 arg2)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        protected override Task<TResult> GetTask()
        {
            var actor = Actor as Func<T1, T2, Task<TResult>>;
            return actor(_arg1, _arg2);
        }
    }

    internal class TaskWithResultRunner<T1, T2, T3, TResult> : TaskWithResultRunner<TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;

        public TaskWithResultRunner(T1 arg1, T2 arg2, T3 arg3)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        protected override Task<TResult> GetTask()
        {
            var actor = Actor as Func<T1, T2, T3, Task<TResult>>;
            return actor(_arg1, _arg2, _arg3);
        }
    }

    internal class TaskWithResultRunner<T1, T2, T3, T4, TResult> : TaskWithResultRunner<TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;

        public TaskWithResultRunner(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        protected override Task<TResult> GetTask()
        {
            var actor = Actor as Func<T1, T2, T3, T4, Task<TResult>>;
            return actor(_arg1, _arg2, _arg3, _arg4);
        }
    }

}
