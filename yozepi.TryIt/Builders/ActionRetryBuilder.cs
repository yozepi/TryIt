using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry.Builders
{
    /// <summary>
    /// TryItBase is the base class for the Action and Func based TryIt classes. Most of the real work takes place here.
    /// </summary>
    public class ActionRetryBuilder : BaseBuilder
    {

        #region instance properties and methods:

        /// <summary>
        /// Runs your action and retries if the action fails.
        /// </summary>
        public void Go()
        {
            Go(CancellationToken.None);
        }

        /// <summary>
        /// Runs your action and retries if the action fails.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Go(CancellationToken cancellationToken)
        {
            Run(cancellationToken);
        }

        /// <summary>
        /// Runs your action as an awaitable task.
        /// </summary>
        /// <returns></returns>
        public Task GoAsync()
        {
            return GoAsync(CancellationToken.None);
        }

        /// <summary>
        /// Runs your action as an awaitable task.
        /// </summary>
        /// <returns></returns>
        public async Task GoAsync(CancellationToken cancellationToken)
        {
            await RunAsync(cancellationToken);
        }

        #endregion //instance properties and methods:
    }
}
