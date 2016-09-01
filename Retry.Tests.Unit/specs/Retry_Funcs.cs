using Moq;
using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Builders;
using Retry.Runners;

namespace Retry.Tests.Unit.specs
{
    class Retry_Funcs : nspec
    {
        void with_no_arguments()
        {
            FuncRetryBuilder<string> subject = null;
            Func<string> subjectFunc = null;
            string subjectResult = null;
            string expectedResult = null;
            int retries = default(int);
            Exception thrown = null;

            beforeAll = () =>
            {
                subjectResult = null;
                expectedResult = "Hello!";
                retries = 3;
                thrown = null;
                subjectFunc = () =>
                {
                    return expectedResult;
                };

            };

            act = () => subject = TryIt.Try(subjectFunc, retries);


            describe["TryIt.Try(func, retries).WithErrorPolicy()"] = () =>
            {
                ErrorPolicyDelegate errorDelegate = (ex, i) => { return true; };
                object errorPolicyResult = null;
                before = () => errorPolicyResult = null;
                act = () => errorPolicyResult = subject.WithErrorPolicy(errorDelegate);

                it["should set the internal ErrorPolicy property"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(errorDelegate);

                it["should return the subject"] = () =>
                    subject.Should().BeSameAs(errorPolicyResult);

                describe["TryIt.Try(func, retries).WithErrorPolicy(ErrorPolicyDelegate).Go()"] = () =>
                {
                    int errorDelegateCallCount = default(int);
                    before = () => errorDelegateCallCount = default(int);
                    act = () =>
                    {
                        try
                        {
                            subject.Go();
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };

                    context["when there are no exceptions to test"] = () =>
                    {
                        before = () => errorDelegate = (ex, i) => { errorDelegateCallCount++; return true; };
                        it["ErrorPolicyDelegate should never be called"] = () =>
                            errorDelegateCallCount.Should().Be(0);
                    };

                    context["when all exceptions pass the policy set by the delegate"] = () =>
                    {
                        before = () =>
                        {
                            errorDelegate = (ex, i) => { errorDelegateCallCount++; return true; };
                            subjectFunc = () => { throw new Exception(); };
                        };

                        it["should throw RetryFailedException"] = () =>
                            thrown.Should().BeOfType<RetryFailedException>();

                        it["should check the ErrorPolicyDelegate once for everey failed attempt"] = () =>
                        {
                            errorDelegateCallCount.Should().NotBe(0);
                            errorDelegateCallCount.Should().Be(subject.Attempts);
                        };
                    };

                    context["when the exception fails the policy set by the delegate and the delegate returns false"] = () =>
                    {
                        Exception expectedException = new Exception("I Failed!");
                        before = () =>
                        {
                            subjectFunc = () => { throw expectedException; };
                            errorDelegate = (ex, i) => { return false; };
                        };

                        it["should set the status to Failed"] = () =>
                            subject.Status.Should().Be(RetryStatus.Fail);

                        it["should try only once"] = () =>
                            subject.Attempts.Should().Be(1);

                        it["should throw the RetryFailedException"] = () =>
                            thrown.Should().BeOfType<RetryFailedException>();

                        it["ExceptionList should contain an ErrorPolicyException exception"] = () =>
                            thrown.As<RetryFailedException>()
                            .ExceptionList.FirstOrDefault(x => x.GetType() == typeof(ErrorPolicyException))
                            .Should().NotBeNull();

                        it["ErrorPolicyException's inner exception should be the expected exception"] = () =>
                             thrown.As<RetryFailedException>()
                            .ExceptionList.First(x => x.GetType() == typeof(ErrorPolicyException))
                            .As<ErrorPolicyException>()
                            .InnerException.Should().BeSameAs(expectedException);

                    };

                    context["when the delegate throws an exception"] = () =>
                    {
                        Exception expectedException = new InvalidOperationException("Can't do that!");
                        before = () =>
                        {
                            subjectFunc = () => { throw new Exception(); };
                            errorDelegate = (ex, i) => { throw expectedException; };
                        };

                        it["should throw the exception"] = () =>
                            thrown.Should().Be(expectedException);
                    };

                };
            };

            describe["TryIt.Try(func, retries).OnSuccess()"] = () =>
            {
                OnSuccessDelegate<string> successDelegate = null;
                object onSuccessResult = null;
                beforeAll = () => successDelegate = (i, r) => { };
                act = () => onSuccessResult = subject.OnSuccess(successDelegate);

                it["should set the internal onSuccess property"] = () =>
                    subject.LastRunner.OnSuccess.Should().BeSameAs(successDelegate);

                it["should return the subject"] = () =>
                    subject.Should().BeSameAs(onSuccessResult);

                describe["TryIt.Try(func, retries).OnSuccess(onSuccessDelegate<T>).Go()"] = () =>
                {
                    object testResult = null;
                    object result = null;
                    bool onSuccessCalled = false;
                    beforeAll = () =>
                    {
                        subjectFunc = () => expectedResult;
                        testResult = null;
                        result = null;
                        onSuccessCalled = false;
                        successDelegate = (i, r) => { testResult = r; onSuccessCalled = true; };
                    };
                    act = () =>
                    {
                        subject = TryIt.Try(subjectFunc, retries);
                        try
                        {
                            result = subject.OnSuccess(successDelegate).Go();
                        }
                        catch (Exception ex)
                        {

                            thrown = ex;
                        }
                    };

                    it["should call the onSuccessDelegate"] = () =>
                        onSuccessCalled.Should().BeTrue();

                    it["should pass the result to the OnSuccess delegate"] = () =>
                        testResult.Should().Be(expectedResult);

                    it["should return the expected result"] = () =>
                        result.Should().Be(expectedResult);


                    it["should set the status to Success"] = () =>
                        subject.Status.Should().Be(RetryStatus.Success);

                    context["when the OnSuccess delegate throws an exception (policy fail)"] = () =>
                    {

                        Exception expectedException = new NotImplementedException();

                        before = () =>
                        {
                            successDelegate = (i, r) =>
                            {
                                if (subject.LastRunner.ExceptionList.Count == 0)
                                    throw expectedException;
                            };
                        };

                        it["should put the exception into the exceptions list"] = () =>
                            subject.ExceptionList.Should().Contain(expectedException);

                        it["should set status to SuccessAfterRetries"] = () =>
                            subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);
                    };

                    context["when every successful attempt throws an exception (policy fail)"] = () =>
                    {

                        Exception expectedException = new NotImplementedException();

                        before = () =>
                        {
                            successDelegate = (i, r) =>
                            {
                                throw expectedException;
                            };
                        };

                        it["should set status to Fail"] = () =>
                            subject.Status.Should().Be(RetryStatus.Fail);

                        it["every policy exception should be in the ExceptionList"] = () =>
                            subject.ExceptionList.Where(x => x == expectedException)
                                .Count().Should().Be(subject.ExceptionList.Count);

                        it["should throw RetryFailedException"] = () =>
                            thrown.Should().BeOfType<RetryFailedException>();

                    };
                };

            };

            describe["TryIt.Try(func, retries).UsingDelay(delay)"] = () =>
            {
                Mock<IDelay> mockDelay = null;
                IDelay newPause = null;
                FuncRetryBuilder<string> result = null;
                before = () =>
                {
                    result = null;
                    mockDelay = new Mock<IDelay>();
                    newPause = mockDelay.Object;
                };

                act = () =>
                {
                    result = subject.UsingDelay(newPause);
                };

                it["Should set the Delay property"] = () =>
                    subject.LastRunner.Delay.Should().Be(newPause);

                it["should return the subject"] = () =>
                    result.Should().Be(subject);
            };

            describe["TryIt.Try(func, retries).Go()"] = () =>
            {

                act = () =>
                {
                    subjectResult = subject.Go();
                };

                it["should return the result of calling the Func"] = () =>
                    subjectResult.Should().Be(expectedResult);

                it["should set status to RetryStatus.Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

                it["should have an empty ExceptionList"] = () =>
                    subject.ExceptionList.Should().BeEmpty();

                context["when retries is an invalid value"] = () =>
                {
                    Action action = () => TryIt.Try(subjectFunc, 0);

                    it["should throw an ArgumentOutOfRangeException"] = () =>
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                };

                context["when every attempt fails"] = () =>
                {
                    act = () =>
                    {
                        subjectFunc = () =>
                        {
                            throw new Exception("I tried. I failed.");
                        };
                        subject = TryIt.Try(subjectFunc, retries);
                    };
                    it["should throw RetryFailedException exception"] = () =>
                        subject.Invoking(s => s.Go()).ShouldThrow<RetryFailedException>();
                };
            };

            describe["TryIt.Try(func, retries).ThenTry(retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = subject.ThenTry(retries);

                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                describe["TryIt.Try(func, retries).ThenTry(retries).Go()"] = () =>
                {
                    context["When initial try fails, should excecute ThenTry()"] = () =>
                    {
                        string altSubjectResult = null;
                        string altExpectedResult = "World!";
                        int funcExecutedCount = default(int);
                        beforeAll = () =>
                        {
                            altSubjectResult = null;
                            funcExecutedCount = default(int);
                            altSubjectResult = null;
                            subjectFunc = () =>
                            {
                                funcExecutedCount++;
                                if (funcExecutedCount <= retries)
                                    throw new Exception("Bad input!");

                                return altExpectedResult;
                            };
                        };

                        act = () =>
                        {
                            funcExecutedCount = default(int);
                            subject = TryIt.Try(subjectFunc, retries);
                            child = subject.ThenTry(retries);
                            child.Go();
                        };


                        it["should return the result of Func"] = () =>
                            altSubjectResult.Should().Be(altSubjectResult);

                        it["should set the Status to SuccessAfterRetries"] = () =>
                            child.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    };
                };
            };

        }

        void with_1_argument()
        {
            FuncRetryBuilder<string> subject = null;
            Func<int, string> subjectFunc = null;
            int arg = 23;
            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i) => { return expectedResult; };

            };

            act = () => subject = TryIt.Try(subjectFunc, arg, retries);
            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set the arg internal property"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, string>>();
                    runner._arg.Should().Be(arg);
                };

            };

