using yozepi.Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Builders
{
    public abstract class BaseSyncBuilder: BaseBuilder
    {


        protected internal virtual void Run(CancellationToken cancellationToken)
        {
            Status = RetryStatus.Running;
            var runningStatus = RetryStatus.Running;

            Attempts = 0;
            Winner = null;
            ExceptionList.Clear();


            var runnerLink = Runners.First;
            try
            {
                while (runnerLink != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    var runner = runnerLink.Value as BaseSyncRunner;
                    runner.Run(cancellationToken);
                    Attempts += runner.Attempts;
                    ExceptionList.AddRange(runner.ExceptionList);

                    if (runner.Status == RetryStatus.Success)
                    {
                        runningStatus = runningStatus == RetryStatus.Fail ?
                            RetryStatus.SuccessAfterRetries : RetryStatus.Success;
                    }
                    else
                    {
                        runningStatus = runner.Status;
                    }


                    if (runningStatus == RetryStatus.Success
                        || runningStatus == RetryStatus.SuccessAfterRetries)
                    {
                        Winner = runner;
                        break;
                    }
                    runnerLink = runnerLink.Next;
                }
            }

            catch (OperationCanceledException)
            {
                Status = RetryStatus.Canceled;
                throw;
            }
            catch (Exception)
            {
                Status = RetryStatus.Fail;
                throw;
            }

            Status = runningStatus;
            if (Status == RetryStatus.Fail)
            {
                throw new RetryFailedException(new List<Exception>(ExceptionList));
            }
        }
    }
}
