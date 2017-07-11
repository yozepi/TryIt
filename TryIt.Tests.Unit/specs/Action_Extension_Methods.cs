using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry;
using Retry.Builders;

namespace TryIt.Tests.Unit.specs
{
    class Action_Extension_Methods : nspec
    {
        void Action_extensions()
        {
            BaseBuilder subject = null;
            int retries = 5;

            before = () => subject = null;

            describe["Action.Try(retries)"] = () =>
            {
                before = () =>
                {
                    Action action = () => { };
                    subject = action.Try(retries);
                };
                it["Should return an ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();
            };
        }
    }
}
