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

        void ActionRunner_T_class()
        {

            ActionRunner<string> subject = null;
            Action<string> actor = (a1) => { };
            string e1 = "Hello";

            act = () =>
            {
                subject = new ActionRunner<string>(e1);
                subject.Actor = actor;
            };
            it["constructor should set the internal argument"] = () =>
                subject._arg.Should().Be(e1);

            it["GetAction() should return the action that will run the Actor"] = () =>
            {
                var runAction = subject.GetAction();
                Assert.IsInstanceOfType(runAction, typeof(Action));
            };
        }

        void ActionRunner_T1_T2_class()
        {

            ActionRunner<string, int> subject = null;
            Action<string, int> actor = (a1, a2) => { };
            string e1 = "Hello";
            int e2 = 42;

            act = () =>
            {
                subject = new ActionRunner<string, int>(e1, e2);
                subject.Actor = actor;
            };
            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
            };

            it["GetAction() should return the action that will run the Actor"] = () =>
            {
                var runAction = subject.GetAction();
                Assert.IsInstanceOfType(runAction, typeof(Action));
            };
        }

        void ActionRunner_T1_T2_T3_class()
        {

            ActionRunner<string, int, long> subject = null;
            Action<string, int, long> actor = (a1, a2, a3) => { };
            string e1 = "Hello";
            int e2 = 42;
            long e3 = long.MinValue;

            act = () =>
            {
                subject = new ActionRunner<string, int, long>(e1, e2, e3);
                subject.Actor = actor;
            };
            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
                subject._arg3.Should().Be(e3);
            };

            it["GetAction() should return the action that will run the Actor"] = () =>
            {
                var runAction = subject.GetAction();
                Assert.IsInstanceOfType(runAction, typeof(Action));
            };
        }

        void ActionRunner_T1_T2_T3_T4_class()
        {

            ActionRunner<string, int, long, double> subject = null;
            Action<string, int, long, double> actor = (a1, a2, a3, a4) => { };
            string e1 = "Hello";
            int e2 = 42;
            long e3 = long.MinValue;
            double e4 = Math.PI;

            act = () =>
            {
                subject = new ActionRunner<string, int, long, double>(e1, e2, e3, e4);
                subject.Actor = actor;
            };
            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
                subject._arg3.Should().Be(e3);
                subject._arg4.Should().Be(e4);
            };

            it["GetAction() should return the action that will run the Actor"] = () =>
            {
                var runAction = subject.GetAction();
                Assert.IsInstanceOfType(runAction, typeof(Action));
            };
        }


    }
}


