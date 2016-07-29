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
    public class TryitFunc_Method_Tests : nSpecTestHarness
    {
        [TestMethod]
public void Tryit_Func_Methods()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(FuncTryIt_Methods), typeof(TryItFunc_Async_Methods) };
                return types;
            });
            this.RunSpecs();
        }
    }
}
