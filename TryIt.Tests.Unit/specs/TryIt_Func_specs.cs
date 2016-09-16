using Moq;
using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Builders;
using Retry.Runners;
using Retry;
using Retry.Delays;

namespace TryIt.Tests.Unit.specs
{
    class TryIt_Func_specs : nspec
    {
        void Try_Method()
        {
            describe["with 0 parameters"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                Func<string> actor = () => { return "Try to take over the world!"; };
                int retries = 5;

                act = () => subject = Retry.TryIt.Try(actor, retries);

                it["should return a FuncRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["with 1 parameter"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                Func<int, string> actor = (p1) => { return "What are we going to do tomorrow?"; };
                int retries = 5;
                int e1 = 42;

                act = () => subject = Retry.TryIt.Try(actor, e1, retries);

                it["should return a ActionRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, string>>();
                    runner._arg.Should().Be(e1);
                };


            };

            describe["with 2 parameters"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                Func<int, long, string> actor = (p1, p2) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MinValue;

                act = () => subject = Retry.TryIt.Try(actor, e1, e2, retries);

                it["should return a ActionRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                };


            };

            describe["with 3 parameters"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                Func<int, long, double, string> actor = (p1, p2, p3) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MinValue;
                double e3 = Math.PI;

                act = () => subject = Retry.TryIt.Try(actor, e1, e2, e3, retries);

                it["should return a ActionRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, double, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                };

            };

            describe["with 4 parameters"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                Func<int, long, double, bool, string> actor = (p1, p2, p3, p4) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MinValue;
                double e3 = Math.PI;
                bool e4 = true;

                act = () => subject = Retry.TryIt.Try(actor, e1, e2, e3, e4, retries);

                it["should return a ActionRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, double, bool, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                    runner._arg4.Should().Be(e4);
                };


            };
        }

        void ThenTry_Method()
        {
            describe["with 0 arguments"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Try to take over the world!"; };
                int retries = 5;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<string>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

            };

            describe["with 1 argument"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<int, string> actor = (p1) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1, 1);
                act = () => subject = sourceBuilder.ThenTry(e1, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T, TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, string>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, string>>();
                    runner._arg.Should().Be(e1);
                };
            };

            describe["with 2 arguments"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<int, long, string> actor = (p1, p2) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1, 2, 1);
                act = () => subject = sourceBuilder.ThenTry(e1, e2, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T1, T2, TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, long, string>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                };
            };

            describe["with 3 arguments"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<int, long, double, string> actor = (p1, p2, p3) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;
                double e3 = Math.PI;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1, 2, 3, 1);
                act = () => subject = sourceBuilder.ThenTry(e1, e2, e3, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T1, T2, T3, TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, long, double, string>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal arguments"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, double, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                };
            };

            describe["with 4 arguments"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<int, long, double, bool, string> actor = (p1, p2, p3, p4) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;
                double e3 = Math.PI;
                bool e4 = true;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1, 2, 3, false, 1);
                act = () => subject = sourceBuilder.ThenTry(e1, e2, e3, e4, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T1, T2, T3, T4, TResilt>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, long, double, bool, string>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal arguments"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, double, bool, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                    runner._arg4.Should().Be(e4);
                };
            };

        }

        void ThenTry_with_alternate_action()
        {
            describe["with 0 arguments"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "What are we going to do tomorrow?"; };
                Func<string> altActor = () => { return "Try to take over the world!"; };
                int retries = 5;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

            };

            describe["with 1 argument"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "What are we going to do tomorrow?"; };
                Func<int, string> altActor = (p1) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, e1, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T, TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, string>>();
                    runner._arg.Should().Be(e1);
                };
            };

            describe["with 2 argument"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "What are we going to do tomorrow?"; };
                Func<int, long, string> altActor = (p1, p2) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, e1, e2, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T1, T2, TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, long, string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                };
            };

            describe["with 3 argument"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "What are we going to do tomorrow?"; };
                Func<int, long, double, string> altActor = (p1, p2, p3) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;
                double e3 = Math.PI;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, e1, e2, e3, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T1, T2, T3, TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, long, double, string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, double, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                };
            };

            describe["with 4 argument"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "What are we going to do tomorrow?"; };
                Func<int, long, double, bool, string> altActor = (p1, p2, p3, p4) => { return "Try to take over the world!"; };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;
                double e3 = Math.PI;
                bool e4 = true;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, e1, e2, e3, e4, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<T1, T2, T3, T4, TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<int, long, double, bool, string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, long, double, bool, string>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                    runner._arg4.Should().Be(e4);
                };
            };
        }

    }
}
