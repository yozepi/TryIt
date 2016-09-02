using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Runners;
using Retry.Builders;

namespace Retry.Tests.Unit.specs
{
    class Retry_TaskTResults : nspec
    {
        void with_no_arguments()
        {
            int retries = 3;
            bool taskExecuted = false;
            string expectedResult = "Hello from a Task!";
            Func<Task<string>> subjectFunc = null;
            FuncRetryBuilder<string> subject = null;
            before = () =>
            {
                taskExecuted = false;
                subject = null;
                subjectFunc = () =>
                {
                    return Task<string>.Factory.StartNew(() => { taskExecuted = true; return expectedResult; });
                };
            };
            act = () => subject = TryIt.TryTask(subjectFunc, retries);

            describe["Tryit(Func<Task<>>, retries)"] = () =>
            {
                it["should return an FuncRetryBuilder<> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<string>>();

            };

            describe["Tryit(Func<Task<>>, retries).Go()"] = () =>
            {
                string result = null;
                before = () => result = null;
                act = () => result = subject.Go();
                it["should execute the task."] = () =>
                    taskExecuted.Should().BeTrue();

                it["should return the expected result"] = () =>
                    result.Should().Be(expectedResult);

                context["when the task is not started by the action"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () =>
                        {
                            return new Task<string>(() => { taskExecuted = true; return expectedResult; });
                        };
                    };

                    it["should start the task and run it"] = () =>
                        {
                            taskExecuted.Should().BeTrue();
                            result.Should().Be(expectedResult);
                        };
                };

                context["when the task fails the first time"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () =>
                        {
                            return new Task<string>(() =>
                            {
                                if (subject.LastRunner.Attempts == 1)
                                    throw new Exception("Nope!");

                                taskExecuted = true;
                                return expectedResult;
                            });
                        };
                    };
                    it["should try again"] = () =>
                        subject.Attempts.Should().Be(2);

                    it["Should execute the action succesfully"] = () =>

                    {
                        taskExecuted.Should().BeTrue();
                        result.Should().Be(expectedResult);
                    };

                    it["should set status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                };
            };

            describe["Tryit(Func<Task<>>, retries).WithSuccessPolicy().Go()"] = () =>
            {
                SuccessPolicyDelegate<string> successPolicy = null;
                bool successPolicyCalled = false;

                before = () =>
                {
                    successPolicyCalled = false;
                    successPolicy = (s, i) => { successPolicyCalled = true; };
                };

                act = () => subject.WithSuccessPolicy(successPolicy).Go();

                it["should call the SuccessPolicy delegate"] = () =>
                successPolicyCalled.Should().BeTrue();
            };

            describe["Tryit(Func<Task<>>, retries).GoAsync()"] = () =>
            {

                string result = null;
                before = () => result = null;
                actAsync = async () => result = await subject.GoAsync();

                it["should execute the task."] = () =>
                {
                    taskExecuted.Should().BeTrue();
                    result.Should().Be(expectedResult);
                };

                context["when the task is not started by the action"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () =>
                        {
                            return new Task<string>(() => { taskExecuted = true; return expectedResult; });
                        };
                    };

                    it["should start the task and run it"] = () =>
                    {
                        taskExecuted.Should().BeTrue();
                        result.Should().Be(expectedResult);
                    };
                };

                context["when the task fails the first time"] = () =>
                {
                    before = () =>
                    {
                        subjectFunc = () =>
                        {
                            return Task<string>.Factory.StartNew(() =>
                            {
                                if (subject.LastRunner.Attempts == 1)
                                    throw new Exception("Nope!");

                                taskExecuted = true;
                                return expectedResult;
                            });
                        };
                    };

                    it["should try again"] = () =>
                        subject.Attempts.Should().Be(2);

                    it["Should execute the action succesfully"] = () =>
                    {
                        taskExecuted.Should().BeTrue();
                        result.Should().Be(expectedResult);
                    };

                    it["should set status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);
                };

                describe["Tryit(Func<Task>, retries).ThenTry()"] = () =>
                {
                    FuncRetryBuilder<string> child = null;
                    before = () => child = null;
                    act = () => child = subject.ThenTry(retries);

                    it["should return an TaskTryItAndReturnResult instance"] = () =>
                        child.Should().BeOfType<FuncRetryBuilder<string>>();


                    describe["Tryit(Func<Task>, retries).ThenTry().Go()"] = () =>
                    {
                        before = () => result = null;
                        act = () => result = child.Go();

                        it["should execute the task."] = () =>
                        {
                            taskExecuted.Should().BeTrue();
                            result.Should().Be(expectedResult);
                        };

                        context["when the task is not started by the action"] = () =>
                        {
                            before = () =>
                            {
                                subjectFunc = () =>
                                {
                                    return new Task<string>(() => { taskExecuted = true; return expectedResult; });
                                };
                            };

                            it["should start the task and run it"] = () =>
                            {
                                taskExecuted.Should().BeTrue();
                                result.Should().Be(expectedResult);
                            };
                        };

                        context["when Try() fails on every attempt"] = () =>
                        {
                            before = () =>
                            {
                                subjectFunc = () => Task<string>.Factory.StartNew(() =>
                                {
                                    if (subject.LastRunner.Attempts == 0)
                                    {
                                        throw new Exception("Oooohhh child, things are going to get easier!");
                                    }
                                    return expectedResult;
                                });
                            };

                            it["should execute ThenTry()"] = () =>
                            {
                                subject.LastRunner.Attempts.Should().Be(1);
                                result.Should().Be(expectedResult);
                            };
                        };
                    };

                };
            };

            describe["Tryit(Func<Task<>>, retries).ThenTry(retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;
                act = () => child = subject.ThenTry(retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().Be(2);
            };

            describe["TryIt.Try(func<arg, Task<>>, retries).ThenTry(retries).Go()"] = () =>
            {
                string result = null;
                FuncRetryBuilder<string> child = null;

                before = () =>
                {
                    result = null;
                    child = null;
                };
                act = () =>
                {
                    child = subject.ThenTry(retries);
                    result = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         result.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectFunc = () => Task<string>.Factory.StartNew(() =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        });
                    };
                    it["should return the expected result"] = () =>
                            result.Should().Be(expectedResult);

                    it["subject should try once for each retry attempt"] = () =>
                        subject.Attempts.Should().Be(retries + 1);
                };
            };

        }

