using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Runners;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retry;

namespace TryIt.Tests.Unit.specs
{
    class FuncRunner_specs : nspec
    {
        void FuncRunner_TResult_base_class()
        {
            FuncRunner<string> subject = null;
            Func<string> expectedActor = null;

            string expectedResult = "Why Me?";
            string actualResult = null;
            CancellationToken token = CancellationToken.None;

            act = () =>
            {
                subject = null;
                actualResult = null;
                subject = new FuncRunner<string>();
                subject.Actor = expectedActor;
            };

            describe["ExecuteFunc method"] = () =>
            {
                before = () =>
                {
                    token = CancellationToken.None;
                    expectedActor = () => expectedResult;
                };
                act = () => actualResult = subject.ExecuteFunc();

                it["should execute the Actor and return the expected results"] = () =>
                    actualResult.Should().Be(expectedResult);
            };

            describe["ExecuteActorAsync method"] = () =>
            {
                Exception thrown = null;

                before = () => expectedActor = () => expectedResult;
                actAsync = async () => 
                {
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

                it["should set the Result to the expected value"] = () =>
                    subject.Result.Should().Be(expectedResult);


                context["when Task is cancelled"] = () =>
                {
                    CancellationTokenSource tokenSource = null;
                    bool actorExecuted = false;

                    before = () =>
                    {
                        actorExecuted = false;
                        expectedActor = () =>
                        {
                            actorExecuted = true;
                            return expectedResult;
                        };

                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                        tokenSource.Cancel();
                    };

                    after = () => tokenSource.Dispose();

                    it["should never execute the actor"] = () =>
                        actorExecuted.Should().BeFalse();

                    it["should raise OperationCanceledException exception"] = () =>
                        Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                };
            };

            describe["HandleSuccessPolicy method"] = () =>
            {
                bool policyHandled = false;
                SuccessPolicyDelegate<string> policy = (result, tries) => { policyHandled = true; };
                act = () =>
                {
                    policyHandled = false;
                    subject = new FuncRunner<string>();
                    subject.SuccessPolicy = policy;
                    subject.HandleSuccessPolicy(1);
                };

                it["should execute the policy provided in the SuccessPolicy property"] = () =>
                    policyHandled.Should().BeTrue();
            };

        }

    }
}
