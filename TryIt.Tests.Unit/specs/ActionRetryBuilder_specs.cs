using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using Retry.Builders;
using Retry.Runners;
using Retry;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TryIt.Tests.Unit.specs
{
    class ActionRetryBuilder_specs : nspec
    {
        ActionRetryBuilder subject = null;

        void Go_Method()
        {
            before = () =>
            {
                subject = new ActionRetryBuilder();
                subject.AddRunner(GetRunner());
            };

            act = () => subject.Go();
            it["should execute the runner and set Status to Success"] = () =>
                subject.Status.Should().Be(RetryStatus.Success);

            context["when CancellationToken has been canceled"] = () =>
            {
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;

                before = () =>
                {
                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    tokenSource.Cancel();
                    thrown = null;
                };
                after = () => tokenSource.Dispose();

                act = () =>
                {
                    try
                    {
                        subject.Go(token);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };


                it["should set status to Canceled"] = () =>
                    subject.Status.Should().Be(RetryStatus.Canceled);

                it["should raise OperationCanceledException"] = () =>
                    Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));
            };
        }

        void GoAsync_Method()
        {
            before = () =>
            {
                subject = new ActionRetryBuilder();
                subject.AddRunner(GetRunner());
            };

            actAsync = async () =>  await subject.GoAsync();

            it["should execute the runner and set Status to Success"] = () =>
                subject.Status.Should().Be(RetryStatus.Success);

            context["when CancellationToken has been canceled"] = () =>
            {
                CancellationTokenSource tokenSource = null;
                CancellationToken token = CancellationToken.None;
                Exception thrown = null;

                before = () =>
                {
                    tokenSource = new CancellationTokenSource();
                    token = tokenSource.Token;
                    tokenSource.Cancel();
                    thrown = null;
                };
                after = () => tokenSource.Dispose();

                actAsync = async () =>
                {
                    try
                    {
                        await subject.GoAsync(token);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };


                it["should set status to Canceled"] = () =>
                    subject.Status.Should().Be(RetryStatus.Canceled);

                it["should raise OperationCanceledException"] = () =>
                    Assert.IsInstanceOfType(thrown, typeof(OperationCanceledException));
            };

        }

        ActionRunner GetRunner()
        {
            var runner = new ActionRunner();
            Action actor = () => { };
            runner.Actor = actor;
            runner.RetryCount = 1;
            return runner;
        }
    }
}
