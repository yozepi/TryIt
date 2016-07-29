using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
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
        int RetryCount { get; }
        IDelay Delay { get; set; }
        List<Exception> ExceptionList { get; }
        RetryStatus Status { get; set; }
        object Actor { get; }

        Task Run();
    }
}