        void with_1_argument()
        {
            int arg = int.MinValue;
            int retries = 4;
            string expectedResult = "Hello from a Task!";
            Func<int, Task<string>> subjectAction = null;
            FuncRetryBuilder<string> subject = null;
            before = () =>
            {
                subject = null;
                subjectAction = (i) =>
                {
                    return Task<string>.Factory.StartNew(() => { return expectedResult; });
                };
            };
            act = () => subject = TryIt.TryTask(subjectAction, arg, retries);

            describe["TryIt.Try(func<arg, Task<>>, arg, retries)"] = () =>
            {
                it["should return an FuncRetryBuilder<> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<string>>();

                it["should set the arg internal property"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskWithResultRunner<int, string>>();
                    runner._arg.Should().Be(arg);
                };
            };

            describe["TryIt.Try(func<arg, Task<>>, arg, retries).Go()"] = () =>
            {
                string result = null;
                before = () => result = null;
                act = () => result = subject.Go();

                it["should return the expected result"] = () =>
                    result.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func<arg, Task<>>, arg, retries).ThenTry(arg, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg, retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

            };

            describe["TryIt.Try(func<arg, Task<>>, arg, retries).ThenTry(arg, retries).Go()"] = () =>
            {
                string result = null;
                FuncRetryBuilder<string> child = null;

                before = () =>
                {
                    result = null;
                    child = null;
                };
                act = () =>
                {
                    child = subject.ThenTry(arg, retries);
                    result = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         result.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectAction = (i) => Task<string>.Factory.StartNew(() =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        });
                    };
                    it["should return the expected result"] = () =>
                            result.Should().Be(expectedResult);

                    it["subject should try once for each retry attempt"] = () =>
                        subject.Attempts.Should().Be(retries + 1);
                };
            };

        }

        void with_2_arguments()
        {
            int arg1 = int.MinValue;
            double arg2 = Math.PI;

            int retries = 4;
            string expectedResult = "Hello from a Task!";
            Func<int, double, Task<string>> subjectAction = null;
            FuncRetryBuilder<string> subject = null;
            before = () =>
            {
                subject = null;
                subjectAction = (a1, a2) =>
                {
                    return Task<string>.Factory.StartNew(() => { return expectedResult; });
                };
            };
            act = () => subject = TryIt.TryTask(subjectAction, arg1, arg2, retries);

            describe["TryIt.Try(func<arg1, arg2, Task<>>, arg1, arg2, retries)"] = () =>
            {
                it["should return an FuncRetryBuilder<> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<string>>();

                it["should set the internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskWithResultRunner<int, double, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(func<arg1, arg2, Task<>>, arg1, arg2, retries).Go()"] = () =>
            {
                string result = null;
                before = () => result = null;
                act = () => result = subject.Go();

                it["should return the expected result"] = () =>
                    result.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func<arg1, arg2, Task<>>, arg1, arg2, retries).ThenTry(arg1, arg2, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg1, arg2, retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

            };

            describe["TryIt.Try(func<arg1, arg2, Task<>>, arg2, arg2, retries).ThenTry(arg1, arg2, retries).Go()"] = () =>
            {
                string result = null;
                FuncRetryBuilder<string> child = null;

                before = () =>
                {
                    result = null;
                    child = null;
                };
                act = () =>
                {
                    child = subject.ThenTry(arg1, arg2, retries);
                    result = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         result.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectAction = (a1, a2) => Task<string>.Factory.StartNew(() =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        });
                    };
                    it["should return the expected result"] = () =>
                           result.Should().Be(expectedResult);

                    //it["subject should try once for each retry attempt"] = () =>
                    //    subject.Attempts.Should().Be(retries + 1);
                };
            };

        }

