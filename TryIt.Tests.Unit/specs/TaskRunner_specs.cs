using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using Retry.Runners;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retry;

namespace TryIt.Tests.Unit.specs
{
    class TaskRunner_specs : nspec
    {
        void TaskRunner_class()
        {
            TaskRunner subject = null;

            describe["GetTask method"] = () =>
            {
                Task expectedTask = Task.Run(() => { });
                Func<Task> expectedActor = () => expectedTask;

                act = () =>
                {
                    subject = new TaskRunner();
                    subject.Actor = expectedActor;
                };

                it["should return the task created by the Actor"] = () =>
                    subject.GetTask().Should().BeSameAs(expectedTask);
            };

            describe["ExecuteActorAsync method"] = () =>
            {
                bool actionExecuted = false;
                CancellationToken token = CancellationToken.None;
                Task expectedTask = null;
                Func<Task> expectedActor = () => expectedTask;
                Exception thrown = null;

                before = () =>
                {
                    token = CancellationToken.None;
                };

                actAsync = async () =>
                {
                    actionExecuted = false;
                    expectedTask = Task.Run(() => { actionExecuted = true; }, token);
                    subject = new TaskRunner();
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

                it["should execute the actor set in the Actor property"] = () =>
                    actionExecuted.Should().BeTrue();

                context["when the actor returns an unstarted task"] = () =>
                {
                    actAsync = async () =>
                    {
                        actionExecuted = false;
                        expectedTask = new Task(() => { actionExecuted = true; }, token);
                        subject = new TaskRunner();
                        subject.Actor = expectedActor;
                        thrown = null;
                        await subject.ExecuteActorAsync(token);
                    };

                    it["should execute the actor set in the Actor property"] = () =>
                        actionExecuted.Should().BeTrue();

                };

                context["when Task is cancelled"] = () =>
                {
                    CancellationTokenSource tokenSource = null;
                    before = () =>
                    {
                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                        tokenSource.Cancel();
                    };

                    after = () => tokenSource.Dispose();

                    it["should never execute the actor"] = () =>
                          actionExecuted.Should().BeFalse();

                    it["should raise OperationCanceledException exception"] = () =>
                        Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                };
            };

            describe["HandleSuccessPolicy method"] = () =>
            {
                bool policyHandled = false;
                SuccessPolicyDelegate policy = (tries) => { policyHandled = true; };
                act = () =>
                {
                    policyHandled = false;
                    subject = new TaskRunner();
                    subject.SuccessPolicy = policy;
                    subject.HandleSuccessPolicy(1);
                };

                it["should execute the policy provided in the SuccessPolicy property"] = () =>
                    policyHandled.Should().BeTrue();
            };
        }
    }
}
