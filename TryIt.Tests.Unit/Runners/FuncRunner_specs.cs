using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using yozepi.Retry.Runners;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yozepi.Retry;

namespace TryIt.Tests.Unit.Runners
{
    [TestClass]
    public class FuncRunner_specs : nSpecTestHarness
    {

        [TestMethod]
        public void FuncRunnerTests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { this.GetType() };
                return types;
            });
            this.RunSpecs();
        }


        FuncRunner<string> subject = null;
        Func<string> expectedActor = null;

        string expectedResult = "Why Me?";
        string actualResult = null;

        void ExecuteActorAsync_Method()
        {

            before = () =>
            {
                expectedActor = () => expectedResult;
                actualResult = null;
                subject = new FuncRunner<string>();
                subject.Actor = expectedActor;
            };

            act = () => subject.ExecuteActor();

            it["should set the Result to the expected value"] = () =>
                subject.Result.Should().Be(expectedResult);
        }

        void HandleSuccessPolicy_Method()
        {
            bool policyHandled = false;
            SuccessPolicyDelegate<string> policy = null;

            act = () =>
            {
                policyHandled = false;
                subject = new FuncRunner<string>();
                subject.SuccessPolicy = policy;
                subject.HandleSuccessPolicy(1);
            };

            describe["when no policy is provided"] = () =>
            {
                before = () => policy = null;

                it["should set the success policy to null"] = () =>
                       subject.SuccessPolicy.Should().BeNull();

                it["should continue without errors"] = () =>
                    policyHandled.Should().BeFalse();
            };

            describe["when a policy is provided"] = () =>
            {
                before = () => policy = (result, tries) => { policyHandled = true; };

                it["should execute the policy provided in the SuccessPolicy property"] = () =>
                    policyHandled.Should().BeTrue();
            };
        }

    }
}
