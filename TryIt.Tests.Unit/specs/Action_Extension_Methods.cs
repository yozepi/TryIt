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

            describe["Action<T>.Try(T, retries)"] = () =>
            {
                before = () =>
                {
                    Action<int> action = (a1) => { };
                    subject = action.Try(1, retries);
                };
                it["Should return an ActionRetryBuilder instance"] = () =>
                     subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Action<T1, T2>.Try(T1, T2, retries)"] = () =>
            {
                before = () =>
                {
                    Action<int, int> action = (a1, a2) => { };
                    subject = action.Try(1, 2, retries);
                };
                it["Should return an ActionRetryBuilder instance"] = () =>
                    subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Action<T1, T2, T3>.Try(T1, T2, T3, retries)"] = () =>
            {
                before = () =>
                {
                    Action<int, int, int> action = (a1, a2, a3) => { };
                    subject = action.Try(1, 2, 3, retries);
                };
                it["Should return an ActionRetryBuilder instance"] = () =>
                   subject.Should().BeOfType<ActionRetryBuilder>();
            };

            describe["Action<T1, T2, T3, T4>.Try(T1, T2, T3, T4, retries)"] = () =>
            {
                before = () =>
                {
                    Action<int, int, int, int> action = (a1, a2, a3, a4) => { };
                    subject = action.Try(1, 2, 3, 4, retries);
                };
                it["Should return a ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();
            };
        }
    }
}
