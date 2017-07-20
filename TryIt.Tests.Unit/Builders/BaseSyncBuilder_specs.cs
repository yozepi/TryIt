using NSpec;
using yozepi.Retry.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using yozepi.Retry;
using yozepi.Retry.Runners;
using yozepi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TryIt.Tests.Unit.Builders
{
    [TestClass]
    public class BaseSyncBuilder_specs : nSpecTestHarness
    {
        [TestMethod]
        public void BaseSyncBuilderTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }


        MethodRetryBuilder subject = null;

        void Run_Method()
        {


            context["when Run() is canceled."] = () =>
            {
                Action runnerAction = null;
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;
                Action subjectAction = new Action(() =>
                {
                    subject = yozepi.Retry.TryIt.Try(runnerAction, 1);
                    subject = yozepi.Retry.TryIt.ThenTry(subject, 1);
                    try
                    {
                        subject.Run(token);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                });

                describe["when OperationCanceledException is raised"] = () =>
                {
                    before = () =>
                    {
                        subject = null;
                        thrown = null;
                        runnerAction = () => { };
                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                    };
                    after = () => tokenSource.Dispose();

                    act = subjectAction;

                    context["because Task was already canceled"] = () =>
                    {
                        before = () =>
                        {
                            tokenSource.Cancel();
                        };

                        it["should raise OperationCanceledException"] = () =>
                            thrown.Should().BeAssignableTo<OperationCanceledException>();

                        it["should never run the runners"] = () =>
                            subject.Runners.Count(r => r.Status == RetryStatus.NotStarted).Should().Be(2);

                        it["should set status to Canceled"] = () =>
                            subject.Status.Should().Be(RetryStatus.Canceled);

                    };

                    context["while executing the runner"] = () =>
                    {
                        before = () =>
                        {
                            runnerAction = () => { throw new TaskCanceledException(); };
                        };

                        it["should raise OperationCanceledException"] = () =>
                            thrown.Should().BeAssignableTo<OperationCanceledException>();

                        it["should not run any additional runners"] = () =>
                            subject.LastRunner.Status.Should().Be(RetryStatus.NotStarted);

                        it["should set status to canceled"] = () =>
                            subject.Status.Should().Be(RetryStatus.Canceled);
                    };
                };
            };
        }

        void Runner_Behavior()
        {
            BaseSyncBuilder subject = null;

            describe["when there is only one runner"] = () =>
            {
                Action actor = null;
                ActionRunner runner = null;
                int retryCount = default(int);
                Exception thrown = null;

                act = () =>
                {
                    runner = new ActionRunner()
                    {
                        RetryCount = retryCount,
                        Actor = actor
                    };
                    subject = new MethodRetryBuilder();
                    subject.AddRunner(runner);

                    thrown = null;
                    try
                    {
                        subject.Run(CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                context["when the runner succeeds"] = () =>
                {
                    before = () =>
                    {
                        actor = () => { };
                        retryCount = 3;
                    };

                    it["should only try only once"] = () =>
                        subject.Attempts.Should().Be(1);

                    it["Status should be Success"] = () =>
                        subject.Status.Should().Be(RetryStatus.Success);

                    it["should have no exceptions in ExceptionList"] = () =>
                        subject.ExceptionList.Should().BeEmpty();
                };

                context["when the runner fails at first but finally succeeds"] = () =>
                {
                    before = () =>
                    {
                        actor = () =>
                        {
                            if (subject.LastRunner.Attempts <= 1)
                                throw new Exception();
                        };
                        retryCount = 3;
                    };

                    it["Status should be SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    it["should have exceptions in ExceptionList"] = () =>
                        subject.ExceptionList.Should().NotBeEmpty();

                    it["builder attempts should equal runner attempts"] = () =>
                        subject.Attempts.Should().Be(subject.LastRunner.Attempts);
                };

                context["when the runner fails every attempt"] = () =>
                {
                    before = () =>
                    {
                        actor = () =>
                        {
                            throw new Exception();
                        };
                        retryCount = 3;
                    };

                    it["Status should be Fail"] = () =>
                        subject.Status.Should().Be(RetryStatus.Fail);

                    it["should have exceptions in ExceptionList"] = () =>
                        subject.ExceptionList.Should().NotBeEmpty();

                    it["builder attempts should equal runner attempts"] = () =>
                        subject.Attempts.Should().Be(subject.LastRunner.Attempts);

                    it["should raise RetryFailedException"] = () =>
                        thrown.Should().BeOfType<RetryFailedException>();

                    it["RetryFailedException should contain all the exceptions"] = () =>
                        thrown.As<RetryFailedException>()
                        .ExceptionList.Should().Contain(subject.ExceptionList);
                };
            };

            describe["when there are two runners"] = () =>
            {
                Action actor = null;
                ActionRunner runner1 = null;
                ActionRunner runner2 = null;
                ErrorPolicyDelegate errorPolicy = null;

                int retryCount = default(int);
                Exception thrown = null;

                act = () =>
                {
                    runner1 = new ActionRunner()
                    {
                        RetryCount = retryCount,
                        Actor = actor
                    };
                    runner2 = new ActionRunner();
                    runner1.CopySettings(runner2);
                    subject = new MethodRetryBuilder();
                    subject.AddRunner(runner1);
                    subject.SetErrorPolicy(errorPolicy);
                    subject.AddRunner(runner2);

                    thrown = null;
                    try
                    {
                        subject.Run(CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };
                context["when the first runner succeeds"] = () =>
                {
                    before = () =>
                    {
                        actor = () => { };
                        retryCount = 3;
                    };

                    it["Status should be Success"] = () =>
                        subject.Status.Should().Be(RetryStatus.Success);

                    it["the second runner should never run"] = () =>
                        subject.LastRunner.Status.Should().Be(RetryStatus.NotStarted);
                };

                context["when the first runner fails and the second runner succeeds"] = () =>
                {
                    before = () =>
                    {
                        actor = () =>
                        {
                            if (runner2.Status == RetryStatus.NotStarted)
                                throw new Exception();
                        };
                        retryCount = 3;
                    };

                    it["Status should be SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    it["both runners should run"] = () =>
                    {
                        runner1.Status.Should().NotBe(RetryStatus.NotStarted);
                        runner2.Status.Should().NotBe(RetryStatus.NotStarted);
                    };

                    it["builder attempts should equal both runner attempts"] = () =>
                        subject.Attempts.Should().Be(runner1.Attempts + runner2.Attempts);
                };

                context["when both runners fail"] = () =>
                {
                    before = () =>
                    {
                        actor = () =>
                        {
                            throw new Exception();
                        };
                        retryCount = 3;
                    };
                    it["Status should be Fail"] = () =>
                        subject.Status.Should().Be(RetryStatus.Fail);

                    it["builder attempts should equal both runner attempts"] = () =>
                         subject.Attempts.Should().Be(runner1.Attempts + runner2.Attempts);

                    it["should raise RetryFailedException"] = () =>
                        thrown.Should().BeOfType<RetryFailedException>();

                    it["RetryFailedException should contain the exceptions from both runners"] = () =>
                        thrown.As<RetryFailedException>()
                        .ExceptionList.Should().Contain(runner1.ExceptionList)
                        .And.Contain(runner2.ExceptionList);


                    context["when an error policy throws an exception"] = () =>
                    {
                        Exception expectedException = new Exception();

                        before = () =>
                        {
                            errorPolicy = (ex, tries) => { throw expectedException; };
                        };

                        it["should raise the exception"] = () =>
                            thrown.Should().BeSameAs(expectedException);

                        it["Status should be Fail"] = () =>
                            subject.Status.Should().Be(RetryStatus.Fail);
                    };


                };
            };
        }


    }
}
