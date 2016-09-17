using Microsoft.VisualStudio.TestTools.UnitTesting;
using TryIt.Tests.Unit.specs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryIt.Tests.Unit.Tests
{
    [TestClass]
    public class Tests : nSpecTestHarness
    {
        [TestMethod]
        public void BaseBuilder_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(BaseBuilder_specs) };
                return types;
            });
            this.RunSpecs();
        }

        [TestMethod]
        public void Builder_Implementor_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(ActionRetryBuilder_specs), typeof(FuncRetryBuilder_specs) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void TryIt_Action_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(TryIt_Action_specs) };
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
        public void TryIt_Func_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(TryIt_Func_specs) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void Func_Task_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(TryIt_Actions_on_Tasks), typeof(TryIt_Funcs_on_TaskTResults) };
                return types;
            });
            this.RunSpecs();
        }

        [TestMethod]
        public void TryIt_UsingDelay_WithError_WithSuccess_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(TryIt_UsingDelay_WithError_WithSuccess_specs) };
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
                Type[] types = { typeof(Delay_Implementors) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void BaseRunner_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(BaseRunner_specs) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void ActionRunner_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(ActionRunner_specs) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void FuncRunner_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(FuncRunner_specs) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void TaskRunner_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(TaskRunner_specs) };
                return types;
            });
            this.RunSpecs();
        }


        [TestMethod]
        public void TaskWithResultRunner_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(TaskWithResultRunner_specs) };
                return types;
            });
            this.RunSpecs();
        }

        [TestMethod]
        public void RunnerFactory_Tests()
        {
            this.LoadSpecs(() =>
            {
                Type[] types = { typeof(RunnerFactory_specs) };
                return types;
            });
            this.RunSpecs();
        }

    }
}
