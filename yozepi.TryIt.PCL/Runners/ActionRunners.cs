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

}
