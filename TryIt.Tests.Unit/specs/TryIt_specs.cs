using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yozepi.Retry.Builders;
using yozepi.Retry;
using yozepi.Retry.Runners;
using yozepi.Retry.Delays;
using yozepi.Retry.Exceptions;
using NSpec;

namespace TryIt.Tests.Unit
{
    [TestClass]
    public class TryIt_specs : nSpecTestHarness
    {
        [TestMethod]
        public void TryItTests()
        {
            this.LoadSpecs(() => new Type[] { this.GetType() });
            this.RunSpecs();
        }

        void Try_Actions()
        {
            describe["Try(Action)"] = () =>
            {
                ActionRetryBuilder subject = null;
                Action actor = () => { };
                int retries = 5;

                act = () => subject = yozepi.Retry.TryIt.Try(actor, retries);

                it["should return an ActionRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry(altAction)"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                Action altActor = () => { };
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an ActionRunner"] = () =>
                    subject.LastRunner.Should().BeOfType<ActionRunner>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["UsingDelay Method"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                IDelay expectedDelay = Delay.Basic(TimeSpan.FromSeconds(1));

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.UsingDelay(expectedDelay);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);
            };

            describe["WithErrorPolicy Method"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithErrorPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the error policy on the last runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);
            };

            describe["WithSuccessPolicy Method"] = () =>
            {
                ActionRetryBuilder subject = null;
                ActionRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                SuccessPolicyDelegate expectedPolicy = (tries) => { };

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithSuccessPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the success policy on the last runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);
            };

        }

