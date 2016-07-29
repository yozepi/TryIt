using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry
{
    public abstract class FuncTryItBase<TActor, TResult> : TryItBase, ITryAndReturnValue<TResult>
    {

        internal FuncTryItBase(int retries, TActor actor)
            : base(retries, actor)
        { }

        protected TResult Result { get; set; }

        protected internal bool HasResult { get; set; }

        public new TResult Go()
        {
            base.Go();
            return GetResult();
        }

        public virtual new async Task<TResult> GoAsync()
        {
            await Run();
            if (Status == RetryStatus.Fail)
            {
                throw new RetryFailedException(ExceptionList);
            }
            return GetResult();
        }

        protected override async Task ExecuteActor()
        {
            Result = default(TResult);
            HasResult = false;
            await Task.Factory.StartNew(() =>
            {
                Result = ExecuteFunc();
                HasResult = true;
            }, TaskCreationOptions.AttachedToParent);
        }

        protected abstract TResult ExecuteFunc();

        private TResult GetResult()
        {
            if (HasResult)
                return Result;

            var parent = ((IInternalAccessor)this).Parent as ITryAndReturnValue<TResult>;
            if (parent == null)
                return default(TResult);

            return parent.GetResult();
        }

        TResult ITryAndReturnValue<TResult>.GetResult()
        {
            return GetResult();
        }
    }


    public class FuncTryIt<TResult> : FuncTryItBase<Func<TResult>, TResult>
    {
        internal FuncTryIt(int retries, Func<TResult> func)
            : base(retries, func)
        { }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<TResult>;
            return func();
        }
    }

    public class FuncTryIt<T, TResult> : FuncTryItBase<Func<T, TResult>, TResult>
    {
        internal T _arg;

        internal FuncTryIt(int retries, T arg, Func<T, TResult> func)
            : base(retries, func)
        {
            _arg = arg;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T, TResult>;
            return func(_arg);
        }
    }

    public class FuncTryIt<T1, T2, TResult> : FuncTryItBase<Func<T1, T2, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, Func<T1, T2, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, TResult>;
            return func(_arg1, _arg2);
        }
    }

    public class FuncTryIt<T1, T2, T3, TResult> : FuncTryItBase<Func<T1, T2, T3, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, Func<T1, T2, T3, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, T3, TResult>;
            return func(_arg1, _arg2, _arg3);
        }
    }

    public class FuncTryIt<T1, T2, T3, T4, TResult> : FuncTryItBase<Func<T1, T2, T3, T4, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<T1, T2, T3, T4, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, T3, T4, TResult>;
            return func(_arg1, _arg2, _arg3, _arg4);
        }
    }

    public class FuncTryIt<T1, T2, T3, T4, T5, TResult> : FuncTryItBase<Func<T1, T2, T3, T4, T5, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<T1, T2, T3, T4, T5, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, T3, T4, T5, TResult>;
            return func(_arg1, _arg2, _arg3, _arg4, _arg5);
        }
    }

    public class FuncTryIt<T1, T2, T3, T4, T5, T6, TResult> : FuncTryItBase<Func<T1, T2, T3, T4, T5, T6, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<T1, T2, T3, T4, T5, T6, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, T3, T4, T5, T6, TResult>;
            return func(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6);
        }
    }

    public class FuncTryIt<T1, T2, T3, T4, T5, T6, T7, TResult> : FuncTryItBase<Func<T1, T2, T3, T4, T5, T6, T7, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;
        internal T7 _arg7;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, T3, T4, T5, T6, T7, TResult>;
            return func(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7);
        }
    }

    public class FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : FuncTryItBase<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;
        internal T7 _arg7;
        internal T8 _arg8;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
            _arg8 = arg8;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>;
            return func(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8);
        }
    }

    public class FuncTryIt<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : FuncTryItBase<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>, TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;
        internal T5 _arg5;
        internal T6 _arg6;
        internal T7 _arg7;
        internal T8 _arg8;
        internal T9 _arg9;

        internal FuncTryIt(int retries, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func)
            : base(retries, func)
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
            _arg5 = arg5;
            _arg6 = arg6;
            _arg7 = arg7;
            _arg8 = arg8;
            _arg9 = arg9;
        }

        protected override TResult ExecuteFunc()
        {
            var func = Actor as Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>;
            return func(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7, _arg8, _arg9);
        }
    }
}
