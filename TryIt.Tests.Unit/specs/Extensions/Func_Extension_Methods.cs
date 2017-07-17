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
    public class Func_Extension_Methods : nSpecTestHarness
    {
        [TestMethod]
        public void FuncExtensionTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        void Func_extensions()
        {
            BaseBuilder subject = null;
            Func<bool> func = () => { return true; };
            int retries = 5;

            before = () => subject = null;

            describe["Func<Tresult>.Try(retries)"] = () =>
            {
                before = () =>
                {
                    subject = func.Try(retries);
                };
                it["Should return a FuncRetryBuilder<T> instance"] = () =>
                    subject.Should().BeOfType<FuncRetryBuilder<bool>>();
            };

            describe["Func<Tresult>.TryAsync(retries)"] = () =>
            {
                before = () =>
                {
                    subject = func.TryAsync(retries);
                };
                it["Should return a TaskRetryBuilder<T> instance"] = () =>
                    subject.Should().BeOfType<TaskRetryBuilder<bool>>();
            };

        }
    }
}