        [Tag("focus")]
        void Try_Funcs()
        {
            describe["Try(Func<T>)"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                Func<string> actor = () => { return "Try to take over the world!"; };
                int retries = 5;

                act = () => subject = yozepi.Retry.TryIt.Try(actor, retries);

                it["should return a FuncRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);

                describe["when trying to pass a Func that returns a Task"] = () =>
                {
                    Func<Task> taskActor = () => new Task(() => { });
                    Action action = () => yozepi.Retry.TryIt.Try(taskActor, retries);

                    it["should throw TaskNotAllowedException"] = () =>
                        action.ShouldThrow<TaskNotAllowedException>()
                        .And.Message.Should().Be(yozepi.Retry.TryIt.TaskErrorMessage);

                };

                describe["when trying to pass a Func that returns a Task<T>"] = () =>
                {
                    Func<Task<int>> taskActor = () => new Task<int>(() => { return 2; });
                    Action action = () => yozepi.Retry.TryIt.Try(taskActor, retries);

                    it["should throw TaskNotAllowedException"] = () =>
                        action.ShouldThrow<TaskNotAllowedException>()
                        .And.Message.Should().Be(yozepi.Retry.TryIt.TaskErrorMessage);

                };
            };

            describe["ThenTry"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Try to take over the world!"; };
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<string>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry(alternate Func<T>)"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "What are we going to do tomorrow?"; };
                Func<string> altActor = () => { return "Try to take over the world!"; };
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an FuncRunner<TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<FuncRunner<string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["UsingDelay Method"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                IDelay expectedDelay = Delay.Basic(TimeSpan.FromSeconds(1));

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.UsingDelay(expectedDelay);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);
            };

            describe["WithErrorPolicy Method"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithErrorPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the error policy on the last runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);
            };

            describe["WithSuccessPolicy Method"] = () =>
            {
                FuncRetryBuilder<string> subject = null;
                FuncRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                SuccessPolicyDelegate<string> expectedPolicy = (result, tries) => { };

                before = () => sourceBuilder = yozepi.Retry.TryIt.Try(actor, 1);
                act = () => subject = sourceBuilder.WithSuccessPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the success policy on the last runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);
            };
        }

        void Try_Tasks()
        {
            describe["TryAsync(Func<Task>)"] = () =>
            {
               TaskRetryBuilder subject = null;
                Func<Task> actor = new Func<Task>(() => new Task(new Action(() => { })));
                int retries = 5;

                act = () => subject =  yozepi.Retry.TryIt.TryAsync(actor, retries);

                it["should return a TaskRetryBuilder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry"] = () =>
            {
                TaskRetryBuilder subject = null;
                TaskRetryBuilder sourceBuilder = null;
                Func<Task> actor = new Func<Task>(() => new Task(new Action(() => { })));
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an TaskRunner"] = () =>
                    subject.LastRunner.Should().BeOfType<TaskRunner>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry(alternate Func<Task>)"] = () =>
            {
                TaskRetryBuilder subject = null;
                TaskRetryBuilder sourceBuilder = null;
                Func<Task> actor = new Func<Task>(() => new Task(new Action(() => { })));
                Func<Task> altActor = new Func<Task>(() => new Task(new Action(() => { })));
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an TaskRunner"] = () =>
                    subject.LastRunner.Should().BeOfType<TaskRunner>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry(alternate Action)"] = () =>
            {
                TaskRetryBuilder subject = null;
                TaskRetryBuilder sourceBuilder = null;
                Func<Task> actor = new Func<Task>(() => new Task(new Action(() => { })));
                Action altActor =() => new Task(new Action(() => { }));
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an TaskRunner"] = () =>
                    subject.LastRunner.Should().BeOfType<TaskRunner>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().BeOfType<Func<Task>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["UsingDelay Method"] = () =>
            {
                TaskRetryBuilder subject = null;
                TaskRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                IDelay expectedDelay = Delay.Basic(TimeSpan.FromSeconds(1));

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.UsingDelay(expectedDelay);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);
            };

            describe["WithErrorPolicy Method"] = () =>
            {
                TaskRetryBuilder subject = null;
                TaskRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.WithErrorPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the error policy on the last runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);
            };

            describe["WithSuccessPolicy Method"] = () =>
            {
                TaskRetryBuilder subject = null;
                TaskRetryBuilder sourceBuilder = null;
                Action actor = () => { };
                SuccessPolicyDelegate expectedPolicy = (tries) => { };

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.WithSuccessPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the success policy on the last runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);
            };

        }


        void Try_Task_T()
        {
            describe["Try(Func<Task<T>>)"] = () =>
            {
                TaskRetryBuilder<string> subject = null;
                Func<Task<string>> actor = () => new Task<string>(() => { return "Try to take over the world!"; });
                int retries = 5;

                act = () => subject = yozepi.Retry.TryIt.TryAsync(actor, retries);

                it["should return a TaskRetryBuilder<T>"] = () =>
                    subject.Should().NotBeNull();

                it["should add a runner"] = () =>
                    subject.Runners.Count.Should().NotBe(0);

                it["should set the runner's Actor"] = () =>
                    subject.LastRunner.Actor.Should().BeSameAs(actor);

                it["should set the runners's RetryCount"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry"] = () =>
            {
                TaskRetryBuilder<string> subject = null;
                TaskRetryBuilder<string> sourceBuilder = null;
                Func<Task<string>> actor = () => new Task<string>(() => { return "Try to take over the world!"; });
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an TaskRunner<TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<TaskRunner<string>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry(alternate Func<Task<T>>)"] = () =>
            {
                TaskRetryBuilder<string> subject = null;
                TaskRetryBuilder<string> sourceBuilder = null;
                Func<Task<string>> actor = () => new Task<string>(() => { return "What are we going to do tomorrow?"; });
                Func<Task<string>> altActor = () => new Task<string>(() => { return "Try to take over the world!"; });
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an TaskRunner<TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<TaskRunner<string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().Be(altActor);

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["ThenTry(alternate Func<T>)"] = () =>
            {
                TaskRetryBuilder<string> subject = null;
                TaskRetryBuilder<string> sourceBuilder = null;
                Func<Task<string>> actor = () => new Task<string>(() => { return "What are we going to do tomorrow?"; });
                Func<string> altActor = () => { return "Try to take over the world!"; };
                int retries = 5;

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.ThenTry(altActor, retries);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should add a new runner"] = () =>
                    subject.Runners.Count.Should().Be(2);

                it["the new runner should be an TaskRunner<TResult>"] = () =>
                    subject.LastRunner.Should().BeOfType<TaskRunner<string>>();

                it["should set the Actor of the new runner to the alternate action"] = () =>
                    subject.LastRunner.Actor.Should().BeOfType<Func<Task<string>>>();

                it["should set the retryCount of the new runner"] = () =>
                    subject.LastRunner.RetryCount.Should().Be(retries);
            };

            describe["UsingDelay Method"] = () =>
            {
                TaskRetryBuilder<string> subject = null;
                TaskRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                IDelay expectedDelay = Delay.Basic(TimeSpan.FromSeconds(1));

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.UsingDelay(expectedDelay);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the delay on the last runner"] = () =>
                    subject.LastRunner.Delay.Should().BeSameAs(expectedDelay);
            };

            describe["WithErrorPolicy Method"] = () =>
            {
                TaskRetryBuilder<string> subject = null;
                TaskRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                ErrorPolicyDelegate expectedPolicy = (ex, tries) => { return true; };

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.WithErrorPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the error policy on the last runner"] = () =>
                    subject.LastRunner.ErrorPolicy.Should().BeSameAs(expectedPolicy);
            };

            describe["WithSuccessPolicy Method"] = () =>
            {
                TaskRetryBuilder<string> subject = null;
                TaskRetryBuilder<string> sourceBuilder = null;
                Func<string> actor = () => { return "Hello world!"; };
                SuccessPolicyDelegate<string> expectedPolicy = (result, tries) => { };

                before = () => sourceBuilder = yozepi.Retry.TryIt.TryAsync(actor, 1);
                act = () => subject = sourceBuilder.WithSuccessPolicy(expectedPolicy);

                it["should return the the source builder"] = () =>
                    subject.Should().NotBeNull();

                it["should set the success policy on the last runner"] = () =>
                    subject.LastRunner.SuccessPolicy.Should().BeSameAs(expectedPolicy);
            };

        }
    }
}
