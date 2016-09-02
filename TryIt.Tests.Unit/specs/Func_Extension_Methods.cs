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
            FuncRetryBuilder<bool> subject = null;
            int retries = 5;

            before = () => subject = null;

            describe["Func<Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<bool> func = () => { return true; };
                    subject = func.Try(retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<string, bool> func = (s) => { return true; };
                    subject = func.Try("hello", retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, bool> func = (i1, i2) => { return true; };
                    subject = func.Try(1, 2, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, T3, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, bool> func = (i1, i2, i3) => { return true; };
                    subject = func.Try(1, 2, 3, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, T3, T4, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, bool> func = (i1, i2, i3, i4) => { return true; };
                    subject = func.Try(1, 2, 3, 4, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };
        }
    }
}
