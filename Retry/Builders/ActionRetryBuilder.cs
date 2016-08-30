using Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Run();
        }


        /// <summary>
        /// Runs your action as an awaitable task.
        /// </summary>
        /// <returns></returns>
        public async Task GoAsync()
        {
            await RunAsync();
        }

        #endregion //instance properties and methods:
    }
}
