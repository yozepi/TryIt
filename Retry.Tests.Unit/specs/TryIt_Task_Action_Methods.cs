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
        void Static_Func_Action_Task_TryIt_Method()
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
            };

            act = () => subject = TryIt.Try(subjectAction, retries);

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
                                if (child.Attempts == 0)
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

            describe["TryIt.Try(Func<Task>, retries).ThenTry(altFunc<Task>, retries)"] = () =>
            {
                ITry child = null;

                Func<Task> altFunc = () => Task.Factory.StartNew(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, retries);
                };

                it["should return an TaskTryIt instance"] = () =>
                     subject.Should().BeOfType<TaskTryIt>();

                it["ThenTry() should use the alternate action"] = () =>
                    child.As<IInternalAccessor>().Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<Task>, retries).ThenTry(altAction, retries)"] = () =>
            {
                ITry child = null;

                Action altAction = () => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.As<IInternalAccessor>().Actor.Should().BeSameAs(altAction);
            };

        }

        void Static_Func_T_Action_Task_TryIt_Method()
        {
            string arg = "Hello World!";

            string e = null;
            ITry subject = null;
            Func<string, Task> subjectAction = null;
            int retries = 4;
            beforeAll = () =>
            {
                e = null;
                subject = null;
                subjectAction = subjectAction = (a) => 
                {
                    return Task.Factory.StartNew(() => { e = a; });
                };
            };
            act = () => subject = TryIt.Try(subjectAction, arg, retries);

            describe["TryIt.Try(Func<T, Task>, arg, retries)"] = () =>
            {

                it["should create an TaskTryIt<T> instance"] = () =>
                    subject.Should().BeOfType<TaskTryIt<string>>();

                it["should set the arg internal property"] = () =>
                {
                    var asTryIt = subject.As<TaskTryIt<string>>();
                    asTryIt._arg.Should().Be(arg);
                };
            };

            describe["TryIt.Try(Func<T, Task>, arg, retries).Go()"] = () =>
            {

                act = () => subject.Go();

                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);

                it["should pass the arguments into the task"] = () =>
                    e.Should().Be(arg);

                it["should return with a status of Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

            };

            describe["TryIt.Try(Func<T, Task>, arg, retries).ThenTry(arg, retries)"] = () =>
            {
                ITry child = null;
                act = () => child = subject.ThenTry(arg, 3);

                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg internal property on the child"] = () =>
                {
                    var asTryIt = subject.As<TaskTryIt<string>>();
                    asTryIt._arg.Should().Be(arg);
                };
            };

            describe["TryIt.Try(Func<T, Task>, retries).ThenTry(altFunc<T, Task>, arg, retries)"] = () =>
            {
                ITry child = null;

                Func<string, Task> altFunc = (a) => Task.Factory.StartNew(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg, retries);
                };

                it["should return an TaskTryIt<T> instance"] = () =>
                     subject.Should().BeOfType<TaskTryIt<string>>();

                it["ThenTry() should use the alternate action"] = () =>
                    child.As<IInternalAccessor>().Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<T, Task>, arg, retries).ThenTry(altAction<T>, arg, retries)"] = () =>
            {
                ITry child = null;

                Action<string> altAction = (a) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.As<IInternalAccessor>().Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var actionTryIt = child.As<ActionTryIt<string>>();
                    actionTryIt._arg.Should().Be(arg);
                };
            };

        }

        void Static_Func_T1_T2_Action_Task_TryIt_Method()
        {
            string arg1 = "Hello World!";
            int arg2 = int.MaxValue;

            string e1 = null;
            int e2 = default(int);
            ITry subject = null;
            Func<string, int, Task> subjectAction = null;
            int retries = 4;
            beforeAll = () =>
            {
                e1 = null;
                e2 = default(int);
                subject = null;
                subjectAction = subjectAction = (a1, a2) => 
                {
                    return Task.Factory.StartNew(() => { e1 = a1; e2 = a2; });
                };
            };
            act = () => subject = TryIt.Try(subjectAction, arg1, arg2, retries);

            describe["TryIt.Try(Func<T, Task>, arg1, arg2, retries)"] = () =>
            {

                it["should create an TaskTryIt<T1, T2> instance"] = () =>
                    subject.Should().BeOfType<TaskTryIt<string, int>>();

                it["should set the arg internal property"] = () =>
                {
                    var asTryIt = subject.As<TaskTryIt<string, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).Go()"] = () =>
            {

                act = () => subject.Go();

                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);

                it["should pass the arguments into the task"] = () =>
                {
                    e1.Should().Be(arg1);
                    e2.Should().Be(arg2);
                };

                it["should return with a status of Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).ThenTry(arg1, arg2, retries)"] = () =>
            {
                ITry child = null;
                act = () => child = subject.ThenTry(arg1, arg2, 3);

                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg internal property on the child"] = () =>
                {
                    var asTryIt = subject.As<TaskTryIt<string, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).ThenTry(altFunc<T1, T2, Task>, arg1, arg2, retries)"] = () =>
            {
                ITry child = null;

                Func<string, int, Task> altFunc = (a1, a2) => Task.Factory.StartNew(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg1, arg2, retries);
                };

                it["should return an TaskTryIt<T1, T2> instance"] = () =>
                     subject.Should().BeOfType<TaskTryIt<string, int>>();

                it["ThenTry() should use the alternate action"] = () =>
                    child.As<IInternalAccessor>().Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).ThenTry(altAction<T>, arg1, arg2, retries)"] = () =>
            {
                ITry child = null;

                Action<string, int> altAction = (a1, a2) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg1, arg2, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.As<IInternalAccessor>().Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var actionTryIt = child.As<ActionTryIt<string, int>>();
                    actionTryIt._arg1.Should().Be(arg1);
                    actionTryIt._arg2.Should().Be(arg2);
                };
            };

        }
    }
}
