﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yozepi.Retry
{
    /// <summary>
    /// Pass an instance of this delegate to the 
    /// <see cref="TryIt.WithErrorPolicy{TResult}(Builders.MethodRetryBuilder{TResult}, ErrorPolicyDelegate)"/> extension methods.
    /// </summary>
    /// <param name="ex">The exception that has been caught.</param>
    /// <param name="attemptCount">A count of the execution attempts so far.</param>
    /// <returns>Returning true will cause TryIt.Try (or ThenTry) to continue.
    /// Returning false will stop retrying and raise the exception. </returns>
    public delegate bool ErrorPolicyDelegate(Exception ex, int attemptCount);

    /// <summary>
    /// Pass an instance of this deletate to the <see cref="TryIt.WithSuccessPolicy(Builders.MethodRetryBuilder, SuccessPolicyDelegate)" /> extension method.
    /// </summary>
    /// <param name="attemptCount">A count of the execution attempts so far.</param>
    public delegate void SuccessPolicyDelegate(int attemptCount);

    /// <summary>
    /// Pass an instance of this deletate to the
    /// <see cref="TryIt.WithErrorPolicy{TResult}(Builders.MethodRetryBuilder{TResult}, ErrorPolicyDelegate)" /> extension method.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned upon a succesful try.</typeparam>
    /// <param name="result">The result of the succesfull attempt.</param>
    /// <param name="attemptCount">A count of the execution attempts so far.</param>
    public delegate void SuccessPolicyDelegate<TResult>(TResult result, int attemptCount);

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
        Fail,

        /// <summary>
        /// The Task running GoAsync() was cancelled.
        /// </summary>
        Canceled

    }
}
