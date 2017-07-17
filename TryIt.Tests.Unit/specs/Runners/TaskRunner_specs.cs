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

namespace TryIt.Tests.Unit.specs.Runners
{
    [TestClass]
    public class TaskRunner_specs : nSpecTestHarness
    {

        [TestMethod]
        public void TaskRunnerTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        TaskRunner subject = null;
        void before_each()
        {
            subject = null;
        }

            void ExecuteActor_Method()
            {
                bool actionExecuted = false;
                Action expectedActor = () => { actionExecuted = true; };


                actAsync = async () =>
                {
                    subject = new TaskRunner();
                    subject.Actor = new Func<Task>(() => new Task( expectedActor));
                    await subject.ExecuteActorAsync(CancellationToken.None);
                };

                it["should execute the actor set in the Actor property"] = () =>
                    actionExecuted.Should().BeTrue();

            }

        void HandleSuccessPolicy_Method()
        {
            SuccessPolicyDelegate policy = null;
            int expectedTries = 77;
            int actualRetries = default(int);

            act = () =>
            {
                actualRetries = default(int);
                subject = new TaskRunner();
                subject.SuccessPolicy = policy;
                subject.HandleSuccessPolicy(expectedTries);
            };

            describe["when no policy is provided"] = () =>
            {
                before = () => policy = null;

                it["should set the success policy to null"] = () =>
                       subject.SuccessPolicy.Should().BeNull();

                it["should continue without errors"] = () =>
                    actualRetries.Should().Be(default(int));
            };

            describe["when a policy is provided"] = () =>
            {
                before = () => policy = (tries) => { actualRetries = tries; };


                it["should pass the number of tries to the policy"] = () =>
                    actualRetries.Should().Be(expectedTries);
            };
        }
    }
}
