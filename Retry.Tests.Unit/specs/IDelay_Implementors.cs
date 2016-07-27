using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Retry.Tests.Unit.specs
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

            describe["NoDelay"] = () =>
            {
                before = () => subject = new NoDelay();
                it["should return without any delay"] = () =>
                    executionTime.TotalMilliseconds.As<int>().Should().Be(0);
            };

            describe["TimedPause"] = () =>
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

            describe["BackoffPause"] = () =>
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
            };
        }
    }
}