using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry;
using Retry.Builders;
using Retry.Delays;
using Retry.Runners;

namespace Retry
{
    /// <summary>
    /// Here's where the magic begins! Use TryIt's various static methods and extensions
    /// to Try and Re-Try your actions and methods.
    /// </summary>
    public static partial class TryIt
    {

        #region Try methods:

        /// <summary>
        /// Try the provided <see cref="Action"/> the specified number of times
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to try.</param>
        /// <param name="retries">The number of times the action will be tried before giving up and throwing a <see cref="RetryFailedException"/>.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when The action parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when retries is less than 1.</exception>
        public static ActionRetryBuilder Try(Action action, int retries)
        {
            return new ActionRetryBuilder()
                .AddRunner(new ActionRunner())
                .SetActor(action)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }


        #endregion //Try methods:


        #region Try Task methods:

        public static ActionRetryBuilder TryTask(Func<Task> func, int retries)
        {

            return new ActionRetryBuilder()
                .AddRunner(new TaskRunner())
                .SetActor(func)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }


        #endregion //Try Task methods:


        #region ThenTry extensions:

        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, int retries)
        {
            BaseRunner runner = RunnerFactory.GetRunner(builder.LastRunner);
            builder.AddRunner(runner);
            builder.SetRetryCount(retries);
            return builder;
        }


        #endregion //ThenTry extensions:


        #region ThenTry Alternate Action extensions:

        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, Action altAction, int retries)
        {
            return builder
                .AddRunner(new ActionRunner())
                .SetActor(altAction)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        #endregion //ThenTry Alternate Action extensions:


        #region ThenTry Alternate Task extensions:

        public static ActionRetryBuilder ThenTry(this ActionRetryBuilder builder, Func<Task> altFunc, int retries)
        {
            return builder
                .AddRunner(new TaskRunner())
                .SetActor(altFunc)
                .SetRetryCount(retries) as ActionRetryBuilder;
        }

        #endregion //ThenTry Alternate Task extensions:
    }
}
