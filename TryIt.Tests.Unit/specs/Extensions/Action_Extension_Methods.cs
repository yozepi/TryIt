using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using yozepi.Retry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yozepi.Retry.Builders;

namespace TryIt.Tests.Unit.specs
{
    [TestClass]
    public class Action_Extension_Methods : nSpecTestHarness
    {
        [TestMethod]
        public void ActionExtensionTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        void Action_extensions()
        {
            BaseBuilder subject = null;
            Action action = () => { };
            int retries = 5;

            before = () => subject = null;

            describe["Action.Try(retries)"] = () =>
            {
                before = () =>
                {
                    subject = action.Try(retries);
                };
                it["Should return a MethodRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<MethodRetryBuilder>();
            };

            describe["Action.TryAsync(retries)"] = () =>
            {
                before = () =>
                {
                    subject = action.TryAsync(retries);
                };
                it["Should return an TaskRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<TaskRetryBuilder>();
            };
        }
    }
}
