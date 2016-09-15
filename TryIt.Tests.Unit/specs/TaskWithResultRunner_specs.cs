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

namespace TryIt.Tests.Unit.specs
{
    class TaskWithResultRunner_specs: nspec
    {
        void TaskWithResultRunner_base_class()
        {
            TaskWithResultRunner<string> subject = null;

            describe["GetTask method"] = () =>
            {
                string expectedResult = "Who's your daddy!";
                Task<string> expectedTask = Task.Run(() => { return expectedResult; });
                Func<Task<string>> expectedActor = () => expectedTask;

                act = () =>
                {
                    subject = new TaskWithResultRunner<string>();
                    subject.Actor = expectedActor;
                };

                it["should return the task created by the Actor"] = () =>
                    subject.GetTask().Should().BeSameAs(expectedTask);
            };

            describe["ExecuteActorAsync method"] = () =>
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
                    subject = new TaskWithResultRunner<string>();
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
                        subject = new TaskWithResultRunner<string>();
                        subject.Actor = expectedActor;
                        await subject.ExecuteActorAsync(token);
                    };

                    it["should execute the actor set in the Actor property"] = () =>
                        subject.Result.Should().Be(expectedResult);

                };

                context["when Task is cancelled"] = () =>
                {
                    CancellationTokenSource tokenSource = null;
                    bool actionExecuted = false;
                    before = () =>
                    {
                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                        tokenSource.Cancel();
                        actionExecuted = false;
                        expectedTask = new Task<string>(() => 
                        {
                            actionExecuted = true;
                            return expectedResult;
                        }, token);
                    };

                    after = () => tokenSource.Dispose();

                    it["should never execute the actor"] = () =>
                          actionExecuted.Should().BeFalse();

                    it["should raise OperationCanceledException exception"] = () =>
                        Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                };
            };

        }

        void TaskWithResultRunner_T_class()
        {
            string expectedResult = "Who's your daddy!";
            TaskWithResultRunner<int, string> subject = null;
            Task<string> expectedTask = Task.Run(() => { return expectedResult; });
            Func<int, Task<string>> actor = (a1) => expectedTask;
            int e1 = 42;

            act = () =>
            {
                subject = new TaskWithResultRunner<int, string>(e1);
                subject.Actor = actor;
            };
            it["constructor should set the internal argument"] = () =>
                subject._arg.Should().Be(e1);

            it["GetTask() should return the task created by the Actor"] = () =>
                subject.GetTask().Should().BeSameAs(expectedTask);

        }

        void TaskWithResultRunner_T1_T2_class()
        {
            string expectedResult = "Who's your daddy!";
            TaskWithResultRunner<int, double, string> subject = null;
            Task<string> expectedTask = Task.Run(() => { return expectedResult; });
            Func<int, double, Task<string>> actor = (a1, a2) => expectedTask;
            int e1 = 42;
            double e2 = 99.0;

            act = () =>
            {
                subject = new TaskWithResultRunner<int, double, string>(e1, e2);
                subject.Actor = actor;
            };
            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
            };

            it["GetTask() should return the task created by the Actor"] = () =>
                subject.GetTask().Should().BeSameAs(expectedTask);

        }

        void TaskWithResultRunner_T1_T2_T3_class()
        {
            string expectedResult = "Who's your daddy!";
            TaskWithResultRunner<int, double, long, string> subject = null;
            Task<string> expectedTask = Task.Run(() => { return expectedResult; });
            Func<int, double, long, Task<string>> actor = (a1, a2, a3) => expectedTask;
            int e1 = 42;
            double e2 = 99.0;
            long e3 = DateTime.Now.Ticks;

            act = () =>
            {
                subject = new TaskWithResultRunner<int, double, long, string>(e1, e2, e3);
                subject.Actor = actor;
            };
            it["constructor should set the internal arguments"] = () =>
            {
                subject._arg1.Should().Be(e1);
                subject._arg2.Should().Be(e2);
                subject._arg3.Should().Be(e3);
            };

            it["GetTask() should return the task created by the Actor"] = () =>
                subject.GetTask().Should().BeSameAs(expectedTask);

        }

        void TaskWithResultRunner_T1_T2_T3_T4_class()
        {
            string expectedResult = "Who's your daddy!";
            TaskWithResultRunner<int, double, long, bool, string> subject = null;
            Task<string> expectedTask = Task.Run(() => { return expectedResult; });
            Func<int, double, long, bool, Task<string>> actor = (a1, a2, a3, a4) => expectedTask;
            int e1 = 42;
            double e2 = 99.0;
            long e3 = DateTime.Now.Ticks;
            bool e4 = true;

            act = () =>
            {
                subject = new TaskWithResultRunner<int, double, long, bool, string>(e1, e2, e3, e4);
                subject.Actor = actor;
            };
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
