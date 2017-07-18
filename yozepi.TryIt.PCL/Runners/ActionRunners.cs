using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Runners
{
    internal class ActionRunner : BaseSyncRunner
    {


        protected internal override void ExecuteActor()
        {
            var action = Actor as Action;
            action();
        }

        protected internal override void HandleSuccessPolicy(int count)
        {
            (SuccessPolicy as SuccessPolicyDelegate)?.Invoke(count);
        }

    }

}
