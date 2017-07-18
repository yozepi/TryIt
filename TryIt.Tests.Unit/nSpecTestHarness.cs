using System.Linq;
using System.Reflection;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace TryIt.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    public class nSpecTestHarness: nspec
    {

        protected string Tags { get; set; }

        protected IFormatter Formatter { get; set; }

        protected bool FailFast { get; set; }

        private Func<Type[]> _specFinder = null;

        public nSpecTestHarness()
        {
            this.Formatter = new ConsoleFormatter();
        }

        protected void LoadSpecs(Func<Type[]> specFinder)
        {
            _specFinder = specFinder;
        }

        public void RunSpecs()
        {
            var types = FindSpecTypes();
            var finder = new SpecFinder(types, "");
            var tagsFilter = new Tags().Parse(Tags);
            var builder = new ContextBuilder(finder, tagsFilter, new DefaultConventions());
            var runner = new ContextRunner(tagsFilter, Formatter, FailFast);
            var contexts = builder.Contexts().Build();

            bool hasFocusTags = contexts.AnyTaggedWithFocus();
            if (hasFocusTags)
            {
                tagsFilter = new Tags().Parse(NSpec.Domain.Tags.Focus);

                builder = new ContextBuilder(finder, tagsFilter, new DefaultConventions());

                runner = new ContextRunner(tagsFilter, Formatter, FailFast);

                contexts = builder.Contexts().Build();
            }


            var results = runner.Run(contexts);

            //assert that there aren't any failures
            Assert.AreEqual<int>(0, results.Failures().Count());

            var pending = results.Examples().Count(xmpl => xmpl.Pending);
            if (pending != 0)
            {
                Assert.Inconclusive(string.Format("{0} spec(s) are marked as pending.", pending));
            }
            if (results.Examples().Count() == 0)
            {
                Assert.Inconclusive("Spec count is zero.");
            }
            if (hasFocusTags)
            {
                Assert.Inconclusive("One or more specs are tagged with focus.");
            }
        }

        private Type[] FindSpecTypes()
        {
            if (_specFinder != null)
            {
                return _specFinder();
            }
            return GetType().Assembly.GetTypes();

        }
    }
}