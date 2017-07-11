using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retry.Builders;

namespace Retry
{
    public static class ActionExtensions
    {
        public static ActionRetryBuilder Try(this Action action, int retries)
        {
            return TryIt.Try(action, retries);
        }

    }
}
