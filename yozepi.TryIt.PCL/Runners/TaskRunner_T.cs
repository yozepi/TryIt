using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Runners
{
    internal class TaskRunner<T> : BaseAsyncRunner
    {
        public TaskRunner() : base() { }

        internal T Result { get; set; }

        protected internal override async Task ExecuteActorAsync(CancellationToken cancelationToken)
        {
            var actor = Actor as Func<Task<T>>;

            var task = actor();
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

           Result =  await task;
        }


        protected override internal void HandleSuccessPolicy(int count)
        {
            (SuccessPolicy as SuccessPolicyDelegate<T>)?.Invoke(Result, count);
        }

    }
}
