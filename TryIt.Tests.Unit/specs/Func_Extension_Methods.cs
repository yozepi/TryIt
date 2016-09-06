using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Builders;
using Retry;

namespace TryIt.Tests.Unit.specs
{
    class Func_Extension_Methods : nspec
    {
        void Func_extensions()
        {
            BaseBuilder subject = null;
            int retries = 5;

            before = () => subject = null;

            describe["Func<Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<bool> func = () => { return true; };
                    subject = func.Try(retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<string, bool> func = (s) => { return true; };
                    subject = func.Try("hello", retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T1, T2, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, bool> func = (i1, i2) => { return true; };
                    subject = func.Try(1, 2, retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T1, T2, T3, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, bool> func = (i1, i2, i3) => { return true; };
                    subject = func.Try(1, 2, 3, retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T1, T2, T3, T4, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, bool> func = (i1, i2, i3, i4) => { return true; };
                    subject = func.Try(1, 2, 3, 4, retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<Task>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<Task> func = () => new Task(() => { });
                    subject = func.Try(retries);
                };
                it["Should return a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Func<T, Task>.Try(T, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, Task> func = (i) => new Task(() => { });
                    subject = func.Try(1, retries);
                };
                it["Should return a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Func<T1, T2, Task>.Try(T1, T2, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, Task> func = (i1, i2) => new Task(() => { });
                    subject = func.Try(1, 2, retries);
                };
                it["Should return a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Func<T1, T2, T3, Task>.Try(T1, T2, T3, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, Task> func = (i1, i2, i3) => new Task(() => { });
                    subject = func.Try(1, 2, 3, retries);
                };
                it["Should return a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Func<T1, T2, T3, T4, Task>.Try(T1, T2, T3, T4, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, Task> func = (i1, i2, i3, i4) => new Task(() => { });
                    subject = func.Try(1, 2, 3, 4, retries);
                };
                it["Should return a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Func<Task<TResult>>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<Task<bool>> func = () => new Task<bool>(() => { return true; });
                    subject = func.Try(retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T, Task<TResult>>.Try(T, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, Task<bool>> func = (i1) => new Task<bool>(() => { return true; });
                    subject = func.Try(1, retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T1, T2, Task<TResult>>.Try(T1, T2, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, Task<bool>> func = (i1, i2) => new Task<bool>(() => { return true; });
                    subject = func.Try(1, 2, retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T1, T2, T3, Task<TResult>>.Try(T1, T2, T3, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, Task<bool>> func = (i1, i2, i3) => new Task<bool>(() => { return true; });
                    subject = func.Try(1, 2, 3, retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<T1, T2, T3, T4, Task<TResult>>.Try(T1, T2, T3, T4, retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, Task<bool>> func = (i1, i2, i3, i4) => new Task<bool>(() => { return true; });
                    subject = func.Try(1, 2, 3, 4, retries);
                };
                it["Should return a FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

        }
    }
}
