using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public delegate bool OnErrorDelegate(Exception ex, int retryCount);
    public delegate bool OnSuccessDelegate<TResult>(Exception ex, int retryCount, TResult result);
    public delegate bool OnSuccessDelegate(Exception ex, int retryCount);

    public enum RetryStatus
    {
        NotStarted,
        Running,
        Success,
        SuccessAfterRetries,
        Fail
    }

    public interface ITry
    {
        int RetryCount { get; }
        int Attempts { get; }
        Delay Delay { get; }
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
        int RetryCount { get; }
        IDelay Delay { get; set; }
        List<Exception> ExceptionList { get; }
        RetryStatus Status { get; set; }
        object Actor { get; }

        Delegate OnError { get; set; }
        Task Run();
    }
}