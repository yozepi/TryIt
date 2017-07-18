using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using yozepi.Retry.Runners;
using System.Threading;
using yozepi.Retry;
using Moq;
using yozepi.Retry.Delays;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TryIt.Tests.Unit.specs.Runners
{
    [TestClass]
    public class BaseSyncRunner_specs : nSpecTestHarness
    {
        [TestMethod]
        public void BaseSyncRunnerTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        void Run_Method()
        {
            context["when the actor succeeds"] = () =>
            {
                BaseSyncRunner subject = null;
                Action subjectAction = null;
                before = () =>
                {
                    subject = null;
                    subjectAction = null;
                };

                act = () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                    };
                    subject.Run(CancellationToken.None);
                };

                context["after the first try"] = () =>
                {
                    before = () => subjectAction = () => { };

                    it["should set Status to Success"] = () =>
                        subject.Status.Should().Be(RetryStatus.Success);

                    it["should try only once"] = () =>
                        subject.Attempts.Should().Be(1);

                };

                context["after failing at least once."] = () =>
                {
                    var expectedEx = new Exception();

                    before = () => subjectAction = () =>
                    {
                        if (subject.Attempts == 1)
                            throw expectedEx;

                    };

                    it["should set Status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    it["should put the exception in the exception list"] = () =>
                        subject.ExceptionList.Should().Contain(expectedEx);
                };

            };

            context["when there is a success policy"] = () =>
            {
                BaseSyncRunner subject = null;
                Action subjectAction = null;
                SuccessPolicyDelegate successPolicy = null;
                before = () =>
                {
                    subject = null;
                    subjectAction = null;
                    successPolicy = null;
                };

                act = () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                        SuccessPolicy = successPolicy
                    };
                    subject.Run(CancellationToken.None);
                };

                before = () =>
                {
                    subjectAction = () => { };
                };

                context["and the policy passess"] = () =>
                {
                    bool policyExecuted = false;

                    before = () =>
                    {
                        policyExecuted = false;
                        successPolicy = (tries) => { policyExecuted = true; };
                    };

                    it["should execute the policy"] = () =>
                        policyExecuted.Should().BeTrue();

                    it["should set Status to Success"] = () =>
                          subject.Status.Should().Be(RetryStatus.Success);

                    it["should try only once"] = () =>
                        subject.Attempts.Should().Be(1);
                };

                context["when the policy fails every time"] = () =>
                {
                    var expectedEx = new Exception();
                    before = () => successPolicy = (tries) =>
                    {
                        throw expectedEx;
                    };

                    it["should put the exceptions into the ExceptinList"] = () =>
                        subject.ExceptionList.Should().Contain(expectedEx);

                    it["should set the status to Fail"] = () =>
                        subject.Status.Should().Be(RetryStatus.Fail);
                };

                context["when the policy fails first then succeeds"] = () =>
                {
                    var expectedEx = new Exception();
                    before = () => successPolicy = (tries) =>
                    {
                        if (tries == 1)
                            throw expectedEx;
                    };

                    it["should put the exceptions into the ExceptinList"] = () =>
                        subject.ExceptionList.Should().Contain(expectedEx);

                    it["should set the status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);
                };
            };

            context["when the actor never succeeds"] = () =>
            {
                BaseSyncRunner subject = null;
                Action subjectAction = null;
                before = () =>
                {
                    subject = null;
                    subjectAction = () =>
                    {
                        throw new Exception();
                    };
                };

                act = () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                    };

                    subject.Run(CancellationToken.None);
                };

                it["should set the Status to Fail"] = () =>
                   subject.Status.Should().Be(RetryStatus.Fail);

                it["should put the exceptions into the exception list"] = () =>
                    subject.ExceptionList.Count().Should().Be(subject.RetryCount);
            };

            context["when there is a failure policy"] = () =>
            {
                BaseSyncRunner subject = null;
                Action subjectAction = null;
                ErrorPolicyDelegate errorDelegate = null;
                Exception thrown = null;
                before = () =>
                {
                    subject = null;
                    errorDelegate = null;
                    thrown = null;
                    subjectAction = null;
                };

                act = () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                        ErrorPolicy = errorDelegate
                    };

                    try
                    {
                        subject.Run(CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                context["and the error passes the policy criteria (no action to take - policy returns true)"] = () =>
                {
                    bool policyExecuted = false;

                    before = () =>
                    {
                        policyExecuted = false;
                        errorDelegate = (ex, tries) =>
                        {
                            policyExecuted = true;
                            return true;
                        };
                    };

                    it["should execute the policy"] = () =>
                        policyExecuted.Should().BeTrue();

                    it["should retry the actor"] = () =>
                        subject.Attempts.Should().Be(subject.RetryCount);

                    it["should set status to Fail"] = () =>
                        subject.Status.Should().Be(RetryStatus.Fail);

                    it["should not raise any exceptions"] = () =>
                    thrown.Should().BeNull();
                };

                context["when the error fails the policy criteria (cancel retries - policy returns false)"] = () =>
                {
                    var expectedEx = new Exception();

                    before = () =>
                    {
                        errorDelegate = (ex, tries) => false;
                        subjectAction = () => { throw expectedEx; };
                    };

                    it["should set status to Fail"] = () =>
                        subject.Status.Should().Be(RetryStatus.Fail);

                    it["should stop further attempts on the actor"] = () =>
                        subject.Attempts.Should().BeLessThan(subject.RetryCount);

                    it["should put a ErrorPolicyException containing the failing error into the exception list"] = () =>
                        subject.ExceptionList.First(e => e.GetType() == typeof(ErrorPolicyException))
                        .As<ErrorPolicyException>().InnerException.Should().BeSameAs(expectedEx);

                    it["should not raise any exceptions"] = () =>
                      thrown.Should().BeNull();
                };

                context["when the policy throws an exception"] = () =>
                {
                    var expectedEx = new InvalidOperationException();

                    before = () =>
                    {
                        errorDelegate = (ex, tries) => { throw expectedEx; };
                        subjectAction = () => { throw new Exception(); };
                    };

                    it["should set status to Fail"] = () =>
                        subject.Status.Should().Be(RetryStatus.Fail);

                    it["should stop further attempts on the actor"] = () =>
                        subject.Attempts.Should().BeLessThan(subject.RetryCount);

                    it["should put the exception into the exception list"] = () =>
                        subject.ExceptionList.Should().Contain(expectedEx);

                    it["should raise the exception"] = () =>
                        thrown.Should().BeSameAs(expectedEx);

                };
            };

            context["when RunAsync() is canceled."] = () =>
            {
                BaseSyncRunner subject = null;
                Action subjectAction = null;
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;
                Mock<IDelay> delayMock = null;
                before = () =>
                {
                    subject = null;
                    subjectAction = () => { };
                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    delayMock = new Mock<IDelay>();
                    thrown = null;
                };

                act = () =>
                {
                    subject = new ActionRunner
                    {
                        RetryCount = 2,
                        Actor = subjectAction,
                        Delay = delayMock.Object
                    };
                    try
                    {
                        subject.Run(token);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                afterAll = () =>
                {
                    tokenSource.Dispose();
                };


                context["before it even starts"] = () =>
                {
                    bool actionRan = false;
                    before = () =>
                    {
                        tokenSource.Cancel();
                        actionRan = false;
                        subjectAction = () => { actionRan = true; };
                    };

                    it["should never run the action"] = () =>
                        actionRan.Should().BeFalse();

                    it["should never try to delay"] = () =>
                        delayMock.Verify(m => m.WaitAsync(It.IsAny<int>(), token), Times.Never);

                    it["should raise OperationCanceledException"] = () =>
                        thrown.Should().BeAssignableTo<OperationCanceledException>();

                    it["should set status to Canceled"] = () =>
                        subject.Status.Should().Be(RetryStatus.Canceled);
                };
                context["while executing the action"] = () =>
                {
                    before = () =>
                    {
                        subjectAction = () => { throw new TaskCanceledException(); };
                    };

                    it["should never try to delay"] = () =>
                        delayMock.Verify(m => m.WaitAsync(It.IsAny<int>(), token), Times.Never);

                    it["should raise OperationCanceledException"] = () =>
                        thrown.Should().BeAssignableTo<OperationCanceledException>();

                    it["should set status to canceled"] = () =>
                           subject.Status.Should().Be(RetryStatus.Canceled);
                };

                context["during the delay"] = () =>
                {
                    before = () =>
                    {
                        subjectAction = () => { throw new Exception(); };
                        delayMock.Setup(m => m.WaitAsync(It.IsAny<int>(), token)).Throws<TaskCanceledException>();
                    };
                    it["should start the delay"] = () =>
                        delayMock.Verify(m => m.WaitAsync(It.IsAny<int>(), token), Times.Once);

                    it["should raise OperationCanceledException"] = () =>
                        thrown.Should().BeAssignableTo<OperationCanceledException>();

                    it["should set status to canceled"] = () =>
                           subject.Status.Should().Be(RetryStatus.Canceled);
                };
            };
        }
    }
}
