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

namespace TryIt.Tests.Unit.specs
{
    class ActionRunners : nspec
    {
        void runner_common_specs()
        {
            context["task cancellation"] = () =>
            {
                BaseRunner subject = null;
                Action subjectAction = null;
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;
                Mock < IDelay > delayMock = null;
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
                    subject = new ActionRunner();
                    subject.RetryCount = 2;
                    subject.Actor = subjectAction;
                    subject.Delay = delayMock.Object;
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

                describe["when the task is canceled before start up"] = () =>
                {
                    before = () =>
                    {
                        tokenSource.Cancel();
                    };

                    it["should raise TaskCanceledException"] = () =>
                        thrown.Should().BeOfType<TaskCanceledException>();

                    it["should set status to canceled"] = () =>
                           subject.Status.Should().Be(RetryStatus.Canceled);


                };

                describe["when TaskCanceledException is raised"] = () =>
                {
   
                    context["because Task was already canceled"] = () =>
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

                        it["should raise TaskCanceledException"] = () =>
                            thrown.Should().BeOfType<TaskCanceledException>();

                        it["should set status to canceled"] = () =>
                               subject.Status.Should().Be(RetryStatus.Canceled);
                    };

                    context["when raised while executing the action"] = () =>
                    {
                        before = () =>
                        {
                            subjectAction = () => { throw new TaskCanceledException(); };
                        };

                        it["should never try to delay"] = () =>
                            delayMock.Verify(m => m.WaitAsync(It.IsAny<int>(), token), Times.Never);

                        it["should raise TaskCanceledException"] = () =>
                            thrown.Should().BeOfType<TaskCanceledException>();

                        it["should set status to canceled"] = () =>
                               subject.Status.Should().Be(RetryStatus.Canceled);
                    };

                    context["when raised during the delay"] = () =>
                    {
                        before = () =>
                        {
                            subjectAction = () => { throw new Exception(); };
                            delayMock.Setup(m => m.WaitAsync(It.IsAny<int>(), token)).Throws<TaskCanceledException>();
                        };
                        it["should try to delay"] = () =>
                            delayMock.Verify(m => m.WaitAsync(It.IsAny<int>(), token), Times.Once);

                        it["should raise TaskCanceledException"] = () =>
                            thrown.Should().BeOfType<TaskCanceledException>();

                        it["should set status to canceled"] = () =>
                               subject.Status.Should().Be(RetryStatus.Canceled);
                    };
                };

            };
        }
    }
}
