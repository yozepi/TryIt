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
    public class Func_Extension_Method_Tests : nSpecTestHarness
    {
        [TestMethod]
        public void Func_Extension_Methods()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(Func_Extension_Methods) };
                return types;
            });
            this.RunSpecs();
        }
    }
}
