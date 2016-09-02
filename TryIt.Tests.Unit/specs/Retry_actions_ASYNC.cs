using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Retry.Builders;
using Retry;

namespace TryIt.Tests.Unit.specs
{

    class Retry_actions_ASYNC : nspec
    {
        void GoAsync_Method()
        {
            ActionRetryBuilder subject = null;
            Action subjectAction = null;
            int retries = 3;

            int actionExecutionCount = default(int);

            before = () =>
            {
                actionExecutionCount = 0;
                subject = Retry.TryIt.Try(subjectAction, retries);
            };

            describe["TryIt.Try(action, retries)GoAsync()"] = () =>
            {
                Exception thrown = null;
                actAsync = async () =>
                {
                    thrown = null;
                    try
                    {
                        await subject.GoAsync();
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                context["when the action succeeds after one try"] = () =>
                 {
                     beforeAll = () => subjectAction = () => { actionExecutionCount++; };

                     it["should set status to Success"] = () =>
                        subject.Status.Should().Be(RetryStatus.Success);

                     it["should execute the action"] = () =>
                        actionExecutionCount.Should().NotBe(0);

                     it["should try the action only once"] = () =>
                        subject.Attempts.Should().Be(1);

                     it["should not throw any exceptions"] = () =>
                        thrown.Should().BeNull();

                 };

                context["when the action succeeds after a few attempts"] = () =>
                {

                    beforeAll = () => subjectAction = () =>
                    {
                        thrown = null;
                        actionExecutionCount++;
                        if (actionExecutionCount < retries)
                            throw new Exception("Something Changed!");
                    };

                    it["should set status to SuccessAfterRetries"] = () =>
                       subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    it["should execute the action"] = () =>
                       actionExecutionCount.Should().NotBe(0);

                    it["should try the action multiple times"] = () =>
                       subject.Attempts.Should().BeGreaterThan(1);

                    it["should save exceptions in the ExceptionList"] = () =>
                        subject.ExceptionList.Count.Should().BeGreaterThan(0);

                    it["should not throw any exceptions"] = () =>
                       thrown.Should().BeNull();
                };

                context["when the action fails after everey attempt"] = () =>
                {
                    beforeAll = () => subjectAction = () =>
                    {
                        actionExecutionCount++;
                        throw new Exception("Something Changed!");
                    };

                    it["should set status to SuccessAfterRetries"] = () =>
                       subject.Status.Should().Be(RetryStatus.Fail);

                    it["should attempt the action once for each retry"] = () =>
                       actionExecutionCount.Should().Be(retries);

                    it["should Set Attempts equal to the number of tries"] = () =>
                       subject.Attempts.Should().Be(retries);

                    it["should save exceptions in the ExceptionList"] = () =>
                        subject.ExceptionList.Count.Should().Be(retries);

                    it["should throw RetryFailedException"] = () =>
                        thrown.Should().BeOfType<RetryFailedException>();
                };
            };

            describe["TryIt.Try(action, retries).ThenTry(retries).GoAsync()"] = () =>
            {
                ActionRetryBuilder child = null;
                Exception thrown = null;
                actAsync = async () =>
                {
                    child = subject.ThenTry(retries);
                    await child.GoAsync();
                };

                context["when the action succeeds after the first try"] = () =>
                {
                    beforeAll = () => subjectAction = () => { actionExecutionCount++; };

                    it["should set status to Success"] = () =>
                        child.Status.Should().Be(RetryStatus.Success);

                    it["should execute the action"] = () =>
                       actionExecutionCount.Should().NotBe(0);

                    it["should try the action only once"] = () =>
                       subject.Attempts.Should().Be(1);

                    it["should not throw any exceptions"] = () =>
                       thrown.Should().BeNull();

                };

                context["when the first try fails after every attempt"] = () =>
                 {
                     context["and ThenTry succeeds"] = () =>
                     {

                         beforeAll = () => subjectAction = () =>
                         {
                             actionExecutionCount++;
                             if (child.Attempts == 0)
                                 throw new Exception("Help meeee!!!");
                         };

                         it["child status should be SuccessAfterRetries"] = () =>
                            child.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                         it["should have attempted the action once for each retry"] = () =>
                            subject.Attempts.Should().Be(retries + 1);

                         it["should have exceptions in the ExceptionList"] = () =>
                            subject.ExceptionList.Count.Should().Be(retries);

                         it["should not throw any exceptions"] = () =>
                            thrown.Should().BeNull();

                     };
                 };
            };
        }
    }
}
