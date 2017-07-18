﻿using System;
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

namespace TryIt.Tests.Unit.specs.Builders
{
    [TestClass]
    public class ActionRetryBuilder_specs : nSpecTestHarness
    {

        [TestMethod]
        public void ActionRetryBuilderTests()
        {
            this.LoadSpecs(() => new Type[] { typeof(ActionRetryBuilder_specs) });
            this.RunSpecs();
        }


        Mock<RetryBuilder> subject = null;

        void Go_Method()
        {
            before = () =>
            {
                subject = new Mock<RetryBuilder>();
            };

            describe["Go()"] = () =>
            {
                act = () => subject.Object.Go();

                it["should call the base class's internal Run method"] = () =>
                    subject.Verify(m => m.Run(It.IsAny<CancellationToken>()), Times.Once);
            };

            describe["Go(CancellationToken)"] = () =>
            {
                act = () => subject.Object.Go(CancellationToken.None);

                it["should call the base class's internal Run method"] = () =>
                    subject.Verify(m => m.Run(It.IsAny<CancellationToken>()), Times.Once);
            };
        }
    }
}
