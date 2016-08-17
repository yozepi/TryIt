using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public abstract class TaskTryItBase : TryItBase
    {

        protected TaskTryItBase(int retries, Delegate actor)
            : base(retries, actor)
        { }

        protected abstract Func<Task> GetFunc();

        protected override Task ExecuteActor()
        {

            var func = GetFunc();
            var task = func();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

            return task;
        }


        protected override void HandleOnSuccess(int count)
        {
            var accessor = ParentOrSelf((x) => x.OnSuccess != null);
            if (accessor != null)
            {
                (accessor.OnSuccess as OnSuccessDelegate)?.Invoke(count);
            }
        }
    }


    public class TaskTryIt : TaskTryItBase
    {
        internal TaskTryIt(int retries, Delegate func)
            : base(retries, func)
        { }

        protected override Func<Task> GetFunc()
        {
            return (Func<Task>)Actor;
        }
    }


    public abstract class TaskTryItAndReturnResultBase<TResult> : FuncTryItBase<TResult>
    {
        protected TaskTryItAndReturnResultBase(int retries, Delegate func)
            : base(retries, func)
        { }

        protected override Task<TResult> RunTask()
        {
            var task = GetTask();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

            return task;
        }

        protected abstract Task<TResult> GetTask();

        protected override TResult ExecuteFunc()
        {
            throw new NotImplementedException();
        }
    }

    public class TaskTryItAndReturnResult<TResult> : TaskTryItAndReturnResultBase<TResult>
    {
        internal TaskTryItAndReturnResult(int retries, Func<Task<TResult>> func)
            : base(retries, func)
        { }

        protected override Task<TResult> GetTask()
        {
            var actor = Actor as Func<Task<TResult>>;
            return actor();
        }
    }

    public class TaskTryItAndReturnResult<T, TResult> : TaskTryItAndReturnResultBase<TResult>
    {
        internal T _arg;

        internal TaskTryItAndReturnResult(int retries, T arg, Func<T, Task<TResult>> func)
        : base(retries, func)
        {
            _arg = arg;
        }

        protected override Task<TResult> GetTask()
        {
            var actor = Actor as Func<T, Task<TResult>>;
            return actor(_arg);
        }
    }
}
