using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    /// <summary>
    /// TryItBase is the base class for the Action and Func based TryIt classes. Most of the real work takes place here.
    /// </summary>
    public abstract class TryItBase : ITry, IInternalAccessor
    {

        #region instance properties and methods:

        #region constructors

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="retries"></param>
        /// <param name="actor"></param>
        protected TryItBase(int retries, System.Delegate actor)
        {
            if (retries < 1)
                throw new ArgumentOutOfRangeException("retries", retries, "Value must be greater than zero (0).");

            if (actor == null)
                throw new ArgumentNullException("actor");

            RetryCount = retries;
            Actor = actor;
            ExceptionList = new List<Exception>();
        }

        #endregion //constructors

        /// <summary>
        /// An integer containing the maximum number of times the Action/Func will be attempted.
        /// </summary>
        /// <remarks>This value is local to this instance of the TryIt chain.</remarks>
        public int RetryCount { get; private set; }

        /// <summary>
        /// An integer containing the actual number of times the Action/Func has been attempted.
        /// <remarks>This value is local to this instance of the TryIt chain.</remarks>
        /// </summary>
        public int Attempts { get; private set; }

        /// <summary>
        /// An IDelay used to calculate the delay between attempts. 
        /// </summary>
        /// <remarks>This value is local to this instance of the TryIt chain.</remarks>
        public IDelay Delay { get; private set; }

        /// <summary>
        /// The list of exceptions that have occurred while trying the Action/Func
        /// </summary>
        public List<Exception> ExceptionList { get; private set; }

        /// <summary>
        /// The accumulated status of the TryIt chain.
        /// </summary>
        public RetryStatus Status { get; private set; }

        /// <summary>
        /// Contains the Action or Func that will be executed.
        /// </summary>
        /// <remarks>Ineritors cast this property to the apropriate value in the <see cref="ExecuteActor"/> method</remarks>
        protected Delegate Actor { get; private set; }


        /// <summary>
        /// Runs the TryIt chain starting at this instance.
        /// </summary>
        /// <returns>Returns a task containing the running chain.</returns>
        async protected Task Run()
        {
            try
            {

                Attempts = 0;
                ExceptionList.Clear();

                if (_parent != null)
                {
                    await _parent.Run();
                    if (_parent.Status != RetryStatus.Fail)
                    {
                        Status = _parent.Status;
                        return;
                    }
                }
                Status = RetryStatus.Running;

                for (int count = 0; count < RetryCount; count++)
                {
                    try
                    {
                        Attempts++;
                        await ExecuteActor();
                        HandleOnSuccess(Attempts);
                        if (count == 0)
                        {
                            if (_parent != null && _parent.Status == RetryStatus.Fail)
                            {
                                Status = RetryStatus.SuccessAfterRetries;
                            }
                            else
                            {
                                Status = RetryStatus.Success;
                            }
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
                                var delaySource = ParentOrSelf((x) => x.Delay != null);
                                IDelay delay;
                                if (delaySource != null)
                                    delay = delaySource.Delay;
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
                    Status = RetryStatus.Fail;
                }

                return;
            }
            catch (Exception)
            {
                Status = RetryStatus.Fail;
                throw;
            }

        }


        /// <summary>
        /// Runs your action and retries if the action fails.
        /// </summary>
        public void Go()
        {
            try
            {
                var task = Run();
                task.Wait();
                if (Status == RetryStatus.Fail)
                {
                    throw new RetryFailedException(GetAllExceptions());
                }
            }
            catch (AggregateException ex)
            {

                throw ex.InnerException;
            }
        }


        /// <summary>
        /// Runs your action as an awaitable task.
        /// </summary>
        /// <returns></returns>
        public async Task GoAsync()
        {
            await Run();
            if (Status == RetryStatus.Fail)
            {
                throw new RetryFailedException(GetAllExceptions());
            }
        }


        /// <summary>
        /// Returns the list of all exceptions up to this point in the Try-ThenTry chain.
        /// </summary>
        /// <returns></returns>
        public List<Exception> GetAllExceptions()
        {
            List<Exception> result = new List<Exception>();
            foreach (var item in GetChain())
            {
                result.AddRange(item.ExceptionList);
            }

            return result;
        }


        /// <summary>
        /// Returns a linked list of all instances of ITry chained to this instance.
        /// </summary>
        /// <returns></returns>
        public LinkedList<ITry> GetChain()
        {
            LinkedList<ITry> result = new LinkedList<ITry>();
            ITry item = null;
            while(item != this)
            {
                item = (ITry)ParentOrSelf(i => i.Parent == item);
                if (item != this)
                {
                    result.AddLast(item);
                }
            }
            result.AddLast(this);

            return result;
        }

        /// <summary>
        /// Implementors extend this method to execute the Func/Action.
        /// </summary>
        /// <returns></returns>
        protected abstract Task ExecuteActor();

        /// <summary>
        /// Implementors execute this action to handle success policy calls.
        /// </summary>
        /// <param name="count"></param>
        protected abstract void HandleOnSuccess(int count);

        private bool HandleOnError(Exception ex, int retryCount)
        {
            var src = ParentOrSelf((x) => x.OnError != null);
            if (src == null)
                return true;

            return src.OnError(ex, retryCount);
        }



        internal IInternalAccessor ParentOrSelf(Predicate<IInternalAccessor> predicate)
        {

            if (predicate(this))
            {
                return this;
            }

            if (_parent != null)
            {
                return _parent.ParentOrSelf(predicate);
            }
            return null;
        }

        #region IInternalAccessor explicit:

        private TryItBase _parent;
        IInternalAccessor IInternalAccessor.Parent
        {
            get { return _parent; }
            set { _parent = value as TryItBase; }
        }

        IDelay IInternalAccessor.Delay
        {
            get { return this.Delay; }
            set { this.Delay = value; }
        }


        Delegate IInternalAccessor.Actor { get { return Actor; } }

        private OnErrorDelegate _onError = null;
        OnErrorDelegate IInternalAccessor.OnError
        {
            get { return _onError; }
            set { _onError = value; }
        }

        Delegate _onSuccess = null;
        Delegate IInternalAccessor.OnSuccess
        {
            get { return _onSuccess; }
            set { _onSuccess = value; }
        }

        #endregion //IInternalAccessor explicit:

        #endregion //instance properties and methods:
    }
}
