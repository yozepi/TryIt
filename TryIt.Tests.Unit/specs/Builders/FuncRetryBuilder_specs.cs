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

namespace TryIt.Tests.Unit.specs.Builders
{
    [TestClass]
    public class FuncRetryBuilder_specs : nSpecTestHarness
    {

        [TestMethod]
        public void FuncRetryBuilderTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }


        Mock<FuncRetryBuilder<string>> subjectMock = null;
        FuncRetryBuilder<string> subject = null;
        string expectedResult = "Hi mom!";
        string actualResult = null;

        void Go_Method()
        {
            before = () =>
            {
                actualResult = null;
                var runner = new FuncRunner<string> { Result = expectedResult };
                subjectMock = new Mock<FuncRetryBuilder<string>>();
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
