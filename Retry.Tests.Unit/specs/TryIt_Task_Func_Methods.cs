using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Retry.Tests.Unit.specs
{
    class TryIt_Task_Func_Methods : nspec
    {
        void Static_Func_Task_TryIt_Method()
        {
            int retries = 3;
            bool taskExecuted = false;
            string expectedResult = "Hello from a Task!";
            Func<Task<string>> subjectAction = null;
            ITryAndReturnValue<string> subject = null;
            before = () =>
            {
                taskExecuted = false;
                subject = null;
                subjectAction = () =>
                {
                    return Task<string>.Factory.StartNew(() => { taskExecuted = true; return expectedResult; });
                };
            };
            act = () => subject = TryIt.Try(subjectAction, retries);

            describe["Tryit(Func<Task<>>, retries)"] = () =>
            {
                it["should return an TaskTryItAndReturnResult instance"] = () =>
                    subject.Should().BeOfType<TaskTryItAndReturnResult<string>>();

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
                        subjectAction = () =>
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
                        subjectAction = () =>
                        {
                            return new Task<string>(() =>
                            {
                                if (subject.Attempts == 1)
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

            describe["Tryit(Func<Task<>>, retries).OnSuccess().Go()"] = () =>
            {
                OnSuccessDelegate<string> onSuccess = null;
                bool onSuccessCalled = false;

                before = () =>
                {
                    onSuccessCalled = false;
                    onSuccess = (i, s) => { onSuccessCalled = true; };
                };

                act = () => subject.OnSuccess(onSuccess).Go();

                it["should call the OnSuccess delegate"] = () =>
                onSuccessCalled.Should().BeTrue();
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
                        subjectAction = () =>
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
                        subjectAction = () =>
                        {
                            return Task<string>.Factory.StartNew(() =>
                            {
                                if (subject.Attempts == 1)
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

            describe["Tryit(Func<Task>, retries).ThenTry()"] = () =>
            {
                ITryAndReturnValue<string> child = null;
                before = () => child = null;
                act = () => child = subject.ThenTry(retries);

                it["should return an TaskTryItAndReturnResult instance"] = () =>
                    child.Should().BeOfType<TaskTryItAndReturnResult<string>>();


                describe["Tryit(Func<Task>, retries).ThenTry().Go()"] = () =>
                {
                    string result = null;
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
                            subjectAction = () =>
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
                            subjectAction = () => Task<string>.Factory.StartNew(() =>
                            {
                                if (child.Attempts == 0)
                                {
                                    throw new Exception("Oooohhh child, things are going to get easier!");
                                }
                                return expectedResult;
                            });
                        };

                        it["should execute ThenTry()"] = () =>
                        {
                            child.Attempts.Should().Be(1);
                            result.Should().Be(expectedResult);
                        };
                    };
                };

            };

        }

        void Static_Func_T_Task_TryIt_Method()
        {
            int arg = int.MinValue;
            int retries = 42;
            string expectedResult = "Hello from a Task!";
            Func<int, Task<string>> subjectAction = null;
            ITryAndReturnValue<string> subject = null;
            before = () =>
            {
                subject = null;
                subjectAction = (i) =>
                {
                    return Task<string>.Factory.StartNew(() => { return expectedResult; });
                };
                act = () => subject = TryIt.Try(subjectAction, arg, retries);
            };

            describe["TryIt.Try(func<arg, Task<>>, arg, retries)"] = () =>
            {
                it["should return an TaskTryItAndReturnResult<T, TResult> instance"] = () =>
                    subject.Should().BeOfType<TaskTryItAndReturnResult<int, string>>();

                it["should set the arg internal property"] = () =>
                {
                    var asTryIt = subject.As<TaskTryItAndReturnResult<int, string>>();
                    asTryIt._arg.Should().Be(arg);
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
                ITryAndReturnValue<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg, retries);

                it["should return a child TaskTryItAndReturnResult<T, TResult> instance"] = () =>
                    child.Should().BeOfType<TaskTryItAndReturnResult<int, string>>();

                it["the child should be distinct from the parent"] = () =>
                    child.Should().NotBe(subject);

            };

            describe["TryIt.Try(func<arg, Task<>>, arg, retries).ThenTry(arg, retries).Go()"] = () =>
            {
                string result = null;
                ITryAndReturnValue<string> child = null;

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

                    it["The child should not have been tried"] = () =>
                        child.Attempts.Should().Be(0);
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

                    it["the parent should have tried once for each retry"] = () =>
                        subject.Attempts.Should().Be(retries);

                    it["the result should have come from the child and not from the parent"] = () =>
                    {
                        subject.As<TaskTryItAndReturnResult<int, string>>().HasResult.Should().BeFalse();
                        child.As<TaskTryItAndReturnResult<int, string>>().HasResult.Should().BeTrue();
                    };
                };
            };

        }
    }
}
