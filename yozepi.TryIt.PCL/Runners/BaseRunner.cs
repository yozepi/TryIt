using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry.Delays;
using System.Threading;

namespace yozepi.Retry.Runners
{
    internal abstract class BaseRunner
    {

        public BaseRunner()
        {
            ExceptionList = new List<Exception>();
        }

        /// <summary>
        /// Contains the Action or Func that will be executed.
        /// </summary>
        /// <remarks>Ineritors cast this property to the apropriate value in the <see cref="ExecuteActorAsync"/> method</remarks>
        public Delegate Actor { get; set; }

        public int RetryCount { get; set; }

        public int Attempts { get; set; }

        public RetryStatus Status { get; protected set; }

        public List<Exception> ExceptionList { get; private set; }

        public IDelay Delay { get; set; }

        public ErrorPolicyDelegate ErrorPolicy { get; set; }

        public Delegate SuccessPolicy { get; set; }

        /// <summary>
        /// Implementors execute this action to handle success policy calls.
        /// </summary>
        /// <param name="count"></param>
        protected internal abstract void HandleSuccessPolicy(int count);

        protected internal bool HandleErrorPolicy(Exception ex, int retryCount)
        {
            if (ErrorPolicy == null)
                return true;
            return ErrorPolicy(ex, retryCount);
        }

        internal virtual void CopySettings(BaseRunner targetRunner)
        {
            targetRunner.Delay = Delay;
            targetRunner.ErrorPolicy = ErrorPolicy;
            targetRunner.SuccessPolicy = SuccessPolicy;
            targetRunner.RetryCount = RetryCount;
            targetRunner.Actor = Actor;
        }
    }
}
