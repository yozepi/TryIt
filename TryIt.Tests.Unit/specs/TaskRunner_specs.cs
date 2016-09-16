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
        void TaskRunner_base_class()
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

        void TaskRunner_T_class()
        {

            TaskRunner<string> subject = null;
            Task expectedTask = Task.Run(() => { });
            Func<string, Task> expectedActor = (a1) => expectedTask;
            string e1 = "Hello";

            act = () =>
            {
                subject = new TaskRunner<string>(e1);
                subject.Actor = expectedActor;
            };

            it["should implement IRunnerArgSource"] = () =>
                subject.As<IRunnerArgSource>().RunnerArgs.Length.Should().Be(1);

            it["constructor should set the internal argument"] = () =>
                subject._arg.Should().Be(e1);

            it["GetTask() should return the task created by the Actor"] = () =>
                subject.GetTask().Should().BeSameAs(expectedTask);

        }

        void TaskRunner_T1_T2_class()
        {

            TaskRunner<string, int> subject = null;
            Task expectedTask = Task.Run(() => { });
            Func<string, int, Task> expectedActor = (a1, a2) => expectedTask;
            string e1 = "Hello";
            int e2 = 42;

            act = () =>
            {
                subject = new TaskRunner<string, int>(e1, e2);
                subject.Actor = expectedActor;
            };

            it["should implement IRunnerArgSource"] = () =>
                subject.As<IRunnerArgSource>().RunnerArgs.Length.Should().Be(2);

            it["constructor should set the internal arguments"] = () =>

            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
            };

            it["GetTask() should return the task created by the Actor"] = () =>
                subject.GetTask().Should().BeSameAs(expectedTask);

        }

        void TaskRunner_T1_T2_T3_class()
        {

            TaskRunner<string, int, double> subject = null;
            Task expectedTask = Task.Run(() => { });
            Func<string, int, double, Task> expectedActor = (a1, a2, a3) => expectedTask;
            string e1 = "Hello";
            int e2 = 42;
            double e3 = double.NaN;

            act = () =>
            {
                subject = new TaskRunner<string, int, double>(e1, e2, e3);
                subject.Actor = expectedActor;
            };

            it["should implement IRunnerArgSource"] = () =>
                subject.As<IRunnerArgSource>().RunnerArgs.Length.Should().Be(3);

            it["constructor should set the internal arguments"] = () =>

            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
                subject._arg3.Should().Be(e3);
            };

            it["GetTask() should return the task created by the Actor"] = () =>
                subject.GetTask().Should().BeSameAs(expectedTask);

        }

        void TaskRunner_T1_T2_T3_T4_class()
        {

            TaskRunner<string, int, double, long> subject = null;
            Task expectedTask = Task.Run(() => { });
            Func<string, int, double, long, Task> expectedActor = (a1, a2, a3, a4) => expectedTask;
            string e1 = "Hello";
            int e2 = 42;
            double e3 = double.NaN;
            long e4 = long.MaxValue;

            act = () =>
            {
                subject = new TaskRunner<string, int, double, long>(e1, e2, e3, e4);
                subject.Actor = expectedActor;
            };

            it["should implement IRunnerArgSource"] = () =>
                subject.As<IRunnerArgSource>().RunnerArgs.Length.Should().Be(4);

            it["constructor should set the internal arguments"] = () =>

            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
                subject._arg3.Should().Be(e3);
                subject._arg4.Should().Be(e4);
            };

            it["GetTask() should return the task created by the Actor"] = () =>
                subject.GetTask().Should().BeSameAs(expectedTask);

        }

    }
}
