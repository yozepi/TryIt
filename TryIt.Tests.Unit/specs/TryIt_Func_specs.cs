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
        }

        void ThenTry_Method()
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
        }

        void ThenTry_with_alternate_action()
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

        }

    }
}
