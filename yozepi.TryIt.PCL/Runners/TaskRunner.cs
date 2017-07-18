using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Runners
{
    internal class TaskRunner : BaseAsyncRunner
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

}