        void with_3_arguments()
        {
            int arg1 = int.MinValue;
            double arg2 = Math.PI;
            string arg3 = "Just a thing";

            int retries = 4;
            string expectedResult = "Hello from a Task!";
            Func<int, double,string, Task<string>> subjectAction = null;
            FuncRetryBuilder<string> subject = null;
            before = () =>
            {
                subject = null;
                subjectAction = (a1, a2, a3) =>
                {
                    return Task<string>.Factory.StartNew(() => { return expectedResult; });
                };
            };
            act = () => subject = TryIt.TryTask(subjectAction, arg1, arg2, arg3, retries);

            describe["TryIt.Try(func<arg1, arg2, arg3, Task<>>, arg1, arg2, arg3, retries)"] = () =>
            {
                it["should return an FuncRetryBuilder<> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<string>>();

                it["should set the internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskWithResultRunner<int, double, string, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                };
            };

            describe["TryIt.Try(func<arg1, arg2, arg3, Task<>>, arg1, arg2, arg3, retries).Go()"] = () =>
            {
                string result = null;
                before = () => result = null;
                act = () => result = subject.Go();

                it["should return the expected result"] = () =>
                    result.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func<arg1, arg2, arg3, Task<>>, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

            };

            describe["TryIt.Try(func<arg1, arg2, arg3, Task<>>, arg2, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries).Go()"] = () =>
            {
                string result = null;
                FuncRetryBuilder<string> child = null;

                before = () =>
                {
                    result = null;
                    child = null;
                };
                act = () =>
                {
                    child = subject.ThenTry(arg1, arg2, arg3, retries);
                    result = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         result.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectAction = (a1, a2, a3) => Task<string>.Factory.StartNew(() =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        });
                    };
                    it["should return the expected result"] = () =>
                           result.Should().Be(expectedResult);

                    it["subject should try once for each retry attempt"] = () =>
                        subject.Attempts.Should().Be(retries + 1);
                };
            };

        }

        void with_4_arguments()
        {
            int arg1 = int.MinValue;
            double arg2 = Math.PI;
            string arg3 = "Just a thing";
            float arg4 = float.MaxValue;

            int retries = 4;
            string expectedResult = "Hello from a Task!";
            Func<int, double, string, float, Task<string>> subjectAction = null;
            FuncRetryBuilder<string> subject = null;
            before = () =>
            {
                subject = null;
                subjectAction = (a1, a2, a3, a4) =>
                {
                    return Task<string>.Factory.StartNew(() => { return expectedResult; });
                };
                act = () => subject = TryIt.TryTask(subjectAction, arg1, arg2, arg3, arg4, retries);
            };

            describe["TryIt.Try(func<arg1, arg2, arg3, arg4, Task<>>, arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                it["should return an FuncRetryBuilder<> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<string>>();

                it["should set the internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<TaskWithResultRunner<int, double, string, float, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                    runner._arg4.Should().Be(arg4);
                };
            };

            describe["TryIt.Try(func<arg1, arg2, arg3, arg4, Task<>>, arg1, arg2, arg3, arg4, retries).Go()"] = () =>
            {
                string result = null;
                before = () => result = null;
                act = () => result = subject.Go();

                it["should return the expected result"] = () =>
                    result.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func<arg1, arg2, arg3, arg4, Task<>>, arg1, arg2, arg3, arg4, retries).ThenTry(arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

            };

            describe["TryIt.Try(func<arg1, arg2, arg3, arg4, Task<>>, arg2, arg2, arg3, arg4, retries).ThenTry(arg1, arg2, arg3, retries).Go()"] = () =>
            {
                string result = null;
                FuncRetryBuilder<string> child = null;

                before = () =>
                {
                    result = null;
                    child = null;
                };
                act = () =>
                {
                    child = subject.ThenTry(arg1, arg2, arg3, arg4, retries);
                    result = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         result.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectAction = (a1, a2, a3, a4) => Task<string>.Factory.StartNew(() =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        });
                    };
                    it["should return the expected result"] = () =>
                           result.Should().Be(expectedResult);

                    it["subject should try once for each retry attempt"] = () =>
                        subject.Attempts.Should().Be(retries + 1);
                };
            };

        }

    }
}