            describe["TryIt.Try(func, arg, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func, arg, retries).ThenTry(arg, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg, retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should have 2 runners"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["should set the internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, string>>();
                    runner._arg.Should().Be(arg);
                };
            };

            describe["TryIt.Try(func, arg, retries).ThenTry(arg, retries).Go()"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () =>
                {
                    child = subject.ThenTry(arg, retries);
                    actualResult = child.Go();
                };

                context["when Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         actualResult.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when Try fails but ThenTry succeeds"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectFunc = (i) =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        };
                    };
                    it["should return the expected result"] = () =>
                            actualResult.Should().Be(expectedResult);

                    it["should set Attempts once for every attempt"] = () =>
                        subject.Attempts.Should().Be(retries + 1);

                };
            };

            describe["TryIt.Try(func, retries).ThenTry(altFunc, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;
                Func<int, string> altFunc = (a1) => { return "Hello world!"; };
                act = () =>
                {
                    subject = TryIt.Try(subjectFunc, arg, retries);
                    child = subject.ThenTry(altFunc, arg, retries);
                };

                it["ThenTry() should use the alternate func"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altFunc);

                context["TryIt.Try(func, retries).ThenTry(altFunc, retries).Go()"] = () =>
                {
                    int altCalled = default(int);
                    before = () =>
                    {
                        altFunc = (a1) => { altCalled++; return "SWAK!"; };
                        subjectFunc = (a1) => { throw new Exception("You killed my father. Prepare to die!"); };
                        altCalled = default(int);
                    };
                    act = () =>
                    {
                        child.Go();
                    };

                    it["ThenTry() should have called the alternate action"] = () =>
                        altCalled.Should().Be(1);
                };
            };

        }

        void with_2_arguments()
        {
            FuncRetryBuilder<string> subject = null;
            Func<int, double, string> subjectFunc = null;
            int arg1 = 23;
            double arg2 = Math.E;
            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i, d) => { return expectedResult; };
            };

            act = () => subject = TryIt.Try(subjectFunc, arg1, arg2, retries);

            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set arg1 and arg2 internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, double, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                };

            };

            describe["TryIt.Try(func, arg1, arg2, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func, arg1, arg2, retries).ThenTry(arg1, arg2, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg1, arg2, retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should have 2 runners"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["should set arg1 and arg2 internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<FuncRunner<int, double, string>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(func, arg1, arg2, retries).ThenTry(arg1, arg2, retries).Go()"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () =>
                {
                    child = subject.ThenTry(arg1, arg2, retries);
                    actualResult = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         actualResult.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectFunc = (a1, a2) =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        };
                    };
                    it["should return the expected result"] = () =>
                            actualResult.Should().Be(expectedResult);

                    it["should set Attempts once for every attempt"] = () =>
                        subject.Attempts.Should().Be(retries + 1);
                };
            };
        }

        void with_3_arguments()
        {
            FuncRetryBuilder<string> subject = null;
            Func<int, double, long, string> subjectFunc = null;

            int arg1 = 23;
            double arg2 = Math.E;
            long arg3 = long.MinValue;

            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i, d, l) => { return expectedResult; };
            };

            act = () => subject = TryIt.Try(subjectFunc, arg1, arg2, arg3, retries);

            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an FuncRetryBuilder<TResult> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<string>>();


                it["should set arg1 - arg3 internal properties"] = () =>
                {
                    var Runner = subject.LastRunner.As<FuncRunner<int, double, long, string>>();
                    Runner._arg1.Should().Be(arg1);
                    Runner._arg2.Should().Be(arg2);
                    Runner._arg3.Should().Be(arg3);
                };

            };

            describe["TryIt.Try(func, arg1, arg2, arg3, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, retries);

                it["should return te subject"] = () =>
                    child.Should().Be(subject);
            };

            describe["TryIt.Try(func, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries).Go()"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () =>
                {
                    child = subject.ThenTry(arg1, arg2, arg3, retries);
                    actualResult = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         actualResult.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectFunc = (a1, a2, a3) =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        };
                    };
                    it["should return the expected result"] = () =>
                            actualResult.Should().Be(expectedResult);

                    it["should have tried once for each retry"] = () =>
                        subject.Attempts.Should().Be(retries + 1);
                };
            };
        }

        void with_4_arguments()
        {
            FuncRetryBuilder<string> subject = null;
            Func<int, double, long, string, string> subjectFunc = null;

            int arg1 = 23;
            double arg2 = Math.E;
            long arg3 = long.MinValue;
            string arg4 = "Happy to be here!";

            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i, d, l, s) => { return expectedResult; };
            };

            act = () => subject = TryIt.Try(subjectFunc, arg1, arg2, arg3, arg4, retries);

            describe["TryIt.Try(func, arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set arg1 - arg4 internal properties"] = () =>
                {
                    var asTryIt = subject.LastRunner.As<FuncRunner<int, double, long, string, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                };

            };

            describe["TryIt.Try(func, arg1, arg2, arg3, arg4, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(func, arg1, arg2, arg3, arg4, retries).ThenTry(arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, retries);

                it["should return the subject"] = () =>
                    child.Should().NotBeNull();
            };

            describe["TryIt.Try(func, arg1, arg2, arg3, arg4, retries).ThenTry(arg1, arg2, arg3, arg4, retries).Go()"] = () =>
            {
                FuncRetryBuilder<string> child = null;

                before = () => child = null;
                act = () =>
                {
                    child = subject.ThenTry(arg1, arg2, arg3, arg4, retries);
                    actualResult = child.Go();
                };

                context["when the parent Try succeeds"] = () =>
                {
                    it["should return the expected result"] = () =>
                         actualResult.Should().Be(expectedResult);

                    it["The subject should have tried once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the parent Try fails"] = () =>
                {
                    int funcAttempts = default(int);

                    before = () =>
                    {
                        funcAttempts = 0;
                        subjectFunc = (a1, a2, a3, a4) =>
                        {
                            funcAttempts++;
                            if (funcAttempts <= retries)
                                throw new Exception("I didn't mean it!");

                            return expectedResult;
                        };
                    };
                    it["should return the expected result"] = () =>
                            actualResult.Should().Be(expectedResult);

                    it["the parent should have tried once for each retry"] = () =>
                        subject.Attempts.Should().Be(retries + 1);
                };
            };
        }

    }
}
