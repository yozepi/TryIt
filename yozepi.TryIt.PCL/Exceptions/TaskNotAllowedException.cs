using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yozepi.Retry.Exceptions
{
    public class TaskNotAllowedException: InvalidOperationException
    {
        internal TaskNotAllowedException(string message)
            :base(message)
        { }
    }
}
