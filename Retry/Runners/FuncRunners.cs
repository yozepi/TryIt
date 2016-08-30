﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry.Runners
{
    internal class FuncRunner<TResult> : BaseRunner
    {

        internal TResult Result { get; set; }


        protected override async Task ExecuteActorAsync()
        {
            Result = default(TResult);
            Result = await RunTaskAsync();
        }

        protected virtual async Task<TResult> RunTaskAsync()
        {
            return await Task<TResult>.Factory.StartNew(() =>
           {
               return ExecuteFunc();
           }, TaskCreationOptions.AttachedToParent);
        }

        protected virtual TResult ExecuteFunc()
        {
            var func = Actor as Func<TResult>;
            return func();
        }

        protected override void HandleOnSuccess(int count)
        {
            (OnSuccess as OnSuccessDelegate<TResult>)?.Invoke(count, Result);
        }
    }


    internal class FuncRunner<T, TResult> : FuncRunner<TResult>
    {
        internal T _arg;

        public FuncRunner(T arg)
            : base()
        {
            _arg = arg;
        }

        protected override TResult ExecuteFunc()
        {
            var actor = Actor as Func<T, TResult>;
            return actor(_arg);
        }
    }


    internal class FuncRunner<T1, T2, TResult> : FuncRunner<TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;

        public FuncRunner(T1 arg1, T2 arg2)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
        }

        protected override TResult ExecuteFunc()
        {
            var actor = Actor as Func<T1, T2, TResult>;
            return actor(_arg1, _arg2);
        }
    }


    internal class FuncRunner<T1, T2, T3, TResult> : FuncRunner<TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;

        public FuncRunner(T1 arg1, T2 arg2, T3 arg3)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        protected override TResult ExecuteFunc()
        {
            var actor = Actor as Func<T1, T2, T3, TResult>;
            return actor(_arg1, _arg2, _arg3);
        }
    }


    internal class FuncRunner<T1, T2, T3, T4, TResult> : FuncRunner<TResult>
    {
        internal T1 _arg1;
        internal T2 _arg2;
        internal T3 _arg3;
        internal T4 _arg4;

        public FuncRunner(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            : base()
        {
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        protected override TResult ExecuteFunc()
        {
            var actor = Actor as Func<T1, T2, T3, T4, TResult>;
            return actor(_arg1, _arg2, _arg3, _arg4);
        }
    }
}
