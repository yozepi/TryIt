using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using yozepi.Retry.Builders;
using yozepi.Retry.Runners;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TryIt.Tests.Unit.Builders
{
    [TestClass]
    public class Task_T_RetryBuilder_specs : nSpecTestHarness
    {

        [TestMethod]
        public void TaskTRetryBuilderTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }



        Mock<TaskRetryBuilder<string>> subjectMock = null;
        TaskRetryBuilder<string> subject = null;
        string expectedResult = "Hi mom!";
        string actualResult = null;

        void Go_Method()
        {
            before = () =>
            {
                actualResult = null;
                var runner = new TaskRunner<string> { Result = expectedResult };
                subjectMock = new Mock<TaskRetryBuilder<string>>();
                subjectMock.Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.Factory.StartNew(() => { }));
                subjectMock.CallBase = true;

                subject = subjectMock.Object;
                subject.Winner = runner;
            };

            describe["Go()"] = () =>
            {
                actAsync = async () =>  actualResult = await subject.Go();

                it["should call the base class's internal RunAsync method"] = () =>
                    subjectMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once);

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);
            };

            describe["Go(CancellationToken)"] = () =>
            {
                actAsync = async () => actualResult = await subject.Go(CancellationToken.None);

                it["should call the base class's internal RunAsync method"] = () =>
                    subjectMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once);

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);
            };

        }


    }
}
