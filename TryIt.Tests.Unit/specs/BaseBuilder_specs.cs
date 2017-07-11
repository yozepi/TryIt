using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Builders;
using System.Threading;
using Retry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retry.Runners;
using Retry.Delays;

namespace TryIt.Tests.Unit.specs
{
    class BaseBuilder_specs : nspec
    {
        void Setter_Methods()
        {
            BaseBuilder subject = new ActionRetryBuilder();
            subject.AddRunner(new ActionRunner { RetryCount = 1 });

            describe["SetRetryCount()"] = () =>
            {
                BaseBuilder returned = null;
                before = () => returned = null;
                act = () => returned = subject.SetRetryCount(3);

                it["should set the RetryCount of the underlying runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(3);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);

                context["when RetryCount is invalid"] = () =>
                {
                    it["should throw InvalidArgumentException"] = () =>
                        subject.Invoking(s => s.SetRetryCount(0))
                        .ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["SetSuccessPolicy()"] = () =>
            {
                BaseBuilder returned = null;
                SuccessPolicyDelegate expectedPolicy = (tries) => { };
                before = () => returned = null;
                act = () => returned = subject.SetSuccessPolicy(expectedPolicy);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };

            describe["SetErrorPolicy()"] = () =>
            {
                BaseBuilder returned = null;
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };
                before = () => returned = null;
                act = () => returned = subject.SetErrorPolicy(expectedPolicy);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };

            describe["SetDelay()"] = () =>
            {
                BaseBuilder returned = null;
                Delay expectedDelay = Delay.NoDelay();
                before = () => returned = null;
                act = () => returned = subject.SetDelay(expectedDelay);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };

            describe["SetActor()"] = () =>
            {
                BaseBuilder returned = null;
                Action expectedActor = () => { };
                before = () => returned = null;
                act = () => returned = subject.SetActor(expectedActor);

                it["set the SuccessPolicy of the underlying runner"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(expectedActor);

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);

                context["when actor is null"] = () =>
                {
                    it["should throw InvalidArgumentException"] = () =>
                        subject.Invoking(s => s.SetActor(null))
                        .ShouldThrow<ArgumentNullException>();
                };
            };

            describe["adding a second runner"] = () =>
            {
                BaseBuilder returned = null;
                BaseRunner firstRunner = null;
                BaseRunner newRunner = null;
                SuccessPolicyDelegate successPolicy = (tries) => { };
                ErrorPolicyDelegate errorPolicy = (ex, tries) => { return true; };
                Action actor = () => { };

                before = () =>
                {
                    returned = null;
                    newRunner = null;
                    firstRunner = subject.LastRunner;
                    firstRunner.RetryCount = 3;
                    firstRunner.SuccessPolicy = successPolicy;
                    firstRunner.ErrorPolicy = errorPolicy;
                    firstRunner.Delay = Delay.Basic(TimeSpan.FromMilliseconds(100));
                    firstRunner.Actor = actor;

                    newRunner = new ActionRunner();
                };

                act = () => returned = subject.AddRunner(newRunner);

                it["should append the second runner"] = () =>
                    subject.LastRunner.Should().BeSameAs(newRunner);

                it["should copy the values of the first runner to the second runner"] = () =>
                {
                    firstRunner.RetryCount.Should().Be(newRunner.RetryCount);
                    firstRunner.SuccessPolicy.Should().BeSameAs(newRunner.SuccessPolicy);
                    firstRunner.ErrorPolicy.Should().BeSameAs(newRunner.ErrorPolicy);
                    firstRunner.Delay.Should().BeSameAs(newRunner.Delay);
                    firstRunner.Actor.Should().BeSameAs(newRunner.Actor);
                };

                it["should return the subject"] = () =>
                    returned.Should().BeSameAs(subject);
            };
        }

        void Run_Method()
        {

            context["when Run() is canceled."] = () =>
            {
                BaseBuilder subject = null;
                Action subjectAction = null;
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;

                describe["when OperationCanceledException is raised"] = () =>
                {
                    before = () =>
                    {
                        subject = null;
                        subjectAction = () => { };
                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                        thrown = null;
                    };
                    after = () => tokenSource.Dispose();

                    act = () =>
                    {
                        subject = Retry.TryIt.Try(subjectAction, 1).ThenTry(1);

                        try
                        {
                            subject.Run(token);
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };

                    context["because Task was already canceled"] = () =>
                    {
                        before = () =>
                        {
                            tokenSource.Cancel();
                        };

                        it["should raise OperationCanceledException"] = () =>
                            Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                        it["should never run the runners"] = () =>
                            subject.Runners.Count(r => r.Status == RetryStatus.NotStarted).Should().Be(2);

                        it["should set status to Canceled"] = () =>
                            subject.Status.Should().Be(RetryStatus.Canceled);

                    };

                    context["while executing the runner"] = () =>
                    {
                        before = () =>
                        {
                            subjectAction = () => { throw new TaskCanceledException(); };
                        };

                        it["should raise OperationCanceledException"] = () =>
                            Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                        it["should run any additional runners"] = () =>
                            subject.LastRunner.Status.Should().Be(RetryStatus.NotStarted);

                        it["should set status to canceled"] = () =>
                            subject.Status.Should().Be(RetryStatus.Canceled);
                    };
                };
            };
        }

        void RunAsync_Method()
        {

            context["when RunAsync() is canceled."] = () =>
            {
                BaseBuilder subject = null;
                Action subjectAction = null;
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;

                describe["when OperationCanceledException is raised"] = () =>
                {
                    before = () =>
                    {
                        subject = null;
                        subjectAction = () => { };
                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                        thrown = null;
                    };

                    actAsync = async () =>
                    {
                        subject = Retry.TryIt.Try(subjectAction, 1).ThenTry(1);

                        try
                        {
                            await subject.RunAsync(token);
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };

                    context["because Task was already canceled"] = () =>
                    {
                        before = () =>
                        {
                            tokenSource.Cancel();
                        };

                        it["should raise OperationCanceledException"] = () =>
                            Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                        it["should never run the runners"] = () =>
                            subject.Runners.Count(r => r.Status == RetryStatus.NotStarted).Should().Be(2);

                        it["should set status to Canceled"] = () =>
                            subject.Status.Should().Be(RetryStatus.Canceled);

                    };

                    context["while executing the runner"] = () =>
                    {
                        before = () =>
                        {
                            subjectAction = () => { throw new TaskCanceledException(); };
                        };

                        it["should raise OperationCanceledException"] = () =>
                            Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));

                        it["should run any additional runners"] = () =>
                            subject.LastRunner.Status.Should().Be(RetryStatus.NotStarted);

                        it["should set status to canceled"] = () =>
                            subject.Status.Should().Be(RetryStatus.Canceled);
                    };
                };
            };
        }

        void Runner_Behavior()
        {
            BaseBuilder subject = null;

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
                    subject = new ActionRetryBuilder();
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
                    subject = new ActionRetryBuilder();
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
                        } ;
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