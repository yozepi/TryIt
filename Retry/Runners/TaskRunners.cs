using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal class TaskRunner : FuncRunner<Task>
    {
        public TaskRunner() : base() { }

        protected override async Task ExecuteActorAsync()
        {
            var task = GetTask();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

            await task;
        }

        protected  virtual Task GetTask()
        {
            var actor = Actor as Func<Task>;
            return actor();

        }

        protected override void HandleOnSuccess(int count)
        {
            (OnSuccess as OnSuccessDelegate)?.Invoke(count);
        }
    }


    internal class TaskRunner<T> : TaskRunner
    {
        internal T _arg;

        public TaskRunner(T arg)
            : base()
        {
            _arg = arg;
        }

        protected override Task GetTask()
        {
            var actor = Actor as Func<T, Task>;
            return actor(_arg);
        }
    }


    internal class TaskRunner<T1, T2> : TaskRunner
    {
        internal T1 _arg1;
        internal T2 _arg2;

        public TaskRunner(T1 arg1, T2 arg2)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        protected override Task GetTask()
        {
            var actor = Actor as Func<T1, T2, Task>;
            return actor(_arg1, _arg2);
        }
    }


    internal class TaskRunner<T1, T2, T3> : TaskRunner
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

        protected override Task GetTask()
        {
            var actor = Actor as Func<T1, T2, T3, Task>;
            return actor(_arg1, _arg2, _arg3);
        }
    }


    internal class TaskRunner<T1, T2, T3, T4> : TaskRunner
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

        protected override Task GetTask()
        {
            var actor = Actor as Func<T1, T2, T3, T4, Task>;
            return actor(_arg1, _arg2, _arg3, _arg4);
        }
    }

}

