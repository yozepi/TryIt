using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class ErrorPolicyException : Exception
    {
        internal ErrorPolicyException(Exception innerException)
            : base("Error policy delegate rejected the exception and is halting the runner. The inner exception contains the violating exception.", innerException)
        { }
    }
}
