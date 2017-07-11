using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using Retry.Runners;

namespace TryIt.Tests.Unit.specs
{
    class RunnerFactory_specs : nspec
    {
        void building_from_existing_runners()
        {
            context["runner is ActionRunner"] = () =>
                it["should return an ActionRunner Instance"] = () =>
                    RunnerFactory.GetRunner(new ActionRunner())
                    .Should().BeOfType<ActionRunner>();


            context["runner is FuncRunner<TResult>"] = () =>
                 it["should return an FuncRunner<TResult> Instance"] = () =>
                     RunnerFactory.GetRunner(new FuncRunner<string>())
                     .Should().BeOfType<FuncRunner<string>>();

            context["runner is TaskRunner"] = () =>
                 it["should return a TaskRunner Instance"] = () =>
                     RunnerFactory.GetRunner(new TaskRunner())
                     .Should().BeOfType<TaskRunner>();

            context["runner is TaskRunner<T>"] = () =>
            {
                it["should return a TaskRunner<T> Instance"] = () =>
                     RunnerFactory.GetRunner(new TaskRunner<int>(42))
                     .Should().BeOfType<TaskRunner<int>>();

                context["when argument is provided"] = () =>
                    it["should set the runner's internal arguments"] = () =>
                    {
                        var runner = RunnerFactory.GetRunner(new TaskRunner<int>(42), 43)
                            .As<TaskRunner<int>>();
                        runner._arg.Should().Be(43);
                    };
            };

            context["runner is TaskRunner<T1, T2>"] = () =>
            {
                it["should return a TaskRunner<T1, T2> Instance"] = () =>
                     RunnerFactory.GetRunner(new TaskRunner<int, long>(42, long.MaxValue))
                     .Should().BeOfType<TaskRunner<int, long>>();

                context["when arguments are provided"] = () =>
                    it["should set the runner's internal arguments"] = () =>
                    {
                        var runner = RunnerFactory.GetRunner(new TaskRunner<int, long>(42, long.MaxValue), 43, long.MinValue)
                            .As<TaskRunner<int, long>>();
                        runner._arg1.Should().Be(43);
                        runner._arg2.Should().Be(long.MinValue);
                    };
            };

            context["runner is TaskWithResultRunner<TResult>"] = () =>
                 it["should return a TaskRunner<TResult> Instance"] = () =>
                     RunnerFactory.GetRunner(new TaskWithResultRunner<string>())
                     .Should().BeOfType<TaskWithResultRunner<string>>();

            context["runner is TaskWithResultRunner<T, TResult>"] = () =>
            {
                it["should return a TaskRunner<T, TResult> Instance"] = () =>
                     RunnerFactory.GetRunner(new TaskWithResultRunner<int, string>(42))
                     .Should().BeOfType<TaskWithResultRunner<int, string>>();

                context["when argument is provided"] = () =>
                    it["should set the runner's internal arguments"] = () =>
                    {
                        var runner = RunnerFactory.GetRunner(new TaskWithResultRunner<int, string>(42), 43)
                            .As<TaskWithResultRunner<int, string>>();
                        runner._arg.Should().Be(43);
                    };
            };

            context["runner is TaskWithResultRunner<T1, T2, TResult>"] = () =>
            {
                it["should return a TaskRunner<T1, T2, TResult> Instance"] = () =>
                     RunnerFactory.GetRunner(new TaskWithResultRunner<int, long, string>(42, long.MaxValue))
                     .Should().BeOfType<TaskWithResultRunner<int, long, string>>();

                context["when arguments are provided"] = () =>
                    it["should set the runner's internal arguments"] = () =>
                    {
                        var runner = RunnerFactory.GetRunner(new TaskWithResultRunner<int, long, string>(42, long.MaxValue), 43, long.MinValue)
                            .As<TaskWithResultRunner<int, long, string>>();
                        runner._arg1.Should().Be(43);
                        runner._arg2.Should().Be(long.MinValue);
                    };
            };
        }
    }
}
