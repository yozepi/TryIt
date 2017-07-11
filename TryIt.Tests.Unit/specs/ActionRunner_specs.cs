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
using System.Reflection;

namespace TryIt.Tests.Unit.specs
{
    class ActionRunner_specs: nspec
    {
        void ActionRunner_base_class()
        {
            ActionRunner subject = null;

            describe["GetAction method"] = () =>
            {
                Action expectedActor = () => { };
                act = () =>
                {
                    subject = new ActionRunner();
                    subject.Actor = expectedActor;
                };

                it["should return the actor set in Actor property"] = () =>
                    subject.GetAction().Should().BeSameAs(expectedActor);

            };

            describe["ExecuteActorAsync method"] = () =>
            {
                bool actionExecuted = false;
                Action expectedActor = () => { actionExecuted = true; };
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;

                before = () => token = CancellationToken.None;

                actAsync = async () =>
                {
                    actionExecuted = false;
                    subject = new ActionRunner();
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
                    subject = new ActionRunner();
                    subject.SuccessPolicy = policy;
                    subject.HandleSuccessPolicy(1);
                };

                it["should execute the policy provided in the SuccessPolicy property"] = () =>
                    policyHandled.Should().BeTrue();
            };

        }

    }
}


