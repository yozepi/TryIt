using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry;
using Retry.Delays;

namespace TryIt.Tests.Unit.specs
{
    class IDelay_Implementors : nspec
    {
        void IDelay_Implementor_Classes()
        {
            int tryCount = 3;
            int tolerance = 25;
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
                await subject.WaitAsync(tryCount);
                executionTime = DateTime.Now.Subtract(start);
            };

            describe["NoDelay class"] = () =>
            {
                before = () => subject = new NoDelay();
                it["should return without any delay"] = () =>
                    executionTime.TotalMilliseconds.As<int>().Should().Be(0);
            };

            describe["TimedDelay class"] = () =>
            {
                var milliseconds = 100;
                TimeSpan delay = TimeSpan.FromMilliseconds(milliseconds);

                before = () => subject = new TimedDelay(delay);
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
                            subject = new TimedDelay(TimeSpan.MinValue);
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
                it["should backoff the delay, doubling the delay time with each try"] =() =>
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

                before = () => delay = Delay.Timed(delayTime);

                it["should return an instance of TimedDelay IDelay instance"] = () =>
                    delay.Should().BeOfType<TimedDelay>();

                it["should set the delay time correctly"] = () =>
                    delay.As<TimedDelay>().PauseTime.Should().Be(delayTime);
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
                    Delay.DefaultDelay = new TimedDelay(TimeSpan.FromDays(1));
                    Delay.DefaultDelay.Should().BeOfType<TimedDelay>();
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