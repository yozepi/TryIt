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

namespace TryIt.Tests.Unit.specs
{
    class BaseBuilder_Methods : nspec
    {
        void Run_Method()
        {

            context["when Run() is canceled."] = () =>
            {
                BaseBuilder subject = null;
                Action<bool> subjectAction = null;
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;

                describe["when OperationCanceledException is raised"] = () =>
                {
                    before = () =>
                    {
                        subject = null;
                        subjectAction = (prop) => { };
                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                        thrown = null;
                    };

                    act = () =>
                    {
                        subject = Retry.TryIt.Try(subjectAction, false, 1).ThenTry(true, 1);

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
                            subjectAction = (param) => { throw new TaskCanceledException(); };
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
                Action<bool> subjectAction = null;
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;

                describe["when OperationCanceledException is raised"] = () =>
                {
                    before = () =>
                    {
                        subject = null;
                        subjectAction = (prop) => { };
                        tokenSource = new CancellationTokenSource();
                        token = tokenSource.Token;
                        thrown = null;
                    };

                    actAsync = async () =>
                    {
                        subject = Retry.TryIt.Try(subjectAction, false, 1).ThenTry(true, 1);

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
                            subjectAction = (param) => { throw new TaskCanceledException(); };
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
    }
}