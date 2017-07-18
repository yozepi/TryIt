using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace yozepi.Retry.Builders
{
    public class MethodRetryBuilder : BaseSyncBuilder
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


        #endregion //instance properties and methods:
    }
}
