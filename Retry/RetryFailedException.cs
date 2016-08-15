using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    /// <summary>
    /// This exception is thrown when every attempt in the Try-ThenTry chain fails.
    /// </summary>
    public class RetryFailedException: Exception
    {
        /// <summary>
        /// A list of all the exceptions that have occurred in the chain.
        /// </summary>
        public IList<Exception> ExceptionList { get; private set; }
        internal RetryFailedException(IList<Exception> exceptions)
            : base("All attempts failed to execute. See the ExceptionList for the list of exceptions for each attempt.")
        {
            ExceptionList = exceptions;
        }
    }
}
