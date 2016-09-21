using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal static class RunnerFactory
    {
        static public BaseRunner GetRunner(BaseRunner runner, params object[] arguments)
        {
            var runnerType = runner.GetType();

            object[] args = arguments;

            if (args.Length == 0)
            {
                var asArgSource = runner as IRunnerArgSource;
                if (asArgSource != null)
                {
                    args = asArgSource.RunnerArgs;
                }
            }
            return Activator.CreateInstance(runnerType, args) as BaseRunner;
        }

    }
}
