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
    public class Tests : nSpecTestHarness
    {
        [TestMethod]
        public void Action_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(Retry_Actions), typeof(Retry_actions_ASYNC) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void Action_Extension_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(Action_Extension_Methods) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void Func_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(Retry_Funcs), typeof(Retry_Funcs_ASYNC) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void Func_Task_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(Retry_Tasks), typeof(Retry_TaskTResults) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void Func_Extension_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(Func_Extension_Methods) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void IDelay_Implementor_Tests()
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
