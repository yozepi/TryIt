using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal interface IRunnerArgSource
    {
        object[] RunnerArgs { get; }
    }
}
