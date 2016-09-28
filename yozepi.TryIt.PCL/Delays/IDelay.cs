using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Delays
{
    /// <summary>
    /// Implementors of this interface provide Delay strategies for Tryit().UsingDelay().
    /// </summary>
    public interface IDelay
    {
        /// <summary>
        /// Implementors provide an async await task that pauses according to their delay strategy.
        /// </summary>
        /// <param name="attemptCount">The number of times TryIt has attempted to execute.</param>
        /// <returns>A task that pauses according to the strategy of the implementor.</returns>
        /// <remarks>
        /// Don't waste your time trying to implement this interface. Instead, extend the abstract <see cref="Delay"/> class.
        /// </remarks>
        Task WaitAsync(int attemptCount, CancellationToken cancelationToken);
    }
}
