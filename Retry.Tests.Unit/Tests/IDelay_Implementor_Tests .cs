using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retry.Tests.Unit.specs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retry.Tests.Unit.Tests
{
    [TestClass]
    public class IDelay_Implementor_Tests : nSpecTestHarness
    {
        [TestMethod]
        public void IDelay_Implementors()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(IDelay_Implementors) };
                return types;
            });
            this.RunSpecs();
        }
    }
}
