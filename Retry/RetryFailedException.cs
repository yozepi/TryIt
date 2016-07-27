using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class RetryFailedException: Exception
    {
        public IList<Exception> ExceptionList { get; private set; }
        internal RetryFailedException(IList<Exception> exceptions)
            : base("All attempts failed to execute. See the ExceptionList for the list of exceptions for each attempt.")
        {
            ExceptionList = exceptions;
        }
    }
}
