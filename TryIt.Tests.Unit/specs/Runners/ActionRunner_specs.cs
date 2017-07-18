using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using yozepi.Retry.Runners;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yozepi.Retry;
using System.Reflection;

namespace TryIt.Tests.Unit.specs.Runners
{
    [TestClass]
    public class ActionRunner_specs : nSpecTestHarness
    {

        [TestMethod]
        public void ActionRunnerTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }



        ActionRunner subject = null;

        void before_each()
        {
            subject = null;
        }

        void ExecuteActor_Method()
        {
            bool actionExecuted = false;
            Action expectedActor = () => { actionExecuted = true; };


            act = () =>
            {
                subject = new ActionRunner();
                subject.Actor = expectedActor;
                subject.ExecuteActor();
            };

            it["should execute the actor set in the Actor property"] = () =>
                actionExecuted.Should().BeTrue();

        }

        void HandleSuccessPolicy_Method()
        {
            bool policyHandled = false;
            SuccessPolicyDelegate policy = null;
            act = () =>
            {
                policyHandled = false;
                subject = new ActionRunner();
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
                before = () => policy = (tries) => { policyHandled = true; };

                it["should execute the policy provided in the SuccessPolicy property"] = () =>
                    policyHandled.Should().BeTrue();
            };
        }
    }
}


