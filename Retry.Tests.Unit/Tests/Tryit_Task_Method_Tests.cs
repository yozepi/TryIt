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
    public class Tryit_Task_Method_Tests : nSpecTestHarness
    {
        [TestMethod]
public void Tryit_Task_Methods()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(TryIt_Task_Action_Methods), typeof(TryIt_Task_Func_Methods) };
                return types;
            });
            this.RunSpecs();
        }
    }
}
