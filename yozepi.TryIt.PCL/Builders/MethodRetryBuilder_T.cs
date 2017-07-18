using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry.Runners;
using System.Threading;
using yozepi.Retry.Exceptions;
using System.Reflection;
namespace yozepi.Retry.Builders
{
    public class MethodRetryBuilder<TResult> : BaseSyncBuilder
    {
        internal const string TaskErrorMessage = "A Task is not allowed to be used as a result in this context. try using a TaskRetryBuilder (or a TaskRetryBuilder<T> if your task returns a result) instead";

        private static TypeInfo TaskInfo = typeof(Task).GetTypeInfo();
        public MethodRetryBuilder()
        {
            var TResultTypeInfo = typeof(TResult).GetTypeInfo();

            if (TResultTypeInfo.IsAssignableFrom(TaskInfo)
                || TResultTypeInfo.IsSubclassOf(typeof(Task)))
            {
                throw new TaskNotAllowedException(TaskErrorMessage);
            }
        }

        public TResult Go()
        {
            return Go(CancellationToken.None);
        }

        public TResult Go(CancellationToken cancellationToken)
        {
            Run(cancellationToken);
            return (Winner as FuncRunner<TResult>).Result;
        }
    }
}
