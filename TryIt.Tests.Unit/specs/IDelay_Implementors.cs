﻿using NSpec;
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

            describe["FibonacciDelay class"] = () =>
            {
                var milliseconds = 100;
                TimeSpan delay = TimeSpan.FromMilliseconds(milliseconds);
                var expected = milliseconds * 2;

                before = () => subject = new FibonacciDelay(delay);
                it["should backoff the delay using the Fibonacci algorithm"] = () =>
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
                            subject = new FibonacciDelay(TimeSpan.MinValue);
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

                it["should set the initial delay time correctly"] = () =>
                    delay.As<BackoffDelay>().InitialDelay.Should().Be(delayTime);
            };

            describe["Fibonacci() method"] = () =>
            {
                IDelay delay = null;
                TimeSpan delayTime = TimeSpan.FromMilliseconds(100);

                before = () => delay = Delay.Fibonacci(delayTime);

                it["should return an instance of FibonacciDelay IDelay instance"] = () =>
                    delay.Should().BeOfType<FibonacciDelay>();

                it["should set the initial delay time correctly"] = () =>
                    delay.As<FibonacciDelay>().InitialDelay.Should().Be(delayTime);
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