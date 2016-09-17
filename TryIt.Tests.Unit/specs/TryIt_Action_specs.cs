using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Retry.Builders;
using Retry.Runners;
using Retry;
using Retry.Delays;
using System.Threading;

namespace TryIt.Tests.Unit.specs
{

    class TryIt_Action_specs : nspec
    {
        void Try_Method()
        {
            describe["with 0 parameters"] = () =>
            {
                ActionRetryBuilder subject = null;
                Action actor = () => { };
                int retries = 5;

                act = () => subject = Retry.TryIt.Try(actor, retries);

                it["should return a ActionRetryBuilder"] = () =>
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
                ActionRetryBuilder subject = null;
                Action<int> actor = (p1) => { };
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
                    var runner = subject.LastRunner.As<ActionRunner<int>>();
                    runner._arg.Should().Be(e1);
                };


            };

            describe["with 2 parameters"] = () =>
            {
                ActionRetryBuilder subject = null;
                Action<int, long> actor = (p1, p2) => { };
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
                    var runner = subject.LastRunner.As<ActionRunner<int, long>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                };


            };

            describe["with 3 parameters"] = () =>
            {
                ActionRetryBuilder subject = null;
                Action<int, long, double> actor = (p1, p2, p3) => { };
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
                    var runner = subject.LastRunner.As<ActionRunner<int, long, double>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                };


            };

            describe["with 4 parameters"] = () =>
            {
                ActionRetryBuilder subject = null;
                Action<int, long, double, bool> actor = (p1, p2, p3, p4) => { };
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
                    var runner = subject.LastRunner.As<ActionRunner<int, long, double, bool>>();
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
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                int retries = 5;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

            };

            describe["with 1 argument"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action<int> actor = (p1) => { };
                int retries = 5;
                int e1 = 42;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1, 1);
                act = () => subject = sourceBuilder.ThenTry(e1, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner<T>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int>>();
                    runner._arg.Should().Be(e1);
                };
            };

            describe["with 2 arguments"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action<int, long> actor = (p1, p2) => { };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1, 2, 1);
                act = () => subject = sourceBuilder.ThenTry(e1, e2, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner<T1, T2>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int, long>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal arguments"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int, long>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                };
            };

            describe["with 3 arguments"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action<int, long, double> actor = (p1, p2, p3) => { };
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

                it["the new runner should be an ActionRunner<T1, T2, T3>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int, long, double>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal arguments"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int, long, double>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                };
            };

            describe["with 4 arguments"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action<int, long, double, bool> actor = (p1, p2, p3, p4) => { };
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

                it["the new runner should be an ActionRunner<T1, T2, T3, T4>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int, long, double, bool>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal arguments"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int, long, double, bool>>();
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
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                Action altActor = () => { };
                int retries = 5;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

            };

            describe["with 1 argument"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                Action<int> altActor = (p1) => { };
                int retries = 5;
                int e1 = 42;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, e1, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner<T>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int>>();
                    runner._arg.Should().Be(e1);
                };
            };

            describe["with 2 argument"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                Action<int, long> altActor = (p1, p2) => { };
                int retries = 5;
                int e1 = 42;
                long e2 = long.MaxValue;

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, e1, e2, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner<T1, T2>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int, long>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int, long>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                };
            };

            describe["with 3 argument"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                Action<int, long, double> altActor = (p1, p2, p3) => { };
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

                it["the new runner should be an ActionRunner<T1, T2, T3>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int, long, double>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int, long, double>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                };
            };

            describe["with 4 argument"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                Action<int, long, double, bool> altActor = (p1, p2, p3, p4) => { };
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

                it["the new runner should be an ActionRunner<T1, T2, T3, T4>"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner<int, long, double, bool>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the runner's internal argument"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<int, long, double, bool>>();
                    runner._arg1.Should().Be(e1);
                    runner._arg2.Should().Be(e2);
                    runner._arg3.Should().Be(e3);
                    runner._arg4.Should().Be(e4);
                };
            };

        }
    }
}