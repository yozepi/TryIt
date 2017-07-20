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
using yozepi.Retry.Exceptions;

namespace TryIt.Tests.Unit.Builders
{
    [TestClass]
    public class MethodRetryBuilder_T_specs : nSpecTestHarness
    {

        [TestMethod]
        public void MethodRetryBuilderofTTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }


        Mock<MethodRetryBuilder<string>> subjectMock = null;
        MethodRetryBuilder<string> subject = null;
        string expectedResult = "Hi mom!";
        string actualResult = null;

        void Constructor_Behavior()
        {
            describe["when the generic result would be a Task"] = () =>
            {
                it["should throw TaskNotAllowedException"] = () =>
                {
                    Action constructor = () => new MethodRetryBuilder<Task>();
                    constructor.ShouldThrow<TaskNotAllowedException>()
                    .And.Message.Should().Be(MethodRetryBuilder<Task>.TaskErrorMessage);
                };
            };
            describe["when the generic result would be a Task<T>"] = () =>
            {
                it["should throw TaskNotAllowedException"] = () =>
                {
                    Action constructor = () => new MethodRetryBuilder<Task<int>>();
                    constructor.ShouldThrow<TaskNotAllowedException>()
                    .And.Message.Should().Be(MethodRetryBuilder<Task>.TaskErrorMessage);
                };
            };
        }


        void Go_Method()
        {
            before = () =>
            {
                actualResult = null;
                var runner = new FuncRunner<string> { Result = expectedResult };
                subjectMock = new Mock<MethodRetryBuilder<string>>();
                subjectMock.Setup(m => m.Run(It.IsAny<CancellationToken>()));
                subjectMock.CallBase = true;

                subject = subjectMock.Object;
                subject.Winner = runner;
            };

            describe["Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should call the base class's internal Run method"] = () =>
                    subjectMock.Verify(m => m.Run(It.IsAny<CancellationToken>()), Times.Once);

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);
            };

            describe["Go(CancellationToken)"] = () =>
            {
                act = () => actualResult = subject.Go(CancellationToken.None);

                it["should call the base class's internal Run method"] = () =>
                    subjectMock.Verify(m => m.Run(It.IsAny<CancellationToken>()), Times.Once);

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);
            };

        }


    }
}
