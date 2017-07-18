using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Runners
{
    internal class FuncRunner<TResult> : BaseSyncRunner
    {

        internal TResult Result { get; set; }


        protected internal override void ExecuteActor()
        {
            Result = default(TResult);
            var func = Actor as Func<TResult>;
            Result = func();
        }

        protected override internal void HandleSuccessPolicy(int count)
        {
            (SuccessPolicy as SuccessPolicyDelegate<TResult>)?.Invoke(Result, count);
        }
    }
}
