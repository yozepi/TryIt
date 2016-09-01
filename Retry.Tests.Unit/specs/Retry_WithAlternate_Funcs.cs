using NSpec;
using Retry.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Runners;

namespace Retry.Tests.Unit.specs
{
    class Retry_WithAlternate_Funcs : nspec
    {
        void Retry_With_Alternate_Funcs()
        {
            FuncRetryBuilder<string> subject = null;
            Func<string> subjectFunc = null;
            int retries = 1;
            int altCalled = default(int);

            before = () =>
            {
                altCalled = default(int);
                subjectFunc = () => { return "Yeah yeah yeah."; };
            };

            act = () => subject = TryIt.Try(subjectFunc, retries);

            describe["TryIt.Try(<any>).ThenTry(altFunc<TResult>, retries)"] = () =>
            {

                FuncRetryBuilder<string> child = null;
                Func<string> altFunc = null;

                before = () =>
                {
                    altFunc = () => { altCalled++; return "SWAK!"; };
                };

                act = () =>
                {
                    child = subject.ThenTry(altFunc, retries);
                };

                it["ThenTry() should use the alternate func"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

                context["TryIt.Try(action, retries).ThenTry(altAction, retries).Go()"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () => { throw new Exception("You killed my father. Prepare to die!"); };
                    };
                    act = () =>
                    {
                        child.Go();
                    };

                    it["ThenTry() should have called the alternate action"] = () =>
                        altCalled.Should().Be(1);
                };
            };

            describe["TryIt.Try(<any>).ThenTry(altFunc<T, TResult>, retries)"] = () =>
            {

                FuncRetryBuilder<string> child = null;
                Func<int, string> altFunc = null;
                int arg = 42;

                int e = default(int);
                before = () =>
                {
                    e = default(int);
                    altCalled = default(int);
                    altFunc = (a1) => { e = a1; altCalled++; return "Hello world!"; };
                };

                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg, retries);
                };

                it["ThenTry() should use the alternate func"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

                it["should set the runner's values"] = () =>
                {
                    var runner = child.LastRunner.As<FuncRunner<int, string>>();
                    runner._arg.Should().Be(arg);
                };

                context["TryIt.Try(action, retries).ThenTry(altAction, retries).Go()"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () => { throw new Exception("You killed my father. Prepare to die!"); };
                    };
                    act = () =>
                    {
                        child.Go();
                    };

                    it["ThenTry() should have called the alternate action"] = () =>
                        altCalled.Should().Be(1);

                    it["should have passed the argument into the alt func"] = () =>
                    {
                        e.Should().Be(arg);
                    };
                };
            };

            describe["TryIt.Try(<any>).ThenTry(altFunc<T1, T2, TResult>, retries)"] = () =>
            {

                FuncRetryBuilder<string> child = null;
                Func<int, double, string> altFunc = null;
                int arg1 = 42;
                double arg2 = double.MaxValue;

                int e1 = default(int);
                double e2 = default(double);
                before = () =>
                {
                    e1 = default(int);
                    e2 = default(double);
                    altCalled = default(int);
                    altFunc = (a1, a2) => { e1 = a1; e2 = a2; altCalled++; return "Hello world!"; };
                };

                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg1, arg2, retries);
                };

                it["ThenTry() should use the alternate func"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

                it["should set the runner's values"] = () =>
                {
                    var runner = child.LastRunner.As<FuncRunner<int, double, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                };

                context["TryIt.Try(action, retries).ThenTry(altFunc<T1, T2, TResult>, retries).Go()"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () => { throw new Exception("You killed my father. Prepare to die!"); };
                    };
                    act = () =>
                    {
                        child.Go();
                    };

                    it["ThenTry() should have called the alternate action"] = () =>
                        altCalled.Should().Be(1);

                    it["should have passed the argument into the alt func"] = () =>
                    {
                        e1.Should().Be(arg1);
                        e2.Should().Be(arg2);
                    };
                };
            };

            describe["TryIt.Try(<any>).ThenTry(altFunc<T1, T2, T3, TResult>, retries)"] = () =>
            {

                FuncRetryBuilder<string> child = null;
                Func<int, double, long, string> altFunc = null;
                int arg1 = 42;
                double arg2 = double.MaxValue;
                long arg3 = long.MinValue;

                int e1 = default(int);
                double e2 = default(double);
                long e3 = default(long);
                before = () =>
                {
                    e1 = default(int);
                    e2 = default(double);
                    e3 = default(long);
                    altCalled = default(int);
                    altFunc = (a1, a2, a3) => { e1 = a1; e2 = a2; e3 = a3; altCalled++; return "Hello world!"; };
                };

                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg1, arg2, arg3, retries);
                };

                it["ThenTry() should use the alternate func"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

                it["should set the runner's values"] = () =>
                {
                    var runner = child.LastRunner.As<FuncRunner<int, double, long, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                };

                context["TryIt.Try(action, retries).ThenTry(altFunc<T1, T2, T3, TResult>, retries).Go()"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () => { throw new Exception("You killed my father. Prepare to die!"); };
                    };
                    act = () =>
                    {
                        child.Go();
                    };

                    it["ThenTry() should have called the alternate action"] = () =>
                        altCalled.Should().Be(1);

                    it["should have passed the argument into the alt func"] = () =>
                    {
                        e1.Should().Be(arg1);
                        e2.Should().Be(arg2);
                        e3.Should().Be(arg3);
                    };
                };
            };

            describe["TryIt.Try(<any>).ThenTry(altFunc<T1, T2, T3, T4, TResult>, retries)"] = () =>
            {

                FuncRetryBuilder<string> child = null;
                Func<int, double, long, double, string> altFunc = null;
                int arg1 = 42;
                double arg2 = double.MaxValue;
                long arg3 = long.MinValue;
                double arg4 = Math.PI;

                int e1 = default(int);
                double e2 = default(double);
                long e3 = default(long);
                double e4 = default(double);
                before = () =>
                {
                    e1 = default(int);
                    e2 = default(double);
                    e3 = default(long);
                    e4 = default(double);
                    altCalled = default(int);
                    altFunc = (a1, a2, a3, a4) => { e1 = a1; e2 = a2; e3 = a3; e4 = a4; altCalled++; return "Hello world!"; };
                };

                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg1, arg2, arg3, arg4, retries);
                };

                it["ThenTry() should use the alternate func"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

                it["should set the runner's values"] = () =>
                {
                    var runner = child.LastRunner.As<FuncRunner<int, double, long, double, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                    runner._arg4.Should().Be(arg4);
                };

                context["TryIt.Try(action, retries).ThenTry(altFunc<T1, T2, T3, TResult>, retries).Go()"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () => { throw new Exception("You killed my father. Prepare to die!"); };
                    };
                    act = () =>
                    {
                        child.Go();
                    };

                    it["ThenTry() should have called the alternate action"] = () =>
                        altCalled.Should().Be(1);

                    it["should have passed the argument into the alt func"] = () =>
                    {
                        e1.Should().Be(arg1);
                        e2.Should().Be(arg2);
                        e3.Should().Be(arg3);
                        e4.Should().Be(arg4);
                    };
                };
            };

        }
    }
}
