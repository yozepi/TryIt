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
    public class TaskRunner_T_specs : nSpecTestHarness
    {

        [TestMethod]
        public void TaskRunnerTTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType()});
            this.RunSpecs();
        }


        TaskRunner<string> subject;

        void ExecuteActorAsync_Method()
        {
            string expectedResult = "Who's your daddy!";
            CancellationToken token = CancellationToken.None;
            Task<string> expectedTask = null;
            Func<Task<string>> expectedActor = () => expectedTask;
            Exception thrown = null;

            before = () =>
            {
                token = CancellationToken.None;
                expectedTask = Task.Run(() => { return expectedResult; }, token);
            };

            actAsync = async () =>
            {
                subject = new TaskRunner<string>();
                subject.Actor = expectedActor;
                thrown = null;
                try
                {
                    await subject.ExecuteActorAsync(token);
                }
                catch (Exception ex)
                {
                    thrown = ex;
                }
            };

            it["should set the Result returned by the Actor"] = () =>
                subject.Result.Should().Be(expectedResult);

            context["when the actor returns an unstarted task"] = () =>
            {
                before = () =>
                {
                    expectedTask = new Task<string>(() => { return expectedResult; }, token);
                };

                actAsync = async () =>
                {
                    subject = new TaskRunner<string>();
                    subject.Actor = expectedActor;
                    await subject.ExecuteActorAsync(token);
                };

                it["should execute the actor set in the Actor property"] = () =>
                    subject.Result.Should().Be(expectedResult);

            };


        }

        void HandleSuccessPolicy_Method()
        {
            SuccessPolicyDelegate<string> policy = null;
            string expectedResult = "Hey hey, My my";
            int expectedTries = 3;
            string actualResult = null;
            int actualRetries = default(int);

            act = () =>
            {
                actualResult = null;
                actualRetries = default(int);
                subject = new TaskRunner<string>();
                subject.Result = expectedResult;
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
                before = () => policy = (result, retries) => { actualResult = result; actualRetries = retries; };


                it["should pass the number of tries to the policy"] = () =>
                    actualRetries.Should().Be(expectedTries);

                it["should pass the result of the actor to the policy"] = () =>
                    actualResult.Should().Be(expectedResult);


            };
        }


    }
}
