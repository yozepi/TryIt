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
    class Retry_Tasks : nspec
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
                    return Task.Factory.StartNew(() => { taskExecuted = true; });
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
                            subjectAction = () => Task.Factory.StartNew(() =>
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

                Func<Task> altFunc = () => Task.Factory.StartNew(() => { });
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

        void with_1_argument()
        {
            string arg = "Hello World!";

            string e = null;
            ActionRetryBuilder subject = null;
            Func<string, Task> taskAction = null;
            int retries = 4;
            beforeAll = () =>
            {
                e = null;
                subject = null;
                taskAction = taskAction = (a) =>
                {
                    return Task.Factory.StartNew(() => { e = a; });
                };
            };
            act = () => subject = Retry.TryIt.TryTask(taskAction, arg, retries);

            describe["TryIt.Try(Func<T, Task>, arg, retries)"] = () =>
            {

                it["should create an ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the arg internal property"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string>>();
                    runner._arg.Should().Be(arg);
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
                ActionRetryBuilder child = null;
                act = () => child = subject.ThenTry(arg, 3);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);


                it["should set the arg internal property on the child"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string>>();
                    runner._arg.Should().Be(arg);
                };
            };

            describe["TryIt.Try(Func<T, Task>, retries).ThenTry(altFunc<T, Task>, arg, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Func<string, Task> altFunc = (a) => Task.Factory.StartNew(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg, retries);
                };

                it["should return the subject"] = () =>
                     child.Should().Equals(subject);

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<T, Task>, arg, retries).ThenTry(altAction<T>, arg, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Action<string> altAction = (a) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var runner = child.LastRunner.As<TaskRunner<string>>();
                    runner._arg.Should().Be(arg);
                };
            };

        }

        void with_2_arguments()
        {
            string arg1 = "Hello World!";
            int arg2 = int.MaxValue;

            string e1 = null;
            int e2 = default(int);
            ActionRetryBuilder subject = null;
            Func<string, int, Task> taskFunc = null;
            int retries = 4;
            beforeAll = () =>
            {
                e1 = null;
                e2 = default(int);
                subject = null;
                taskFunc = taskFunc = (a1, a2) =>
                {
                    return Task.Factory.StartNew(() => { e1 = a1; e2 = a2; });
                };
            };
            act = () => subject = Retry.TryIt.TryTask(taskFunc, arg1, arg2, retries);

            describe["TryIt.Try(Func<T, Task>, arg1, arg2, retries)"] = () =>
            {

                it["should create a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string, int>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
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
                ActionRetryBuilder child = null;
                act = () => child = subject.ThenTry(arg1, arg2, 3);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should set the arg internal property on the child"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string, int>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).ThenTry(altFunc<T1, T2, Task>, arg1, arg2, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Func<string, int, Task> altFunc = (a1, a2) => Task.Factory.StartNew(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg1, arg2, retries);
                };

                it["should return a ActionRetryBuilder instance"] = () =>
                     subject.Should().BeOfType<ActionRetryBuilder>();

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).ThenTry(altAction<T>, arg1, arg2, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Action<string, int> altAction = (a1, a2) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg1, arg2, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var runner = child.LastRunner.As<ActionRunner<string, int>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                };
            };

        }

        void with_3_arguments()
        {
            string arg1 = "Hello World!";
            int arg2 = int.MaxValue;
            long arg3 = long.MinValue;

            string e1 = null;
            int e2 = default(int);
            long e3 = default(long);
            ActionRetryBuilder subject = null;
            Func<string, int, long, Task> taskFunc = null;
            int retries = 4;
            beforeAll = () =>
            {
                e1 = null;
                e2 = default(int);
                e3 = default(long);
                subject = null;
                taskFunc = taskFunc = (a1, a2, a3) =>
                {
                    return Task.Factory.StartNew(() => { e1 = a1; e2 = a2; e3 = a3; });
                };
            };
            act = () => subject = Retry.TryIt.TryTask(taskFunc, arg1, arg2, arg3, retries);

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, arg3, retries)"] = () =>
            {

                it["should create a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string, int, long>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                };
            };

            describe["TryIt.Try(Func<T1, T2, T3, Task>, arg1, arg2, arg3, retries).Go()"] = () =>
            {

                act = () => subject.Go();

                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);

                it["should pass the arguments into the task"] = () =>
                {
                    e1.Should().Be(arg1);
                    e2.Should().Be(arg2);
                    e3.Should().Be(arg3);
                };

                it["should return with a status of Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);
            };

            describe["TryIt.Try(Func<T1, T2, T3, Task>, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries)"] = () =>
            {
                ActionRetryBuilder child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, 3);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should set the arg internal property on the child"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string, int, long>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                };
            };

            describe["TryIt.Try(Func<T1, T2, T3, Task>, arg1, arg2, arg3, retries).ThenTry(altFunc<T1, T2, T3, Task>, arg1, arg2, arg3, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Func<string, int, long, Task> altFunc = (a1, a2, a3) => Task.Factory.StartNew(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg1, arg2, arg3, retries);
                };

                it["should return a ActionRetryBuilder instance"] = () =>
                     subject.Should().BeOfType<ActionRetryBuilder>();

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).ThenTry(altAction<T>, arg1, arg2, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Action<string, int, long> altAction = (a1, a2, a3) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg1, arg2, arg3, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var runner = child.LastRunner.As<ActionRunner<string, int, long>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                };
            };

        }

        void with_4_arguments()
        {
            string arg1 = "Hello World!";
            int arg2 = int.MaxValue;
            long arg3 = long.MinValue;
            bool arg4 = true;

            string e1 = null;
            int e2 = default(int);
            long e3 = default(long);
            bool e4 = default(bool);

            ActionRetryBuilder subject = null;
            Func<string, int, long, bool, Task> taskFunc = null;
            int retries = 4;
            beforeAll = () =>
            {
                e1 = null;
                e2 = default(int);
                e3 = default(long);
                e4 = default(bool);

                subject = null;
                taskFunc = taskFunc = (a1, a2, a3, a4) =>
                {
                    return Task.Factory.StartNew(() => { e1 = a1; e2 = a2; e3 = a3; e4 = a4; });
                };
            };
            act = () => subject = Retry.TryIt.TryTask(taskFunc, arg1, arg2, arg3, arg4, retries);

            describe["TryIt.Try(Func<T, Task>, arg1, arg2, arg3, arg4, retries)"] = () =>
            {

                it["should create a ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string, int, long, bool>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                    runner._arg4.Should().Be(arg4);
                };
            };

            describe["TryIt.Try(Func<T1, T2, T3, T4, Task>, arg1, arg2, arg3, arg4, retries).Go()"] = () =>
            {

                act = () => subject.Go();

                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);

                it["should pass the arguments into the task"] = () =>
                {
                    e1.Should().Be(arg1);
                    e2.Should().Be(arg2);
                    e3.Should().Be(arg3);
                    e4.Should().Be(arg4);
                };

                it["should return with a status of Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);
            };

            describe["TryIt.Try(Func<T1, T2, T3, T4, Task>, arg1, arg2, arg3, arg4, retries).ThenTry(arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                ActionRetryBuilder child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, 3);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should set the arg internal property on the child"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskRunner<string, int, long, bool>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                    runner._arg4.Should().Be(arg4);
                };
            };

            describe["TryIt.Try(Func<T1, T2, T3, T4, Task>, arg1, arg2, arg3, arg4, retries).ThenTry(altFunc<T1, T2, T3, T4, Task>, arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Func<string, int, long, bool, Task> altFunc = (a1, a2, a3, a4) => Task.Factory.StartNew(() => { });
                act = () =>
                {
                    child = subject.ThenTry(altFunc, arg1, arg2, arg3, arg4, retries);
                };

                it["should return a ActionRetryBuilder instance"] = () =>
                     subject.Should().BeOfType<ActionRetryBuilder>();

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

            };

            describe["TryIt.Try(Func<T1, T2, Task>, arg1, arg2, retries).ThenTry(altAction<T>, arg1, arg2, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Action<string, int> altAction = (a1, a2) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg1, arg2, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var runner = child.LastRunner.As<ActionRunner<string, int>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                };
            };

        }

    }
}
