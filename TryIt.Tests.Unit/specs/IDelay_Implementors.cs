using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry;
using Retry.Delays;
using System.Threading;

namespace TryIt.Tests.Unit.specs
{
    class Delay_Implementors : nspec
    {
        int tolerance = 25;

        void Delay_Cancellation()
        {
            IDelay subject = null;
            CancellationTokenSource tokenSource = null;
            CancellationToken token = CancellationToken.None;
            TimeSpan executionTime = default(TimeSpan);
            Exception thrown = null;

            int cancelDelay = default(int);
            before = () =>
            {
                subject = Delay.Basic(TimeSpan.FromSeconds(3));
                tokenSource = new CancellationTokenSource(cancelDelay);
                token = tokenSource.Token;
                executionTime = TimeSpan.Zero;
                thrown = null;
            };
            actAsync = async () =>
            {
                var start = DateTime.Now;
                try
                {
                    await subject.WaitAsync(1, token);
                }
                catch (Exception ex)
                {
                    thrown = ex;
                }
                executionTime = DateTime.Now.Subtract(start);

            };
            afterAll = () =>
            {
                tokenSource.Dispose();
            };

            describe["when the task is cancelled before the delay starts"] = () =>
            {
                it["should throw TaskCanceledException"] = () =>
                {
                    thrown.Should().BeOfType<TaskCanceledException>();
                };
                it["should not delay"] = () =>
                {
                    executionTime.Should().BeGreaterOrEqualTo(TimeSpan.Zero);
                    executionTime.Should().BeLessOrEqualTo(TimeSpan.Zero.Add((TimeSpan.FromMilliseconds(tolerance))));
                };
            };
        }

        void Delay_SubClasses()
        {
            int tryCount = 3;
            IDelay subject = null;
            TimeSpan executionTime = default(TimeSpan);

            before = () =>
            {
                executionTime = TimeSpan.Zero;
                subject = null;
            };
            actAsync = async () =>
            {
                var start = DateTime.Now;
                await subject.WaitAsync(tryCount, CancellationToken.None);
                executionTime = DateTime.Now.Subtract(start);
            };

            describe["NoDelay class"] = () =>
            {
                before = () => subject = new NoDelay();
                it["should return without any delay"] = () =>
                    executionTime.TotalMilliseconds.As<int>().Should().Be(0);
            };

            describe["BasicDelay class"] = () =>
            {
                var milliseconds = 100;
                TimeSpan delay = TimeSpan.FromMilliseconds(milliseconds);

                before = () => subject = new BasicDelay(delay);
                it["should delay for the provided TimeSpan"] = () =>
                     {
                         executionTime.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(milliseconds));
                         executionTime.Should().BeLessOrEqualTo(TimeSpan.FromMilliseconds(milliseconds + tolerance));
                     };

                context["when the provided timespan is invalid"] = () =>
                {
                    it["should throw an ArgumentOutOfRange exception"] = () =>
                    {
                        Exception thrown = null;
                        try
                        {
                            subject = new BasicDelay(TimeSpan.MinValue);
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }

                        thrown.Should().BeOfType<ArgumentOutOfRangeException>();
                    };
                };
            };

            describe["BackoffDelay class"] = () =>
            {
                var milliseconds = 100;
                TimeSpan delay = TimeSpan.FromMilliseconds(milliseconds);
                var expected = milliseconds * Math.Pow(2, (tryCount - 1));

                before = () => subject = new BackoffDelay(delay);
                it["should backoff the delay, doubling the delay time with each try"] = () =>
                    {
                        executionTime.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(expected));
                        executionTime.Should().BeLessOrEqualTo(TimeSpan.FromMilliseconds(expected + tolerance));
                    };

                context["when the provided timespan is invalid"] = () =>
                {
                    it["should throw an ArgumentOutOfRange exception"] = () =>
                    {
                        Exception thrown = null;
                        try
                        {
                            subject = new BackoffDelay(TimeSpan.MinValue);
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }

                        thrown.Should().BeOfType<ArgumentOutOfRangeException>();
                    };
                };
            };
        }

        void Static_Delay_Methods()
        {
            describe["NoDelay() method"] = () =>
             {
                 it["should return an instance of NoDelay IDelay instance"] = () =>
                 {
                     var delay = Delay.NoDelay();
                     delay.Should().BeOfType<NoDelay>();
                 };
             };

            describe["Timed() method"] = () =>
            {
                IDelay delay = null;
                TimeSpan delayTime = TimeSpan.FromMilliseconds(100);

                before = () => delay = Delay.Basic(delayTime);

                it["should return an instance of TimedDelay IDelay instance"] = () =>
                    delay.Should().BeOfType<BasicDelay>();

                it["should set the delay time correctly"] = () =>
                    delay.As<BasicDelay>().PauseTime.Should().Be(delayTime);
            };

            describe["Backoff() method"] = () =>
            {
                IDelay delay = null;
                TimeSpan delayTime = TimeSpan.FromMilliseconds(100);

                before = () => delay = Delay.Backoff(delayTime);

                it["should return an instance of BackoffDelay IDelay instance"] = () =>
                    delay.Should().BeOfType<BackoffDelay>();

                it["should set the delay time correctly"] = () =>
                    delay.As<BackoffDelay>().InitialDelay.Should().Be(delayTime);
            };

            describe["DefaultDelay property"] = () =>
            {
                it["should return an NoDelay instance when not previously set"] = () =>
                    Delay.DefaultDelay.Should().BeOfType<NoDelay>();

                it["when set, DefaultDelay should return the value set"] = () =>
                {
                    Delay.DefaultDelay = new BasicDelay(TimeSpan.FromDays(1));
                    Delay.DefaultDelay.Should().BeOfType<BasicDelay>();
                };

                it["when set to null, DefaultDelay should return a NoDelay instance"] = () =>
                {
                    Delay.DefaultDelay = null;
                    Delay.DefaultDelay.Should().BeOfType<NoDelay>();
                };
            };


        }
    }
}