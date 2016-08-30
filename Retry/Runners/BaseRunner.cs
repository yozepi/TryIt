using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry.Runners
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

        public RetryStatus Status { get; private set; }
        public List<Exception> ExceptionList { get; private set; }

        public IDelay Delay { get; set; }

        public OnErrorDelegate OnError { get; set; }

        public Delegate OnSuccess { get; set; }

        public async Task RunAsync()
        {
            Attempts = 0;
            ExceptionList.Clear();
            Status = RetryStatus.Running;

            for (int count = 0; count < RetryCount; count++)
            {

                try
                {
                    Attempts++;
                    await ExecuteActorAsync();
                    HandleOnSuccess(Attempts);
                    if (count == 0)
                    {
                        Status = RetryStatus.Success;
                    }
                    else
                    {
                        Status = RetryStatus.SuccessAfterRetries;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (HandleOnError(ex, count))
                    {
                        ExceptionList.Add(ex);

                        //Only wait if count hasn't ended.
                        if (count + 1 < RetryCount)
                        {
                            IDelay delay;
                            if (Delay != null)
                                delay = Delay;
                            else
                                delay = Retry.Delay.DefaultDelay;

                            await delay.WaitAsync(count);
                        }
                    }
                    else
                    {
                        Status = RetryStatus.Fail;
                        throw;
                    }
                }
            }

            if (Status == RetryStatus.Running)
            {
                //still running after all attempts - FAIL!
                Status = RetryStatus.Fail;
            }

            return;
        }

        /// <summary>
        /// Implementors extend this method to execute the Func/Action.
        /// </summary>
        /// <returns></returns>
        protected abstract Task ExecuteActorAsync();

        /// <summary>
        /// Implementors execute this action to handle success policy calls.
        /// </summary>
        /// <param name="count"></param>
        protected abstract void HandleOnSuccess(int count);

        private bool HandleOnError(Exception ex, int retryCount)
        {
            if (OnError == null)
                return true;
            return OnError(ex, retryCount);
        }

        internal virtual void CopySettings(BaseRunner targetRunner)
        {
            targetRunner.Delay = Delay;
            targetRunner.OnError = OnError;
            targetRunner.OnSuccess = OnSuccess;
            targetRunner.RetryCount = RetryCount;
            targetRunner.Actor = Actor;
        }
    }
}
