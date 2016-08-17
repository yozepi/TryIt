using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    /// <summary>
    /// Pass an instance of this delegate to the <see cref="TryIt.OnError(ITry, OnErrorDelegate)"/>
    /// and the <see cref="TryIt.OnError{TResult}(ITryAndReturnValue{TResult}, OnErrorDelegate)"/> extension methods.
    /// </summary>
    /// <param name="ex">The exception that has been caught.</param>
    /// <param name="attemptCount">A count of the execution attempts so far.</param>
    /// <returns>Returning true will cause TryIt.Try (or ThenTry) to continue.
    /// Returning false will stop retrying and raise the exception. </returns>
    public delegate bool OnErrorDelegate(Exception ex, int attemptCount);

    /// <summary>
    /// Pass an instance of this deletate to the <see cref="TryIt.OnSuccess(ITry, OnSuccessDelegate)" /> extension method.
    /// </summary>
    /// <param name="attemptCount">A count of the execution attempts so far.</param>
    public delegate void OnSuccessDelegate(int attemptCount);

    /// <summary>
    /// Pass an instance of this deletate to the
    /// <see cref="TryIt.OnSuccess{TResult}(ITryAndReturnValue{TResult}, OnSuccessDelegate{TResult})" /> extension method.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned upon a succesful try.</typeparam>
    /// <param name="attemptCount">A count of the execution attempts so far.</param>
    /// <param name="result">The result of the succesfull attempt.</param>
    public delegate void OnSuccessDelegate<TResult>(int attemptCount, TResult result);

    /// <summary>
    /// Represents the state of the TryIt outcome.
    /// </summary>
    public enum RetryStatus
    {
        /// <summary>
        /// Go() or GoAsync() has not yet been called on the instance.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Go() or GoAsync() has been called and TryIt is processing.
        /// </summary>
        Running,

        /// <summary>
        /// The Tryit worked the first time without having to retry or execute ThenTry().
        /// </summary>
        Success,

        /// <summary>
        /// TryIt succedded either directly after multiple attempts or indirectly after ThenTry() succeeds.
        /// </summary>
        SuccessAfterRetries,

        /// <summary>
        /// All attempts to Try() (or ThenTry()) failed.
        /// </summary>
        Fail
    }


    /// <summary>
    /// Represents the response to TryIt.Try() that executes an action.
    /// </summary>
    public interface ITry
    {
        /// <summary>
        /// The count of times to try the action.
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        /// The count of attempts made before success (or failure).
        /// </summary>
        int Attempts { get; }

        /// <summary>
        /// The delay strategy used between tries.
        /// </summary>
        IDelay Delay { get; }

        /// <summary>
        /// A list of exceptions accumulated for each failed try <c>of this instance.</c>.
        /// </summary>
        List<Exception> ExceptionList { get; }

        /// <summary>
        /// The status of the try.
        /// </summary>
        RetryStatus Status { get; }

        /// <summary>
        /// Start trying to execute the action.
        /// </summary>
        /// <exception cref="RetryFailedException">Thrown when every Try-Retry in the chain fails.</exception>
        /// <remarks>Go() blocks while trying the action. 
        /// Actually the action is executed as an async task but processing is halted while that task executes.
        /// <para>You must execute Go() on the last instance of the Try-ThenTry chain or you get unexpected results.</para>
        /// </remarks>
        void Go();

        /// <summary>
        /// Start trying to execute the action.
        /// </summary>
        /// <returns>Returns an awaitable task that is executing the Try-ThenTry chain.</returns>
        /// <exception cref="RetryFailedException">Thrown when every Try-Retry in the chain fails.</exception>
        /// <remarks>You must execute Go() on the last instance of the Try-ThenTry chain or you get unexpected results.</remarks>
        Task GoAsync();

        /// <summary>
        /// Returns a list of all exceptions in the entire Try-ThenTry chain.
        /// </summary>
        /// <returns></returns>
        List<Exception> GetAllExceptions();

        /// <summary>
        /// Returns a <see cref="LinkedList{ITry}"/> of all ITry instance that make up the Try-ThenTry chain.
        /// </summary>
        /// <returns></returns>
        LinkedList<ITry> GetChain();
    }


    /// <summary>
    /// Extends <see cref="ITry"/> to represent TryIt().Try() that executes a Func.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the Func.</typeparam>
    public interface ITryAndReturnValue<TResult> : ITry
    {
        /// <summary>
        /// Start trying to execute the action.
        /// </summary>
        /// <returns>Returns the result of the first successful Func execution.
        /// <para>
        /// If there is an OnSuccess() delegate policy in place, the result represents the first 
        /// instance of a succesfull execution that meets the OnSuccess policy.
        /// </para>
        /// </returns>
        /// <exception cref="RetryFailedException">Thrown when every Try-Retry in the chain fails.</exception>
        /// <remarks>Go() blocks while trying the Func. 
        /// Actually the Func is executed as an async task but processing is halted while that task executes.
        /// <para>You must execute Go() on the last instance of the Try-ThenTry chain or you get unexpected results.</para>
        ///</remarks>
        new TResult Go();

        /// <summary>
        /// Start trying to execute the action.
        /// </summary>
        /// <returns>Returns an awaitable <see cref="Task{TResult}"/>that is executing the Try-ThenTry chain.</returns>
        /// <remarks>You must execute Go() on the last instance of the Try-ThenTry chain or you get unexpected results.</remarks>
        new Task<TResult> GoAsync();

    }

    internal interface IInternalAccessor
    {
        IInternalAccessor Parent { get; set; }
        IDelay Delay { get; set; }
        Delegate Actor { get; }
        OnErrorDelegate OnError { get; set; }
        Delegate OnSuccess { get; set; }
    }
}