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

        void FuncRunner_T_TResult_class()
        {
            string expectedResult = "Because I said so!";
            FuncRunner<int, string> subject = null;
            Func<int, string> actor = (p1) => { return expectedResult; };
            int e1 = 42;

            act = () =>
            {
                subject = new FuncRunner<int, string>(e1);
                subject.Actor = actor;
            };

            it["constructor should set the internal argument"] = () =>
                subject._arg.Should().Be(e1);

            it["ExecuteResult() should run the actor and return the expected result"] = () =>
                subject.ExecuteFunc().Should().Be(expectedResult);
        }

        void FuncRunner_T1_T2_TResult_class()
        {
            string expectedResult = "Because I said so!";
            FuncRunner<int, long, string> subject = null;
            Func<int, long, string> actor = (p1, p2) => { return expectedResult; };
            int e1 = 42;
            long e2 = long.MaxValue;

            act = () =>
            {
                subject = new FuncRunner<int, long, string>(e1, e2);
                subject.Actor = actor;
            };

            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
            };

            it["ExecuteResult() should run the actor and return the expected result"] = () =>
                subject.ExecuteFunc().Should().Be(expectedResult);
        }

        void FuncRunner_T1_T2_T3_TResult_class()
        {
            string expectedResult = "Because I said so!";
            FuncRunner<int, long, double, string> subject = null;
            Func<int, long, double, string> actor = (p1, p2, p3) => { return expectedResult; };
            int e1 = 42;
            long e2 = long.MaxValue;
            double e3 = Math.E;

            act = () =>
            {
                subject = new FuncRunner<int, long, double, string>(e1, e2, e3);
                subject.Actor = actor;
            };

            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
                subject._arg3.Should().Be(e3);
            };

            it["ExecuteResult() should run the actor and return the expected result"] = () =>
                subject.ExecuteFunc().Should().Be(expectedResult);
        }

        void FuncRunner_T1_T2_T3_T4_TResult_class()
        {
            string expectedResult = "Because I said so!";
            FuncRunner<int, long, double, bool, string> subject = null;
            Func<int, long, double, bool, string> actor = (p1, p2, p3, p4) => { return expectedResult; };
            int e1 = 42;
            long e2 = long.MaxValue;
            double e3 = Math.E;
            bool e4 = true;

            act = () =>
            {
                subject = new FuncRunner<int, long, double, bool, string>(e1, e2, e3, e4);
                subject.Actor = actor;
            };

            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
                subject._arg3.Should().Be(e3);
                subject._arg4.Should().Be(e4);
            };

            it["ExecuteResult() should run the actor and return the expected result"] = () =>
                subject.ExecuteFunc().Should().Be(expectedResult);
        }

    }
}
