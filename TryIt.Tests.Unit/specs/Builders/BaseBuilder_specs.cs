using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using yozepi.Retry.Builders;
using System.Threading;
using yozepi.Retry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yozepi.Retry.Runners;
using yozepi.Retry.Delays;
using yozepi;

namespace TryIt.Tests.Unit.specs.Builders
{
    [TestClass]
    public class BaseBuilder_specs : nSpecTestHarness
    {
        [TestMethod]
        public void BaseBuilderTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        void Setter_Methods()
        {
            BaseBuilder subject = new ActionRetryBuilder();
            subject.AddRunner(new ActionRunner { RetryCount = 1 });

            describe["SetRetryCount()"] = () =>
            {
                BaseBuilder returned = null;
                before = () => returned = null;
                act = () => returned = subject.SetRetryCount(3);

                it["should set the RetryCount of the underlying runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(3);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);

                context["when RetryCount is invalid"] = () =>
                {
                    it["should throw InvalidArgumentException"] = () =>
                        subject.Invoking(s => s.SetRetryCount(0))
                        .ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["SetSuccessPolicy()"] = () =>
            {
                BaseBuilder returned = null;
                SuccessPolicyDelegate expectedPolicy = (tries) => { };
                before = () => returned = null;
                act = () => returned = subject.SetSuccessPolicy(expectedPolicy);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };

            describe["SetErrorPolicy()"] = () =>
            {
                BaseBuilder returned = null;
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };
                before = () => returned = null;
                act = () => returned = subject.SetErrorPolicy(expectedPolicy);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };

            describe["SetDelay()"] = () =>
            {
                BaseBuilder returned = null;
                Delay expectedDelay = Delay.NoDelay();
                before = () => returned = null;
                act = () => returned = subject.SetDelay(expectedDelay);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };

            describe["SetActor()"] = () =>
            {
                BaseBuilder returned = null;
                Action expectedActor = () => { };
                before = () => returned = null;
                act = () => returned = subject.SetActor(expectedActor);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(expectedActor);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);

                context["when actor is null"] = () =>
                {
                    it["should throw InvalidArgumentException"] = () =>
                        subject.Invoking(s => s.SetActor(null))
                        .ShouldThrow<ArgumentNullException>();
                };
            };

            describe["adding a second runner"] = () =>
            {
                BaseBuilder returned = null;
                BaseRunner firstRunner = null;
                BaseRunner newRunner = null;
                SuccessPolicyDelegate successPolicy = (tries) => { };
                ErrorPolicyDelegate errorPolicy = (ex, tries) => { return true; };
                Action actor = () => { };

                before = () =>
                {
                    returned = null;
                    newRunner = null;
                    firstRunner = subject.LastRunner;
                    firstRunner.RetryCount = 3;
                    firstRunner.SuccessPolicy = successPolicy;
                    firstRunner.ErrorPolicy = errorPolicy;
                    firstRunner.Delay = Delay.Basic(TimeSpan.FromMilliseconds(100));
                    firstRunner.Actor = actor;

                    newRunner = new ActionRunner();
                };

                act = () => returned = subject.AddRunner(newRunner);

                it["should append the second runner"] = () =>
                    subject.LastRunner.Should().BeSameAs(newRunner);

                it["should copy the values of the first runner to the second runner"] = () =>
                {
                    firstRunner.RetryCount.Should().Be(newRunner.RetryCount);
                    firstRunner.SuccessPolicy.Should().BeSameAs(newRunner.SuccessPolicy);
                    firstRunner.ErrorPolicy.Should().BeSameAs(newRunner.ErrorPolicy);
                    firstRunner.Delay.Should().BeSameAs(newRunner.Delay);
                    firstRunner.Actor.Should().BeSameAs(newRunner.Actor);
                };

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };
        }

     }
}