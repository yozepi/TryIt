using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Retry.Tests.Unit.specs
{
    class TryIt_Task_Action_Methods : nspec
    {
        void Static_Func_Action_Task_TryIt_Methods()
        {
            int retries = 3;
            bool taskExecuted = false;
            Func<Task> subjectAction = null;
            ITry subject = null;
            before = () =>
            {
                taskExecuted = false;
                subject = null;
                subjectAction = () =>
                {
                    return Task.Factory.StartNew(() => { taskExecuted = true; });
                };
                act = () => subject = TryIt.Try(subjectAction, retries);
            };

            describe["Tryit(Func<Task>, retries)"] = () =>
            {
                it["should return an TaskTryIt instance"] = () =>
                    subject.Should().BeOfType<TaskTryIt>();

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
                                if (subject.Attempts == 1)
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

            describe["Tryit(Func<Task>, retries).OnSuccess().Go()"] = () =>
            {
                OnSuccessDelegate onSuccess = null;
                bool onSuccessCalled = false;

                before = () =>
                {
                    onSuccessCalled = false;
                    onSuccess = (i) => { onSuccessCalled = true; };
                };

                act = () => subject.OnSuccess(onSuccess).Go();

                it["should call the OnSuccess delegate"] = () =>
                onSuccessCalled.Should().BeTrue();
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
                                if (subject.Attempts == 1)
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
                ITry child = null;
                before = () => child = null;
                act = () => child = subject.ThenTry(retries);

                it["should return an TaskTryIt instance"] = () =>
                    child.Should().BeOfType<TaskTryIt>();


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
                            subjectAction = () => Task.Factory.StartNew(() =>
                            {
                                if(child.Attempts == 0)
                                {
                                    throw new Exception("Oooohhh child, things are going to get easier!");
                                }
                            });
                        };

                        it["should execute ThenTry()"] = () =>
                            child.Attempts.Should().Be(1);

                    };
                };

            };
        }
    }
}
