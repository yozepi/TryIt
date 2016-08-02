using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public abstract class TryItBase : ITry, IInternalAccessor
    {

        #region instance properties and methods:

        #region constructors

        protected TryItBase(int retries, object actor)
        {
            if (retries < 1)
                throw new ArgumentOutOfRangeException("times", retries, "Value must be greater than zero (0).");

            if (actor == null)
                throw new ArgumentNullException("actor");

            RetryCount = retries;
            Actor = actor;

            Delay = Retry.Delay.DefaultDelay;
            ExceptionList = new List<Exception>();
        }

        #endregion //constructors

        public int RetryCount { get; private set; }

        public int Attempts { get; private set; }

        public Delay Delay { get; private set; }

        public List<Exception> ExceptionList { get; private set; }

        public RetryStatus Status { get; private set; }

        protected object Actor { get; private set; }

        async protected virtual Task Run()
        {
            Attempts = 0;
            var accessor = this as IInternalAccessor;
            var parent = accessor.Parent;

            accessor.ExceptionList.Clear();

            if (parent != null)
            {
                await parent.Run();
                accessor.ExceptionList.AddRange(parent.ExceptionList);
                if (parent.Status != RetryStatus.Fail)
                {
                    accessor.Status = parent.Status;
                    return;
                }
            }
            accessor.Status = RetryStatus.Running;

            for (int count = 0; count < accessor.RetryCount; count++)
            {
                try
                {
                    Attempts++;
                    await ExecuteActor();
                    if (count == 0)
                    {
                        if (parent != null && parent.Status == RetryStatus.Fail)
                        {
                            accessor.Status = RetryStatus.SuccessAfterRetries;
                        }
                        else
                        {
                            accessor.Status = RetryStatus.Success;
                        }
                    }
                    else
                    {
                        accessor.Status = RetryStatus.SuccessAfterRetries;
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (HandleOnError(_onError, ex, count))
                    {
                        accessor.ExceptionList.Add(ex);
                        await accessor.Delay.WaitAsync(count);
                    }
                    else
                    {
                        accessor.Status = RetryStatus.Fail;
                        throw;
                    }
                }
            }

            if (accessor.Status == RetryStatus.Running)
            {
                accessor.Status = RetryStatus.Fail;
            }

            return;

        }


        public void Go()
        {
            try
            {
                var task = Run();
                task.Wait();
                if (Status == RetryStatus.Fail)
                {
                    throw new RetryFailedException(ExceptionList);
                }
            }
            catch (AggregateException ex)
            {

                throw ex.InnerException;
            }
        }

        public async Task GoAsync()
        {
            await Run();
            if (Status == RetryStatus.Fail)
            {
                throw new RetryFailedException(ExceptionList);
            }
        }

        protected abstract Task ExecuteActor();

        protected abstract bool HandleOnError(Delegate onError, Exception ex, int retryCount);


        private Delegate _onError = null;

        #region IInternalAccessor explicit:

        IInternalAccessor IInternalAccessor.Parent { get; set; }
        RetryStatus IInternalAccessor.Status
        {
            get { return this.Status; }
            set { this.Status = value; }
        }

        IDelay IInternalAccessor.Delay
        {
            get { return this.Delay; }
            set { this.Delay = (Delay)value; }
        }

        int IInternalAccessor.RetryCount { get { return this.RetryCount; } }

        List<Exception> IInternalAccessor.ExceptionList { get { return this.ExceptionList; } }


        object IInternalAccessor.Actor { get { return this.Actor; } }

        async Task IInternalAccessor.Run() { await this.Run(); }

        Delegate IInternalAccessor.OnError
        {
            get { return _onError; }
            set { _onError = value; }
        }
        #endregion //IInternalAccessor explicit:

        #endregion //instance properties and methods:
    }
}
