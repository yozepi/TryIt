using NSpec;
using Retry.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Retry;
using Moq;
using Retry.Delays;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TryIt.Tests.Unit.specs
{
    class BaseRunner_Methods : nspec
    {
        void CopySettings_Method()
        {
            BaseRunner source = null;
            BaseRunner target = null;

            act = () =>
            {
                source = new ActionRunner
                {
                    Delay = Delay.DefaultDelay,
                    ErrorPolicy = (ex, retries) => { return true; },
                    SuccessPolicy = new Action(() => { }),
                    RetryCount = 4,
                    Actor = new Action(() => { })
                };

                target = new ActionRunner();
                source.CopySettings(target);
            };
            it["should copy Delay to the target"] = () =>
                target.Delay.Should().Be(source.Delay);

            it["should copy ErrorPolicy to the target"] = () =>
                target.ErrorPolicy.Should().Be(source.ErrorPolicy);

            it["should copy SuccessPolicy to the target"] = () =>
                target.SuccessPolicy.Should().Be(source.SuccessPolicy);

            it["should copy RetryCount to the target"] = () =>
                target.RetryCount.Should().Be(source.RetryCount);

            it["should copy Actor to the target"] = () =>
                target.Actor.Should().Be(source.Actor);


        }

        void RunAsync_Method()
        {
            context["when the actor succeeds"] = () =>
            {
                BaseRunner subject = null;
                Action subjectAction = null;
                before = () =>
                {
                    subject = null;
                    subjectAction = null;
                };

                actAsync = async () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                    };
                    await subject.RunAsync(CancellationToken.None);
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
                BaseRunner subject = null;
                Action subjectAction = null;
                SuccessPolicyDelegate successPolicy = null;
                before = () =>
                {
                    subject = null;
                    subjectAction = null;
                    successPolicy = null;
                };

                actAsync = async () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                        SuccessPolicy = successPolicy
                    };
                    await subject.RunAsync(CancellationToken.None);
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

                context["when the policy fails before succeeding"] = () =>
                {
                    var expectedEx = new Exception();
                    before = () => successPolicy = (tries) =>
                    {
                        if(tries == 1)
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
                BaseRunner subject = null;
                Action subjectAction = null;
                before = () =>
                {
                    subject = null;
                    subjectAction = () =>
                    {
                        throw new Exception();
                    };
                };

                actAsync = async () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                    };

                    await subject.RunAsync(CancellationToken.None);
                };

                it["should set the Status to Fail"] = () =>
                   subject.Status.Should().Be(RetryStatus.Fail);

                it["should put the exceptions into the exception list"] = () =>
                    subject.ExceptionList.Count().Should().Be(subject.RetryCount);
            };

            context["when there is a failure policy"] = () =>
            {
                BaseRunner subject = null;
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

                actAsync = async () =>
                {
                    subject = new ActionRunner
                    {
                        Actor = subjectAction,
                        RetryCount = 3,
                        ErrorPolicy = errorDelegate
                    };

                    try
                    {
                        await subject.RunAsync(CancellationToken.None);
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
                BaseRunner subject = null;
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

                actAsync = async () =>
                {
                    subject = new ActionRunner
                    {
                        RetryCount = 2,
                        Actor = subjectAction,
                        Delay = delayMock.Object
                    };
                    try
                    {
                        await subject.RunAsync(token);
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
                        Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

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
                        Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

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
                       Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                    it["should set status to canceled"] = () =>
                           subject.Status.Should().Be(RetryStatus.Canceled);
                };
            };
        }
    }
}
