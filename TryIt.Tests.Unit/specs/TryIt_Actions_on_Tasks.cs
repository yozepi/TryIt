using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Runners;
using Retry.Builders;
using Retry;

namespace TryIt.Tests.Unit.specs
{

    class TryIt_Actions_on_Tasks : nspec
    {
        void with_no_arguments()
        {
            int retries = 3;
            bool taskExecuted = false;
            Func<Task> subjectAction = null;
            ActionRetryBuilder subject = null;
            before = () =>
            {
                taskExecuted = false;
                subject = null;
                subjectAction = () =>
                {
                    return Task.Run(() => { taskExecuted = true; });
                };
            };

            act = () => subject = Retry.TryIt.TryTask(subjectAction, retries);

            describe["Tryit(Func<Task>, retries)"] = () =>
            {
                it["should return an ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

            };

            describe["Tryit(Func<Task>, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the task."] = () =>
                    taskExecuted.Should().BeTrue();

                context["when the task is not started by the action"] = () =>
                {
                    before = () =>
                    {
                        subjectAction = () =>
                        {
                            return new Task(() => { taskExecuted = true; });
                        };
                    };

                    it["should start the task and run it"] = () =>
                        taskExecuted.Should().BeTrue();
                };

                context["when the task fails the first time"] = () =>
                {
                    before = () =>
                    {
                        subjectAction = () =>
                        {
                            return new Task(() =>
                            {
                                if (subject.LastRunner.Attempts == 1)
                                    throw new Exception("Nope!");

                                taskExecuted = true;
                            });
                        };
                    };

                    it["should try again"] = () =>
                        subject.Attempts.Should().Be(2);

                    it["Should execute the action succesfully"] = () =>
                        taskExecuted.Should().BeTrue();

                    it["should set status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);
                };
            };

            describe["Tryit(Func<Task>, retries).SuccessPolicy()"] = () =>
            {
                SuccessPolicyDelegate successDelegate = null;
                bool successPolicyCalled = false;

                before = () =>
                {
                    successPolicyCalled = false;
                    successDelegate = (i) => { successPolicyCalled = true; };
                };

                act = () => subject.WithSuccessPolicy(successDelegate);

                it["should set the SuccessPolicy delegate"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().Be(successDelegate);

                describe["Tryit(Func<Task>, retries).WithSuccessPolicy().Go()"] = () =>
                {
                    act = () => subject.Go();

                    it["should set status to Success"] = () =>
                        subject.Status.Should().Be(RetryStatus.Success);

                    it["should call the SuccessPolicy delegate"] = () =>
                        successPolicyCalled.Should().BeTrue();
                };
            };

            describe["Tryit(Func<Task>, retries).GoAsync()"] = () =>
            {
                actAsync = async () => await subject.GoAsync();

                it["should execute the task."] = () =>
                    taskExecuted.Should().BeTrue();

                context["when the task is not started by the action"] = () =>
                {
                    before = () =>
                    {
                        subjectAction = () =>
                        {
                            return new Task(() => { taskExecuted = true; });
                        };
                    };

                    it["should start the task and run it"] = () =>
                        taskExecuted.Should().BeTrue();
                };

                context["when the task fails the first time"] = () =>
                {
                    before = () =>
                    {
                        subjectAction = () =>
                        {
                            return new Task(() =>
                            {
                                if (subject.LastRunner.Attempts == 1)
                                    throw new Exception("Nope!");

                                taskExecuted = true;
                            });
                        };
                    };

                    it["should try again"] = () =>
                        subject.Attempts.Should().Be(2);

                    it["Should execute the action succesfully"] = () =>
                        taskExecuted.Should().BeTrue();

                    it["should set status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);
                };
            };

            describe["Tryit(Func<Task>, retries).ThenTry()"] = () =>
            {
                ActionRetryBuilder child = null;
                before = () => child = null;
                act = () => child = subject.ThenTry(retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);


                describe["Tryit(Func<Task>, retries).ThenTry().Go()"] = () =>
                {
                    act = () => child.Go();
                    it["should execute the task."] = () =>
                        taskExecuted.Should().BeTrue();

                    context["when the task is not started by the action"] = () =>
                    {
                        before = () =>
                        {
                            subjectAction = () =>
                            {
                                return new Task(() => { taskExecuted = true; });
                            };
                        };

                        it["should start the task and run it"] = () =>
                            taskExecuted.Should().BeTrue();
                    };


                    context["when Try() fails on every attempt"] = () =>
                    {
                        before = () =>
                        {
                            subjectAction = () => Task.Run(() =>
                            {
                                if (subject.Runners.First.Value.Status != RetryStatus.Fail)
                                {
                                    throw new Exception("Oooohhh child, things are going to get easier!");
                                }
                            });
                        };


                        it["should execute ThenTry()"] = () =>
                            subject.Runners.Last.Value.Attempts.Should().Be(1);

                    };
                };

            };

            describe["TryIt.Try(Func<Task>, retries).ThenTry(altFunc<Task>, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Func<Task> altFunc = () => Task.Run(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, retries);
                };

                it["should return the subject"] = () =>
                     child.Should().Be(subject);

                it["ThenTry() should use the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<Task>, retries).ThenTry(altAction, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Action altAction = () => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(altAction);
            };

        }
    }
}
