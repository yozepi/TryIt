using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Retry.Tests.Unit.specs
{
    class Func_Extension_Methods : nspec
    {
        void Static_Func_Extension_Methods()
        {
            ITryAndReturnValue<bool> subject = null;
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

            describe["Func<T1, T2, T3, T5, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, bool> func = (i1, i2, i3, i4) => { return true; };
                    subject = func.Try(1, 2, 3, 4, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, T3, T4, T5, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, int, bool> func = (i1, i2, i3, i4, i5) => { return true; };
                    subject = func.Try(1, 2, 3, 4, 5, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, T3, T4, T5, T6, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, int, int, bool> func = (i1, i2, i3, i4, i5, i6) => { return true; };
                    subject = func.Try(1, 2, 3, 4, 5, 6, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, T3, T4, T5, T6, T7, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, int, int, int, bool> func = (i1, i2, i3, i4, i5, i6, i7) => { return true; };
                    subject = func.Try(1, 2, 3, 4, 5, 6, 7, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, T3, T4, T5, T6, T7, T8, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, int, int, int, int, bool> func = (i1, i2, i3, i4, i5, i6, i7, i8) => { return true; };
                    subject = func.Try(1, 2, 3, 4, 5, 6, 7, 8, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };

            describe["Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Func<int, int, int, int, int, int, int, int, int, bool> func = (i1, i2, i3, i4, i5, i6, i7, i8, i9) => { return true; };
                    subject = func.Try(1, 2, 3, 4, 5, 6, 7, 8, 9, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };
        }
    }
}
