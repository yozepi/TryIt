using NSpec;
using yozepi.Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using yozepi.Retry;
using Moq;
using yozepi.Retry.Delays;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TryIt.Tests.Unit.specs.Runners
{
    [TestClass]
    public class BaseRunner_specs : nSpecTestHarness
    {
        [TestMethod]
        public void BaseRunnerTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        void CopySettings_Method()
        {
            BaseRunner source = null;
            BaseRunner target = null;

            act = () =>
            {
                source = new ActionRunner
                {
                    Delay = Delay.DefaultDelay,
                    ErrorPolicy = (ex, retries) => { return true; },
                    SuccessPolicy = new Action(() => { }),
                    RetryCount = 4,
                    Actor = new Action(() => { })
                };

                target = new ActionRunner();
                source.CopySettings(target);
            };
            it["should copy Delay to the target"] = () =>
                target.Delay.Should().Be(source.Delay);

            it["should copy ErrorPolicy to the target"] = () =>
                target.ErrorPolicy.Should().Be(source.ErrorPolicy);

            it["should copy SuccessPolicy to the target"] = () =>
                target.SuccessPolicy.Should().Be(source.SuccessPolicy);

            it["should copy RetryCount to the target"] = () =>
                target.RetryCount.Should().Be(source.RetryCount);

            it["should copy Actor to the target"] = () =>
                target.Actor.Should().Be(source.Actor);


        }

        void HandleErrorPolicy_Method()
        {
            BaseRunner subject = null;
            bool actualResult = default(bool);
            var ex = new Exception();
            int retryCount = 3;

            before = () => subject = new ActionRunner();

            act = () => actualResult = subject.HandleErrorPolicy(ex, retryCount);

            describe["when the error policy is null"] = () =>
            {
                before = () => subject.ErrorPolicy = null;

                it["should return true"] = () =>
                    actualResult.Should().BeTrue();
            };

            describe["when the error policy is not null"] = () =>
            {
                Exception expectedEx = null;
                int expectedRetryCount = default(int);
                bool expectedResult = false;

                before = () =>
                {
                    expectedEx = null;
                    expectedRetryCount = default(int);
                    subject.ErrorPolicy = (Exception x, int c) =>
                    {
                        expectedEx = x;
                        expectedRetryCount = c;
                        return expectedResult;
                    };
                };

                it["should pass the exception to the policy"] = () =>
                    expectedEx.Should().Be(ex);

                it["should pass the retry counts to the policy"] = () =>
                    expectedRetryCount.Should().Be(retryCount);

                it["should return the policy result"] = () =>
                    actualResult.Should().Be(expectedResult);
            };


        }
    }
}
