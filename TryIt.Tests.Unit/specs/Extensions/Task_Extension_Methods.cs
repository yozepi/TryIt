using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using yozepi.Retry.Builders;
using yozepi.Retry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TryIt.Tests.Unit.specs
{
    [TestClass]
    public class Task_Extension_Methods : nSpecTestHarness
    {
        [TestMethod]
        public void TaskExtensionTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        void Task_extensions()
        {
            BaseBuilder subject = null;
            int retries = 5;

            before = () => subject = null;

            describe["Func<Task>.TryAsync(retries)"] = () =>
            {
                Func<Task> task = () => new Task(() => { });

                before = () =>
                {
                    subject = task.TryAsync(retries);
                };
                it["Should return a TaskRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<TaskRetryBuilder>();
            };

            describe["Func<Task<T>>.TryAsync(retries)"] = () =>
            {
                Func<Task<bool>> task = () => new Task<bool>(() => true);

                before = () =>
                {
                    subject = task.TryAsync(retries);
                };
                it["Should return a TaskRetryBuilder<T> instance"] = () =>
                    subject.Should().BeOfType<TaskRetryBuilder<bool>>();
            };

        }
    }
}
