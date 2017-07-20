using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using yozepi.Retry.Builders;
using yozepi.Retry.Runners;
using yozepi.Retry;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TryIt.Tests.Unit.Builders
{
    [TestClass]
    public class TaskRetryBuilder_specs : nSpecTestHarness
    {
        [TestMethod]
        public void TaskRetryBuilderTests()
        {
            this.LoadSpecs(() => new Type[] { typeof(TaskRetryBuilder_specs) });
            this.RunSpecs();
        }


        Mock<TaskRetryBuilder> subject = null;

        void Go_Method()
        {
            before = () =>
            {
                subject = new Mock<TaskRetryBuilder>();
            };

            describe["Go()"] = () =>
            {
                actAsync = () => subject.Object.Go();

                it["should call the base class's internal RunAsync method"] = () =>
                    subject.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
            };

            describe["Go(CancellationToken)"] = () =>
            {
                act = () => subject.Object.Go(CancellationToken.None);

                it["should call the base class's internal RunAsync method"] = () =>
                    subject.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
            };
        }
    }
}
