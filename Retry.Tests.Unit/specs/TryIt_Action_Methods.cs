using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace Retry.Tests.Unit.specs
{

    class TryIt_Action_Methods : nspec
    {
        void Static_Action_TryIt_Methods()
        {
            ITry subject = null;
            Action subjectAction = null;
            int actionExecutionCount = default(int);
            int retries = default(int);
            Exception thrown = null;

            beforeAll = () =>
            {
                actionExecutionCount = 0;
                retries = 3;
                thrown = null;
                subjectAction = () => actionExecutionCount++;
            };

            before = () =>
            {
                thrown = null;
                subject = TryIt.Try(subjectAction, retries);
            };


            describe["TryIt.Try(Action action, int retries)"] = () =>
            {

                it["should return an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                context["when the action is null"] = () =>
                {
                    before = () =>
                    {
                        try
                        {
                            TryIt.Try((Action)null, retries);
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };
                    it["should throw ArgumentNullException"] = () =>
                        thrown.Should().BeOfType<ArgumentNullException>();
                };

            };

            describe["TryIt.Try(Action action, int retries).OnError(OnErrorDelegate)"] = () =>
            {
                OnErrorDelegate errorDelegate = (ex, i) => { return true; };

                act = () => subject = subject.OnError(errorDelegate);

                it["should set the internal onError property"] = () =>
                    subject.As<IInternalAccessor>().OnError.Should().BeSameAs(errorDelegate);

                it["should return an ITry instance"] = () =>
                    subject.Should().NotBeNull();

            };

            describe["TryIt.Try(Action action, int retries).OnSuccess(OnSuccessDelegate)"] = () =>
            {
                OnSuccessDelegate successDelegate = (i) => { };

                act = () => subject = subject.OnSuccess(successDelegate);

                it["should set the internal onSuccess property"] = () =>
                    subject.As<IInternalAccessor>().OnSuccess.Should().BeSameAs(successDelegate);

            };

            describe["TryIt.Try(Action action, int retries).UsingDelay(Delay delay)"] = () =>
            {
                Delay newPause = null;
                ITry result = null;
                before = () =>
                {
                    result = null;
                    newPause = new Mock<Delay>().Object;
                };

                act = () =>
                {
                    try
                    {
                        result = subject.UsingDelay(newPause);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                it["Should set the Delay property"] = () =>
                    subject.Delay.Should().Be(newPause);

                it["should return the subject"] = () =>
                    result.Should().Be(subject);

                it["should not throw any exceptions"] = () =>
                    thrown.Should().BeNull();

                context["when the Delay parameter is null"] = () =>
                {
                    before = () => newPause = null;

                    it["should throw an ArgumentNullException"] = () =>
                        thrown.Should().BeOfType<ArgumentNullException>();

                };
            };

            describe["TryIt.Try(Action action, int retries).Go()"] = () =>
            {

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

                it["should call the action"] = () =>
                    actionExecutionCount.Should().Be(1);

                it["should have attempted the action only once"] = () =>
                    subject.Attempts.Should().Be(1);

                it["should set status to RetryStatus.Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

                it["should have an empty ExceptionList"] = () =>
                    subject.ExceptionList.Should().BeEmpty();

                context["when Try().Go() fails the first couple of times"] = () =>
                {
                    beforeAll = () =>
                    {
                        subjectAction = () =>
                        {
                            actionExecutionCount++;
                            if (actionExecutionCount < 2)
                                throw new Exception("Bad input!");
                        };
                    };

                    before = () => actionExecutionCount = 0;

                    it["should set the status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    it["should record the number of attempts Try made"] = () =>
                        subject.Attempts.Should().Be(actionExecutionCount);

                    it["should contain an exception for every failed attempt"] = () =>
                        subject.ExceptionList.Count.Should().Be(subject.Attempts - 1);

                    it["subject.GetAllExceptions() should return the same exceptions in subject.ExceptionList"] = () =>
                        subject.GetAllExceptions().Should().BeEquivalentTo(subject.ExceptionList);

                    describe["Try().OnError().Go"] = () =>
                    {
                        Exception delegateError = null;
                        int errorTryCount = default(int);
                        bool onErrorHasBeenCalled = false;

                        OnErrorDelegate errorDelegate = null;
                        before = () =>
                        {
                            onErrorHasBeenCalled = false;
                            delegateError = null;
                            errorTryCount = 0;

                            errorDelegate = (e, i) =>
                            {
                                onErrorHasBeenCalled = true;
                                errorTryCount = i;
                                delegateError = e;
                                return true;
                            };

                            subject.OnError(errorDelegate);
                        };

                        it["should call the OnError delegate"] = () =>
                            onErrorHasBeenCalled.Should().BeTrue();

                        context["when OnError() returns false"] = () =>
                        {
                            Exception expectedException = new Exception("An unexpected Journey");

                            beforeAll = () =>
                            {
                                subjectAction = () => { throw expectedException; };
                            };
                            before = () =>
                            {
                                errorDelegate = (e, i) =>
                                {
                                    errorTryCount = i;
                                    return false;
                                };

                                subject.OnError(errorDelegate);
                            };

                            it["should throw the exception"] = () =>
                                thrown.Should().Be(expectedException);

                        };
                    };
                };

                context["when Try().Go() fails on every attempt"] = () =>
                {
                    beforeAll = () =>
                    {
                        subjectAction = () => { throw new Exception("Say WHAT!?!"); };
                    };

                    it["should have attempted the action asmay times aa RetryCount"] = () =>
                        subject.Attempts.Should().Be(subject.RetryCount);

                    it["should contain an exception for evry attempt"] = () =>
                        subject.ExceptionList.Count.Should().Be(subject.Attempts);

                    it["should throw RetryFailedException with all the subject exceptions"] = () =>
                        {
                            thrown.Should().BeOfType<RetryFailedException>();
                            thrown.As<RetryFailedException>().ExceptionList.ShouldBeEquivalentTo(subject.ExceptionList);
                        };

                    it["subject.GetAllExceptions() should return the same exceptions in subject.ExceptionList"] = () =>
                        subject.GetAllExceptions().Should().BeEquivalentTo(subject.ExceptionList);
                };

                context["when retries is an invalid value"] = () =>
                {
                    Action action = () => TryIt.Try(subjectAction, 0);
                    it["should throw an ArgumentOutOfRangeException"] = () =>
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["TryIt.Try(action, retries).UsingDelay(delay).Go()"] = () =>
            {
                Mock<IDelay> newPause = null;
                before = () =>
                {
                    thrown = null;
                    subjectAction = () => { throw new Exception("You killed my father. Prepare to die!"); };
                    newPause = new Mock<IDelay>();
                };

                act = () =>
                {
                    try
                    {
                        subject.UsingDelay(newPause.Object);
                        subject.Go();
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };
                it["should call the provided delay every time Try() fails (less 1"] = () =>
                    newPause.Verify(m => m.WaitAsync(It.IsAny<int>()), Times.Exactly(retries - 1));

            };

            describe["TryIt.Try(action, retries).ThenTry(retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();


                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                context["when the parent has OnError() set"] = () =>
                {
                    OnErrorDelegate onError = (i, e) => { return true; };
                    before = () =>
                    {
                        child = subject.OnError(onError).ThenTry(3);
                    };

                    it["should use the OnError delegate of the parent"] = () =>
                        child.As<TryItBase>().ParentOrSelf((x) => x.OnError != null).OnError.Should().Be(onError);
                };

                describe["TryIt.Try(action, retries).ThenTry(retries).Go()"] = () =>
                {
                    beforeAll = () =>
                    {
                        thrown = null;
                        subjectAction = () => { };
                    };
                   
                    act = () =>
                    {
                        try
                        {
                            child.Go();
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };

                    context["when the first try succeeds"] = () =>
                    {
                        it["should not ever execute ThenTry"] = () =>
                          child.Attempts.Should().Be(0);

                        it["Try should have executed once"] = () =>
                            subject.Attempts.Should().Be(1);

                        it["should have no exceptions"] = () =>
                            child.GetAllExceptions().Count.Should().Be(0);

                    };

                    context["when Try() fails but ThenTry() succeeds"] = () =>
                    {
                        int errorCount = default(int);
                        before = () => errorCount = default(int);

                        beforeAll = () =>
                        {
                            subjectAction = () =>
                              {
                                  errorCount++;
                                  if (errorCount <= retries)
                                      throw new Exception("The train!");
                              };
                        };

                        it["Try() should fail every time"] = () =>
                            subject.Attempts.Should().Be(retries);

                        it["Try() status should be Fail"] = () =>
                            subject.Status.Should().Be(RetryStatus.Fail);

                        it["Try() should contain an exception for every failure"] = () =>
                            subject.ExceptionList.Count.Should().Be(retries);

                        it["ThenTry() should succeed after the first try"] = () =>
                            child.Attempts.Should().Be(1);

                        it["ThenTry() should set status to SuccessAfterRetries"] = () =>
                            child.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                        it["ThenTry() should contain no exceptions"] = () =>
                            child.ExceptionList.Count.Should().Be(0);

                        it["child.GetAllExceptions() should contain all the subject errors"] = () =>
                            child.GetAllExceptions().Should().BeEquivalentTo(subject.ExceptionList);

                    };

                    context["when both Try() and ThenTry() fail after every attempt"] = () =>
                    {
                        beforeAll = () =>
                        {
                            subjectAction = () => { throw new Exception("Goodbye cruel world!"); };
                            subject = TryIt.Try(subjectAction, retries);
                        };
  
                        it["Try() should have an exception for every attempt"] = () =>
                            subject.ExceptionList.Count.Should().Be(retries);

                        it["ThenTry() should have an exception for every attempt"] = () =>
                            child.ExceptionList.Count.Should().Be(retries);

                        it["ThenTry().GetAllExceptions() should have all the exceptions from both Try() and ThenTry()"] = () =>
                            child.GetAllExceptions().Count.Should().Be(retries * 2);


                    };
                };

                describe["TryIt.Try(action, retries).OnError(delegate).ThenTry(retries).Go()"] = () =>
                {
                    OnErrorDelegate onError = null;
                    before = () => subjectAction = () => { };
                    act = () =>
                    {
                        subject = TryIt.Try(subjectAction, retries).OnError(onError);
                        child = subject.ThenTry(retries);
                        try
                        {
                            child.Go();
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };
                    context["when Try() succeeds"] = () =>
                    {
                        int onErrorCalled = default(int);
                        before = () =>
                        {
                            thrown = null;
                            onErrorCalled = default(int);
                            onError = (e, i) => { onErrorCalled++; return true; };
                        };

                        it["should succeed without any failures and a status of Success"] = () =>
                        {
                            onErrorCalled.Should().Be(0);
                            child.ExceptionList.Count.Should().Be(0);
                            subject.Status.Should().Be(RetryStatus.Success);
                            child.Status.Should().Be(RetryStatus.Success);
                        };
                    };

                    context["when Try().ThenTry() both fail"] = () =>
                    {
                        int onErrorCalled = default(int);

                        before = () =>
                        {
                            thrown = null;
                            onErrorCalled = default(int);
                            subjectAction = () => { throw new Exception("Welcome to the Machine!"); };
                            onError = (e, i) => { onErrorCalled++; return true; };
                        };

                        it["should call onError for both Try() and ThenTry()"] = () =>
                            onErrorCalled.Should().Be(retries * 2);

                        it["should throw RetryFailedException"] = () =>
                            thrown.Should().BeOfType<RetryFailedException>();
                    };

                    context["When OnError decides Try() should not continue"] = () =>
                    {
                        var expectedException = new Exception("Woah! What happened?");
                        before = () =>
                        {
                            subjectAction = () => { throw expectedException; };
                            onError = (e, i) => { return false; };
                        };

                        it["should throw the exception caught by OnError"] = () =>
                        thrown.Should().Be(expectedException);

                        it["status should be Fail"] = () =>
                        {
                            child.Status.Should().Be(RetryStatus.Fail);
                            subject.Status.Should().Be(RetryStatus.Fail);
                        };
                    };

                    context["when oError throws an exception"] = () =>
                    {
                        var expectedException = new Exception("I'm a new exception.");
                        before = () =>
                        {
                            subjectAction = () => { throw new Exception("Woah! What happened?"); };
                            onError = (e, i) => { throw expectedException; };
                        };

                        it["should throw the exception thrown by OnError"] = () =>
                        thrown.Should().Be(expectedException);

                        it["status should be Fail"] = () =>
                        {
                            child.Status.Should().Be(RetryStatus.Fail);
                            subject.Status.Should().Be(RetryStatus.Fail);
                        };
                    };

                };

                describe["TryIt.Try(action, retries).OnSuccess(delegate).ThenTry(retries).Go()"] = () =>
                {
                    OnSuccessDelegate onSuccess = null;
                    act = () =>
                    {
                        subjectAction = () => { };
                        subject = TryIt.Try(subjectAction, retries).OnSuccess(onSuccess);
                        child = subject.ThenTry(retries);
                        try
                        {
                            child.Go();
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };
                    context["when OnSuccess does not throw"] = () =>
                    {
                        int onSuccessCalled = default(int);
                        before = () =>
                        {
                            thrown = null;
                            onSuccessCalled = default(int);
                            onSuccess = (i) => { onSuccessCalled = i; };
                        };

                        it["should succeed without any failures and a status of Success"] = () =>
                        {
                            onSuccessCalled.Should().Be(1);
                            child.ExceptionList.Count.Should().Be(0);
                            subject.Status.Should().Be(RetryStatus.Success);
                            child.Status.Should().Be(RetryStatus.Success);
                        };
                    };

                    context["when OnSuccess throws an exception"] = () =>
                    {
                        int onSuccessCalled = default(int);
                        before = () =>
                        {
                            thrown = null;
                            onSuccessCalled = default(int);
                            onSuccess = (i) => { onSuccessCalled++; throw new Exception("BARF!!"); };
                        };
                        it["should set a status of Fail"] = () =>
                        {
                            subject.Status.Should().Be(RetryStatus.Fail);
                            child.Status.Should().Be(RetryStatus.Fail);
                        };

                        it["should call OnSuccess both in Try() and ThenTry()"] = () =>
                            onSuccessCalled.Should().Be(retries * 2);

                        it["should raise RetryFailedException"] = () =>
                            thrown.Should().BeOfType<RetryFailedException>();
                    };
                };

            };

        }

        void Static_Action_T_TryIt_Methods()
        {
            ITry subject = null;
            Action<string> subjectAction = null;
            string actionExecutionString = null;
            string expectedActionExecutionString = null;
            int retries = default(int);
            Exception thrown = null;

            beforeAll = () =>
            {
                actionExecutionString = null;
                expectedActionExecutionString = "Hello!";
                retries = 3;
                thrown = null;
                subjectAction = (s) =>
                {
                    actionExecutionString = s;
                };

            };

            before = () => subject = TryIt.Try(subjectAction, expectedActionExecutionString, retries);

            describe["TryIt.Try(action, arg, retries).UsingDelay(delay)"] = () =>
            {
                Delay newPause = null;
                ITry result = null;
                before = () =>
                {
                    result = null;
                    newPause = new Mock<Delay>().Object;
                };

                act = () =>
                {
                    try
                    {
                        result = subject.UsingDelay(newPause);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                it["Should set the Delay property"] = () =>
                    subject.Delay.Should().Be(newPause);

                it["should return the subject"] = () =>
                    result.Should().Be(subject);

                it["should not throw any exceptions"] = () =>
                    thrown.Should().BeNull();

                context["when the Delay parameter is null"] = () =>
                {
                    before = () => newPause = null;

                    it["should throw an ArgumentNullException"] = () =>
                        thrown.Should().BeOfType<ArgumentNullException>();

                };
            };

            describe["TryIt.Try(action, arg, retries).Go()"] = () =>
            {

                act = () =>
                {
                    subject.Go();
                };

                it["should call the action"] = () =>
                    actionExecutionString.Should().Be(expectedActionExecutionString);

                it["should set status to RetryStatus.Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

                it["should have an empty ExceptionList"] = () =>
                    subject.ExceptionList.Should().BeEmpty();

                context["when retries is an invalid value"] = () =>
                {
                    Action action = () => TryIt.Try(subjectAction, expectedActionExecutionString, 0);

                    it["should throw an ArgumentOutOfRangeException"] = () =>
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["TryIt.Try(action, arg, retries).ThenTry(arg, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(expectedActionExecutionString, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                describe["TryIt.Try(action, arg, retries).ThenTry(altArg, retries).Go()"] = () =>
                {
                    context["When initial try fails, using alternate arg in ThenTry()"] = () =>
                    {
                        string expectedAltActionExecutionString = null;

                        beforeAll = () =>
                        {
                            expectedAltActionExecutionString = "World!";
                            subjectAction = (s) =>
                            {
                                if (s == expectedActionExecutionString)
                                    throw new Exception("Bad input!");

                                actionExecutionString = s;
                            };
                        };

                        before = () =>
                        {
                            subject = TryIt.Try(subjectAction, expectedActionExecutionString, retries);
                            child = subject.ThenTry(expectedAltActionExecutionString, 3);
                        };

                        act = () => child.Go();

                        it["should call the child with the alternate arg"] = () =>
                            actionExecutionString.Should().Be(expectedAltActionExecutionString);

                        it["should set the Status to SuccessAfterRetries"] = () =>
                            child.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                        it["should not have the exceptions of the parent in it's ExceptionsList"] = () =>
                            child.ExceptionList.Count.Should().Be(0);

                        context["when the initial try succeeds"] = () =>
                        {

                            beforeAll = () =>
                            {
                                subjectAction = (s) =>
                                {
                                    actionExecutionString = s;
                                };
                            };

                            it["should set the status to Success"] = () =>
                                subject.Status.Should().Be(RetryStatus.Success);

                            it["shoud attempt to try only once"] = () =>
                                subject.Attempts.Should().Be(1);

                            it["should never attempt to execute ThenTry()"] = () =>
                                child.Attempts.Should().Be(0);

                        };
                    };
                };
            };

        }

        void Static_Action_T1_T2_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            int retries = 4;

            Action<string, int> subjectAction = (s, i) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1 and arg2 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).Go()"] = () =>
            {

                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2, arg 3 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                };
            };
        }

        void Static_Action_T1_T2_T3_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;

            int retries = 4;

            Action<string, int, double> subjectAction = (s, i, d) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, and arg3 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).Go()"] = () =>
            {

                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, arg3, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2 and arg3 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int, double>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                };
            };
        }

        void Static_Action_T1_T2_T3_T4_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;

            int retries = 4;

            Action<string, int, double, long> subjectAction = (s, i, d, l) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, and arg4 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                };
            };

            describe["TryIt.Try(action, arg, arg2, arg3, arg4, retries).Go()"] = () =>
            {

                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, retries).ThenTry(arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2, arg3, arg4 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int, double, long>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                };
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";

            int retries = 4;

            Action<string, int, double, long, string> subjectAction = (s1, i, d, l, s2) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, and arg5 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, retries).ThenTry(arg1, arg2, arg3, arg4, arg5, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, arg5, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2, arg3, arg4, arg5 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int, double, long, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                };
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;

            int retries = 4;

            Action<string, int, double, long, string, bool> subjectAction = (s1, i, d, l, s2, b) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, and arg6 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_T7_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;
            long arg7 = long.MaxValue;

            int retries = 4;

            Action<string, int, double, long, string, bool, long> subjectAction = (s1, i, d, l1, s2, b, l2) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, arg6, and arg7 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool, long>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                    asTryIt._arg7.Should().Be(arg7);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, arg7, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_T7_T8_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;
            long arg7 = long.MaxValue;
            int arg8 = 33;

            int retries = 4;

            Action<string, int, double, long, string, bool, long, int> subjectAction = (s1, i1, d, l1, s2, b, l2, i2) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, arg6, arg7, and arg8 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool, long, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                    asTryIt._arg7.Should().Be(arg7);
                    asTryIt._arg8.Should().Be(arg8);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, arg7, arg8, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_T7_T8_T9_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;
            long arg7 = long.MaxValue;
            int arg8 = 33;
            string arg9 = "fore score";

            int retries = 4;

            Action<string, int, double, long, string, bool, long, int, string> subjectAction = (s1, i1, d, l1, s2, b, l2, i2, s3) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, and arg9 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool, long, int, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                    asTryIt._arg7.Should().Be(arg7);
                    asTryIt._arg8.Should().Be(arg8);
                    asTryIt._arg9.Should().Be(arg9);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, arg7, arg8, arg9, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }
    }
}