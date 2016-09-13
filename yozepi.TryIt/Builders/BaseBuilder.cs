using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry.Delays;
using Retry.Runners;
using System.Threading;

namespace Retry.Builders
{
    public abstract class BaseBuilder
    {

        #region constructors

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        protected BaseBuilder()
        {
            ExceptionList = new List<Exception>();
            Runners = new LinkedList<BaseRunner>();

        }

        #endregion //constructors

        /// <summary>
        /// An integer containing the actual number of times the Action/Func has been attempted.
        /// <remarks>This value is local to this instance of the TryIt chain.</remarks>
        /// </summary>
        public int Attempts { get; private set; }

        /// <summary>
        /// The list of exceptions that have occurred while trying the Action/Func
        /// </summary>
        public List<Exception> ExceptionList { get; private set; }

        /// <summary>
        /// The accumulated status of the TryIt chain.
        /// </summary>
        public RetryStatus Status { get; protected set; }


        internal LinkedList<BaseRunner> Runners { get; private set; }

        internal BaseRunner LastRunner { get; set; }

        internal BaseRunner Winner { get; set; }

        protected internal void Run()
        {
            var awaiter = RunAsync(CancellationToken.None).GetAwaiter();
            awaiter.GetResult();
        }

        protected internal void Run(CancellationToken cancellationToken)
        {
            var awaiter = RunAsync(cancellationToken).GetAwaiter();
            awaiter.GetResult();
        }

        protected internal async Task RunAsync(CancellationToken cancellationToken)
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

                    var runner = runnerLink.Value;
                    await runner.RunAsync(cancellationToken);
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


        internal BaseBuilder AddRunner(BaseRunner runner)
        {
            BaseRunner lastRunner = Runners.Last?.Value;
            if (lastRunner != null)
            {
                lastRunner.CopySettings(runner);
            }

            Runners.AddLast(runner);
            LastRunner = runner;

            return this;
        }

        internal BaseBuilder SetActor(Delegate actor)
        {
            if (actor == null)
                throw new ArgumentNullException("actor");
            LastRunner.Actor = actor;
            return this;
        }

        internal BaseBuilder SetDelay(IDelay delay)
        {
            LastRunner.Delay = delay;
            return this;
        }

        internal BaseBuilder SetErrorPolicy(ErrorPolicyDelegate errorPolicyDelegate)
        {
            LastRunner.ErrorPolicy = errorPolicyDelegate;
            return this;
        }

        internal BaseBuilder SetSuccessPolicy(Delegate successPolicyDelegate)
        {
            LastRunner.SuccessPolicy = successPolicyDelegate;
            return this;
        }

        internal BaseBuilder SetRetryCount(int retries)
        {
            if (retries < 1)
                throw new ArgumentOutOfRangeException("retries", retries, "Value must be 1 or greater.");

            LastRunner.RetryCount = retries;
            return this;
        }

    }
}
