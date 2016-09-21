using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using Retry.Builders;
using Retry.Delays;
using Retry;

namespace TryIt.Tests.Unit.specs
{
    class TryIt_UsingDelay_WithError_WithSuccess_specs:nspec
    {
        void For_ActionRetryBuilders()
        {
            describe["UsingDelay Method"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                IDelay expectedDelay = Delay.Basic(TimeSpan.FromSeconds(1));

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.UsingDelay(expectedDelay);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);
            };

            describe["WithErrorPolicy Method"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithErrorPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);
            };

            describe["WithSuccessPolicy Method"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                SuccessPolicyDelegate expectedPolicy = (tries) => { };

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithSuccessPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);
            };

        }

        void For_FuncRetryBuilders()
        {
            describe["UsingDelay Method"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                IDelay expectedDelay = Delay.Basic(TimeSpan.FromSeconds(1));

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.UsingDelay(expectedDelay);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);
            };

            describe["WithErrorPolicy Method"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithErrorPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);
            };

            describe["WithSuccessPolicy Method"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                SuccessPolicyDelegate<string> expectedPolicy = (result, tries) => { };

                before = () => sourceBuilder = Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithSuccessPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);
            };

        }

    }
}
