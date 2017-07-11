using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal class FuncRunner<TResult> : BaseRunner
    {

        internal TResult Result { get; set; }


        protected internal override async Task ExecuteActorAsync(CancellationToken cancelationToken)
        {
            Result = default(TResult);
            Result = await RunTaskAsync(cancelationToken);
        }

        protected internal virtual async Task<TResult> RunTaskAsync(CancellationToken cancelationToken)
        {
            return await Task<TResult>.Run(() =>
            {
                return ExecuteFunc();
            }, cancelationToken);
        }

        protected internal virtual TResult ExecuteFunc()
        {
            var func = Actor as Func<TResult>;
            return func();
        }

        protected override internal void HandleSuccessPolicy(int count)
        {
            (SuccessPolicy as SuccessPolicyDelegate<TResult>)?.Invoke(Result, count);
        }
    }
}
