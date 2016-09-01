using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public class OnErrorPolicyException : Exception
    {
        internal OnErrorPolicyException(Exception innerException)
            : base("OnError delegate rejected the exception and is halting the runner. The inner exception is the violating exception.", innerException)
        { }
    }
}
