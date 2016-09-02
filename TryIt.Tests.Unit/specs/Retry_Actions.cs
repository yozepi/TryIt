using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Retry.Builders;
using Retry.Runners;
using Retry;
using Retry.Delays;

namespace TryIt.Tests.Unit.specs
{

    class Retry_Actions : nspec
    {

        void with_no_arguments()
        {
            ActionRetryBuilder subject = null;
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
                subject = Retry.TryIt.Try(subjectAction, retries);
            };


            describe["TryIt.Try(Action action, int retries)"] = () =>
            {

                it["should return an Builder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the retry count"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                it["should set the actor"] = () =>
                    subject.LastRunner.Actor.Should().Be(subjectAction);

                context["when the action is null"] = () =>
                {
                    act = () =>
                    {
                        try
                        {
                            Retry.TryIt.Try((Action)null, retries);
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

            describe["TryIt.Try(Action action, int retries).WithErrorPolicy(ErrorPolicyDelegate)"] = () =>
            {
                ErrorPolicyDelegate errorDelegate = (ex, i) => { return true; };

                act = () => subject = subject.WithErrorPolicy(errorDelegate);

                it["should set the internal ErrorPolicy property"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(errorDelegate);

                it["should return a Builder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

            };

            describe["TryIt.Try(Action action, int retries).WithSuccessPolicy(SuccessPolicyDelegate)"] = () =>
            {
                SuccessPolicyDelegate successDelegate = (i) => { };

                act = () => subject = subject.WithSuccessPolicy(successDelegate);

                it["should set the internal SuccessPolicy property"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(successDelegate);

            };

            describe["TryIt.Try(Action action, int retries).UsingDelay(Delay delay)"] = () =>
            {
                Delay newPause = null;
                ActionRetryBuilder result = null;
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
                    subject.LastRunner.Delay.Should().Be(newPause);

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


                    describe["Try().WithErrorPolicy().Go"] = () =>
                    {
                        Exception delegateError = null;
                        int errorTryCount = default(int);
                        bool errorPolicyHasBeenCalled = false;

                        ErrorPolicyDelegate errorDelegate = null;
                        before = () =>
                        {
                            errorPolicyHasBeenCalled = false;
                            delegateError = null;
                            errorTryCount = 0;

                            errorDelegate = (e, i) =>
                            {
                                errorPolicyHasBeenCalled = true;
                                errorTryCount = i;
                                delegateError = e;
                                return true;
                            };

                            subject.WithErrorPolicy(errorDelegate);
                        };

                        it["should call the ErrorPolicy delegate"] = () =>
                            errorPolicyHasBeenCalled.Should().BeTrue();

                        context["when ErrorPolicyDelegate returns false"] = () =>
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

                                subject.WithErrorPolicy(errorDelegate);
                            };

                            it["should not throw an exception"] = () =>
                                thrown.Should().BeOfType<RetryFailedException>();

                            it["should have ErrorPolicyException in the exceptionlist"] = () =>
                                thrown.As<RetryFailedException>()
                                .ExceptionList.FirstOrDefault(x => x.GetType() == typeof(ErrorPolicyException))
                                .Should().NotBeNull();

                            it["ErrorPolicyException's inner exception should be the expected exception"] = () =>
                                 thrown.As<RetryFailedException>()
                                .ExceptionList.First(x => x.GetType() == typeof(ErrorPolicyException))
                                .As<ErrorPolicyException>()
                                .InnerException.Should().BeSameAs(expectedException);

                        };
                    };
                };

                context["when Try().Go() fails on every attempt"] = () =>
                {
                    beforeAll = () =>
                    {
                        subjectAction = () => { throw new Exception("Say WHAT!?!"); };
                    };

                    it["should have attempted the action as many times as RetryCount"] = () =>
                        subject.Attempts.Should().Be(retries);

                    it["should contain an exception for evry attempt"] = () =>
                        subject.ExceptionList.Count.Should().Be(subject.Attempts);

                    it["should throw RetryFailedException with all the subject exceptions"] = () =>
                        {
                            thrown.Should().BeOfType<RetryFailedException>();
                            thrown.As<RetryFailedException>().ExceptionList.ShouldBeEquivalentTo(subject.ExceptionList);
                        };

                };

                context["when retries is an invalid value"] = () =>
                {
                    Action action = () => Retry.TryIt.Try(subjectAction, 0);
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
                it["should call the provided delay every time Try() fails (less 1)"] = () =>
                    newPause.Verify(m => m.WaitAsync(It.IsAny<int>()), Times.Exactly(retries - 1));

            };

            describe["TryIt.Try(action, retries).ThenTry(retries)"] = () =>
            {
                ActionRetryBuilder child = null;
                before = () => child = subject.ThenTry(3);
                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should have 2 runners"] = () =>
                    subject.Runners.Count.Should().Be(2);


                context["when the parent has ErrorPolicy set"] = () =>
                {
                    ErrorPolicyDelegate errorPolicy = (i, e) => { return true; };
                    before = () =>
                    {
                        child = subject.WithErrorPolicy(errorPolicy).ThenTry(3);
                    };

                    it["should use the ErrorPolicy delegate of the parent"] = () =>
                        child.LastRunner.ErrorPolicy.Should().Be(errorPolicy);
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

                        it["Try should have executed once"] = () =>
                            subject.Attempts.Should().Be(1);

                        it["should have no exceptions"] = () =>
                            subject.ExceptionList.Count.Should().Be(0);

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

                        it["should set attempts to retries + 1"] = () =>
                            subject.Attempts.Should().Be(retries + 1);

                        it["status should be SuccessAfterRetries"] = () =>
                            subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                        it["Try() should contain an exception for every failure"] = () =>
                            subject.ExceptionList.Count.Should().Be(retries);

                    };

                    context["when both Try() and ThenTry() fail after every attempt"] = () =>
                    {
                        beforeAll = () =>
                        {
                            subjectAction = () => { throw new Exception("Goodbye cruel world!"); };
                            subject = Retry.TryIt.Try(subjectAction, retries);
                        };

                        it["Try() should have an exception for every attempt"] = () =>
                            subject.ExceptionList.Count.Should().Be(retries * 2);

                        it["status should be Fail"] = () =>
                            subject.Status.Should().Be(RetryStatus.Fail);
                    };
                };

                describe["TryIt.Try(action, retries).WithErrorPolicy(delegate).ThenTry(retries).Go()"] = () =>
                {
                    ErrorPolicyDelegate errorPolicy = null;
                    before = () => subjectAction = () => { };
                    act = () =>
                    {
                        subject = Retry.TryIt.Try(subjectAction, retries).WithErrorPolicy(errorPolicy);
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
                        int errorPolicyCalled = default(int);
                        before = () =>
                        {
                            thrown = null;
                            errorPolicyCalled = default(int);
                            errorPolicy = (e, i) => { errorPolicyCalled++; return true; };
                        };

                        it["should succeed without any failures and a status of Success"] = () =>
                        {
                            errorPolicyCalled.Should().Be(0);
                            subject.ExceptionList.Count.Should().Be(0);
                            subject.Status.Should().Be(RetryStatus.Success);
                        };
                    };

                    context["when Try().ThenTry() both fail"] = () =>
                    {
                        int errorPolicyCalled = default(int);

                        before = () =>
                        {
                            thrown = null;
                            errorPolicyCalled = default(int);
                            subjectAction = () => { throw new Exception("Welcome to the Machine!"); };
                            errorPolicy = (e, i) => { errorPolicyCalled++; return true; };
                        };

                        it["should call ErrorPolicy for both Try() and ThenTry()"] = () =>
                            errorPolicyCalled.Should().Be(retries * 2);

                        it["should throw RetryFailedException"] = () =>
                            thrown.Should().BeOfType<RetryFailedException>();
                    };

                    context["When ErrorPolicy decides Try() should not continue"] = () =>
                    {
                        var expectedException = new Exception("Woah! What happened?");
                        Exception capturedEx = null;
                        before = () =>
                        {
                            capturedEx = null;
                            subjectAction = () => {
                                if (subject.ExceptionList.Count == 0)
                                    throw expectedException;
                            };
                            errorPolicy = (e, i) =>
                            {
                                if (subject.ExceptionList.Count == 0)
                                    return false;
                                return true;
                            };
                        };

                        it["should continue to ThenTry"] = () =>
                            subject.Runners.Last.Value.Attempts.Should().Be(1);

                        it["No exception should be thrown"] = () =>
                            thrown.Should().BeNull();

                        it["status should be SuccessAfterRetries"] = () =>
                            subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);
                    };

                    context["when oError throws an exception"] = () =>
                    {
                        var expectedException = new Exception("I'm a new exception.");
                        before = () =>
                        {
                            subjectAction = () => { throw new Exception("Woah! What happened?"); };
                            errorPolicy = (e, i) => { throw expectedException; };
                        };

                        it["should throw the exception thrown by ErrorPolicy"] = () =>
                        thrown.Should().Be(expectedException);

                        it["status should be Fail"] = () =>
                        {
                            child.Status.Should().Be(RetryStatus.Fail);
                            subject.Status.Should().Be(RetryStatus.Fail);
                        };
                    };

                };

                describe["TryIt.Try(action, retries).WithSuccessPolicy(delegate).ThenTry(retries).Go()"] = () =>
                {
                    SuccessPolicyDelegate successDelegate = null;
                    act = () =>
                    {
                        subjectAction = () => { };
                        subject = Retry.TryIt.Try(subjectAction, retries).WithSuccessPolicy(successDelegate);
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
                    context["when SuccessPolicy does not throw"] = () =>
                    {
                        int successPolicyCalled = default(int);
                        before = () =>
                        {
                            thrown = null;
                            successPolicyCalled = default(int);
                            successDelegate = (i) => { successPolicyCalled = i; };
                        };

                        it["should succeed without any failures and a status of Success"] = () =>
                        {
                            successPolicyCalled.Should().Be(1);
                            child.ExceptionList.Count.Should().Be(0);
                            subject.Status.Should().Be(RetryStatus.Success);
                            child.Status.Should().Be(RetryStatus.Success);
                        };
                    };

                    context["when SuccessPolicy throws an exception"] = () =>
                    {
                        int successPolicyCalled = default(int);
                        before = () =>
                        {
                            thrown = null;
                            successPolicyCalled = default(int);
                            successDelegate = (i) => { successPolicyCalled++; throw new Exception("BARF!!"); };
                        };
                        it["should set a status of Fail"] = () =>
                        {
                            subject.Status.Should().Be(RetryStatus.Fail);
                            child.Status.Should().Be(RetryStatus.Fail);
                        };

                        it["should call SuccessPolicy both in Try() and ThenTry()"] = () =>
                            successPolicyCalled.Should().Be(retries * 2);

                        it["should raise RetryFailedException"] = () =>
                            thrown.Should().BeOfType<RetryFailedException>();
                    };
                };

            };

            describe["TryIt.Try(action, retries).ThenTry(altAction, retries)"] = () =>
            {
                ActionRetryBuilder child = null;
                Action altAction = () => { };
                act = () =>
                {
                    subject = Retry.TryIt.Try(subjectAction, retries);
                    child = subject.ThenTry(altAction, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                context["TryIt.Try(action, retries).ThenTry(altAction, retries).Go()"] = () =>
                {
                    int altCalled = default(int);
                    before = () =>
                    {
                        altAction = () => { altCalled++; };
                        subjectAction = () => { throw new Exception("You killed my father. Prepare to die!"); };
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

        void with_1_argument()
        {
            ActionRetryBuilder subject = null;
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

            before = () => subject = Retry.TryIt.Try(subjectAction, expectedActionExecutionString, retries);

            describe["TryIt.Try(action, arg, retries).UsingDelay(delay)"] = () =>
            {
                Delay newPause = null;
                ActionRetryBuilder result = null;
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
                    subject.LastRunner.Delay.Should().Be(newPause);

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
                    Action action = () => Retry.TryIt.Try(subjectAction, expectedActionExecutionString, 0);

                    it["should throw an ArgumentOutOfRangeException"] = () =>
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["TryIt.Try(action, arg, retries).ThenTry(arg, retries)"] = () =>
            {
                ActionRetryBuilder child = null;
                before = () => child = subject.ThenTry(expectedActionExecutionString, 3);
                it["should return the subject"] = () =>
                    child.Should().Be(subject);

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
                            subject = Retry.TryIt.Try(subjectAction, expectedActionExecutionString, retries);
                            child = subject.ThenTry(expectedAltActionExecutionString, 3);
                        };

                        act = () => child.Go();

                        it["should call the child with the alternate arg"] = () =>
                            actionExecutionString.Should().Be(expectedAltActionExecutionString);

                        it["should set the Status to SuccessAfterRetries"] = () =>
                            child.Status.Should().Be(RetryStatus.SuccessAfterRetries);


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

                        };
                    };
                };
            };

            describe["TryIt.Try(action, retries).ThenTry(altAction<T>, retries)"] = () =>
            {
                ActionRetryBuilder child = null;
                Action<string> altAction = (a) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, expectedActionExecutionString, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept the alternate argument"] = () =>
                    child.LastRunner.As<ActionRunner<string>>()._arg.Should().Be(expectedActionExecutionString);
            };

        }

        void with_2_arguments()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            int retries = 4;

            Action<string, int> subjectAction = (s, i) => { };
            ActionRetryBuilder subject = null;

            act = () => { subject = Retry.TryIt.Try(subjectAction, arg1, arg2, retries); };

            describe["TryIt.Try(action, arg1, arg2, retries)"] = () =>
            {

                it["should return a Builder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the arg1 and arg2 internal properties"] = () =>
                {
                    var asRunner = subject.LastRunner.As<ActionRunner<string, int>>();
                    asRunner._arg1.Should().Be(arg1);
                    asRunner._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, retries).Go()"] = () =>
            {

                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, retries)"] = () =>
            {
                ActionRetryBuilder child = null;
                act = () => child = subject.ThenTry(arg1, arg2, retries);

                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should set the arg1, arg2, arg 3 internal properties on the child"] = () =>
              {
                  var asRunner = child.LastRunner.As<ActionRunner<string, int>>();
                  asRunner._arg1.Should().Be(arg1);
                  asRunner._arg2.Should().Be(arg2);
              };
            };

            describe["TryIt.Try(action, retries).ThenTry(altAction<T1, T2>, retries)"] = () =>
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
                    var asRunner = child.LastRunner.As<ActionRunner<string, int>>();
                    asRunner._arg1.Should().Be(arg1);
                    asRunner._arg2.Should().Be(arg2);
                };
            };
        }

        void with_3_arguments()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;

            int retries = 4;

            Action<string, int, double> subjectAction = (s, i, d) => { };
            ActionRetryBuilder subject = null;

            act = () => { subject = Retry.TryIt.Try(subjectAction, arg1, arg2, arg3, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries)"] = () =>
            {

                it["should create an ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the arg1, arg2, and arg3 internal properties"] = () =>
                {
                    var builder = subject.LastRunner.As<ActionRunner<string, int, double>>();
                    builder._arg1.Should().Be(arg1);
                    builder._arg2.Should().Be(arg2);
                    builder._arg3.Should().Be(arg3);
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
                ActionRetryBuilder child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, 3);
                it["should return the subject"] = () =>
                    child.Should().BeSameAs(subject);

                it["should set the arg1, arg2 and arg3 internal properties on the child"] = () =>
                {
                    var runner = child.LastRunner.As<ActionRunner<string, int, double>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                };
            };

            describe["TryIt.Try(action, retries).ThenTry(altAction<T1, T2, T3>, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Action<string, int, double> altAction = (a1, a2, a3) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg1, arg2, arg3, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var runner = child.LastRunner.As<ActionRunner<string, int, double>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                };
            };
        }

        void with_4_arguments()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;

            int retries = 4;

            Action<string, int, double, long> subjectAction = (s, i, d, l) => { };
            ActionRetryBuilder subject = null;

            act = () => { subject = Retry.TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, retries)"] = () =>
            {

                it["should create an ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();

                it["should set the arg1, arg2, arg3, and arg4 internal properties"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<string, int, double, long>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                    runner._arg4.Should().Be(arg4);
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
                ActionRetryBuilder child = null;
                act = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, 3);
                it["should return the subject"] = () =>
                    child.Should().Be(subject);

                it["should set the arg1, arg2, arg3, arg4 internal properties on the child"] = () =>
                {
                    var runner = subject.LastRunner.As<ActionRunner<string, int, double, long>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                    runner._arg4.Should().Be(arg4);
                };
            };

            describe["TryIt.Try(action, retries).ThenTry(altAction<T1, T2, T3, T4>, retries)"] = () =>
            {
                ActionRetryBuilder child = null;

                Action<string, int, double, long> altAction = (a1, a2, a3, a4) => { };
                act = () =>
                {
                    child = subject.ThenTry(altAction, arg1, arg2, arg3, arg4, retries);
                };

                it["ThenTry() should use the alternate action"] = () =>
                    child.LastRunner.Actor.Should().BeSameAs(altAction);

                it["ThenTry() should accept all the alternate arguments"] = () =>
                {
                    var runner = child.LastRunner.As<ActionRunner<string, int, double, long>>();
                    runner._arg1.Should().Be(arg1);
                    runner._arg2.Should().Be(arg2);
                    runner._arg3.Should().Be(arg3);
                    runner._arg4.Should().Be(arg4);
                };
            };
        }

    }
}