using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public delegate bool OnErrorDelegate(Exception ex, int retryCount);
    public delegate void OnSuccessDelegate(int retryCount);
    public delegate void OnSuccessDelegate<TResult>(int retryCount, TResult result);

    public enum RetryStatus
    {
        NotStarted,
        Running,
        Success,
        SuccessAfterRetries,
        AllAttemptsRejected,
        Fail
    }

    public interface ITry
    {
        int RetryCount { get; }
        int Attempts { get; }
        IDelay Delay { get; }
        List<Exception> ExceptionList { get; }
        RetryStatus Status { get; }

        void Go();
        Task GoAsync();
    }

    public interface ITryAndReturnValue<TResult> : ITry
    {
        new TResult Go();
        new Task<TResult> GoAsync();
        TResult GetResult();
    }

    internal interface IInternalAccessor
    {
        IInternalAccessor Parent { get; set; }
        IDelay Delay { get; set; }
        object Actor { get; }
        OnErrorDelegate OnError { get; set; }
        Delegate OnSuccess { get; set; }
    }
}