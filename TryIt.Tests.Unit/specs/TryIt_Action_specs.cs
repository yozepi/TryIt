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

        }

        void ThenTry_Method()
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
        }

        void ThenTry_with_alternate_action()
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

        }
    }
}