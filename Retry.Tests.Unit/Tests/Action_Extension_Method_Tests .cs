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
    public class Action_Extension_Method_Tests : nSpecTestHarness
    {
        [TestMethod]
        public void Action_Extension_Methods()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(Action_Extension_Methods) };
                return types;
            });
            this.RunSpecs();
        }
    }
}
