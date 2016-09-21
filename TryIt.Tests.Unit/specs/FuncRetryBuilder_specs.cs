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
    class FuncRetryBuilder_specs : nspec
    {
        FuncRetryBuilder<string> subject = null;
        string expectedResult = "Hi mom!";
        string actualResult = null;

        void Go_Method()
        {
            before = () =>
            {
                actualResult = null;
                subject = new FuncRetryBuilder<string>();
                subject.AddRunner(GetRunner());
            };

            act = () => actualResult = subject.Go();

            it["should execute the runner and set Status to Success"] = () =>
                subject.Status.Should().Be(RetryStatus.Success);

            it["should return the expected result"] = () =>
                actualResult.Should().Be(expectedResult);

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
                actualResult = null;
                subject = new FuncRetryBuilder<string>();
                subject.AddRunner(GetRunner());
            };

            actAsync = async () =>  actualResult = await subject.GoAsync();

            it["should execute the runner and set Status to Success"] = () =>
                subject.Status.Should().Be(RetryStatus.Success);

            it["should return the expected result"] = () =>
                actualResult.Should().Be(expectedResult);

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

        FuncRunner<string> GetRunner()
        {
            var runner = new FuncRunner<string>();
            Func<string> actor = () => { return expectedResult; };
            runner.Actor = actor;
            runner.RetryCount = 1;
            return runner;
        }

    }
}
