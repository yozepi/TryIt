using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using yozepi.Retry.Builders;
using yozepi.Retry.Delays;
using yozepi.Retry.Exceptions;
using yozepi.Retry.Runners;

namespace yozepi.Retry
{
    public static partial class TryIt
    {

        public static MethodRetryBuilder<T> Try<T>(Func<T> func, int retries) 
        {
            VerifyNotTask<T>();
            return new MethodRetryBuilder<T>()
                .AddRunner(new FuncRunner<T>())
                .SetActor(func)
                .SetRetryCount(retries) as MethodRetryBuilder<T>;
        }


        public static MethodRetryBuilder<T> ThenTry<T>(this MethodRetryBuilder<T> builder, int retries)
        {
            BaseRunner runner = new FuncRunner<T>();

            return builder
                .AddRunner(runner)
                .SetRetryCount(retries) as MethodRetryBuilder<T>;
        }


        public static MethodRetryBuilder<T> ThenTry<T>(this MethodRetryBuilder<T> builder, Func<T> func, int retries)
        {
            return builder
                .AddRunner(new FuncRunner<T>())
                .SetActor(func)
                .SetRetryCount(retries) as MethodRetryBuilder<T>;
        }


        #region UsingDelay, WithErrorPolicy, WithSuccessPolicy

        public static MethodRetryBuilder<T> UsingDelay<T>(this MethodRetryBuilder<T> builder, IDelay delay)
        {
            return builder
                .SetDelay(delay) as MethodRetryBuilder<T>;

        }

        public static MethodRetryBuilder<T> WithErrorPolicy<T>(this MethodRetryBuilder<T> builder, ErrorPolicyDelegate errorPolicy)
        {
            return builder
                .SetErrorPolicy(errorPolicy) as MethodRetryBuilder<T>;
        }

        public static MethodRetryBuilder<T> WithSuccessPolicy<T>(this MethodRetryBuilder<T> builder, SuccessPolicyDelegate<T> successPolicy)
        {
            return builder
                .SetSuccessPolicy(successPolicy) as MethodRetryBuilder<T>;
        }

        #endregion //UsingDelay, WithErrorPolicy, WithSuccessPolicy


        private static TypeInfo TaskInfo = typeof(Task).GetTypeInfo();
        internal const string TaskErrorMessage = "The Try method does not support Tasks. Use TryAsync instead";

        private static void VerifyNotTask<T>()
        {
            var TResultTypeInfo = typeof(T).GetTypeInfo();

            if (TResultTypeInfo.IsAssignableFrom(TaskInfo)
                || TResultTypeInfo.IsSubclassOf(typeof(Task)))
            {
                throw new TaskNotAllowedException(TaskErrorMessage);
            }
        }
    }
}
