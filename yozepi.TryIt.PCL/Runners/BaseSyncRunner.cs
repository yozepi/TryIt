using yozepi.Retry.Delays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Runners
{
    internal abstract class BaseSyncRunner : BaseRunner
    {
        public void Run(CancellationToken cancellationToken)
        {
            Attempts = 0;
            ExceptionList.Clear();
            Status = RetryStatus.Running;


            try
            {
                for (int count = 0; count < RetryCount; count++)
                {

                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    try
                    {
                        Attempts++;
                        ExecuteActor();
                        HandleSuccessPolicy(Attempts);
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
                    catch (OperationCanceledException)
                    {
                        throw;
                    }

                    catch (Exception ex)
                    {
                        if (HandleErrorPolicy(ex, count))
                        {
                            ExceptionList.Add(ex);

                            //Only wait if count hasn't ended.
                            if (count + 1 < RetryCount)
                            {
                                IDelay delay;
                                if (Delay != null)
                                    delay = Delay;
                                else
                                    delay = Delays.Delay.DefaultDelay;

                                delay.WaitAsync(count, cancellationToken).Wait(cancellationToken);
                            }
                        }
                        else
                        {
                            ExceptionList.Add(new ErrorPolicyException(ex));
                            Status = RetryStatus.Fail;
                            break;
                        }
                    }
                }

                if (Status == RetryStatus.Running)
                {
                    //still running after all attempts - FAIL!
                    Status = RetryStatus.Fail;
                }

            }
            catch (OperationCanceledException)
            {
                Status = RetryStatus.Canceled;
                throw;
            }

            catch (Exception ex)
            {
                ExceptionList.Add(ex);
                Status = RetryStatus.Fail;
                throw;
            }
            return;

        }

        protected internal abstract void ExecuteActor();
    }
}
