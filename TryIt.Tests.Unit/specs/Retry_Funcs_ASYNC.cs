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
    class Retry_Funcs_ASYNC : nspec
    {
        void GoAsync_Method()
        {
            FuncRetryBuilder<string> subject = null;
            Func<string> subjectFunction = null;
            string expectedResult = "You were expecting me.";
            string actualResult = null;
            int retries = 3;
            int functionExecutionCount = default(int);

            before = () =>
            {
                functionExecutionCount = 0;
                subject = Retry.TryIt.Try(subjectFunction, retries);
            };

            describe["TryIt.Try(func, retries)GoAsync()"] = () =>
            {
                Exception thrown = null;
                actAsync = async () =>
                {
                    actualResult = null;
                    thrown = null;
                    try
                    {
                        actualResult = await subject.GoAsync();
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };
                context["when the func succeeds after one try"] = () =>
                {
                    beforeAll = () => subjectFunction = () => { functionExecutionCount++; return expectedResult; };

                    it["should return the expected result"] = () =>
                        actualResult.Should().Be(expectedResult);

                    it["should set status to Success"] = () =>
                       subject.Status.Should().Be(RetryStatus.Success);

                    it["should execute the action"] = () =>
                       functionExecutionCount.Should().NotBe(0);

                    it["should try the action only once"] = () =>
                       subject.Attempts.Should().Be(1);

                    it["should not throw any exceptions"] = () =>
                       thrown.Should().BeNull();

                };

                context["when the Func succeeds after a few attempts"] = () =>
                {

                    beforeAll = () => subjectFunction = () =>
                    {
                        functionExecutionCount++;
                        if (functionExecutionCount < retries)
                            throw new Exception("Something Changed!");

                        return expectedResult;
                    };

                    it["should return the expected result"] = () =>
                        actualResult.Should().Be(expectedResult);

                    it["should set status to SuccessAfterRetries"] = () =>
                       subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    it["should execute the action"] = () =>
                       functionExecutionCount.Should().NotBe(0);

                    it["should try the action multiple times"] = () =>
                       subject.Attempts.Should().BeGreaterThan(1);

                    it["should save exceptions in the ExceptionList"] = () =>
                        subject.ExceptionList.Count.Should().BeGreaterThan(0);

                    it["should not throw any exceptions"] = () =>
                       thrown.Should().BeNull();
                };

                context["when the Func fails after everey attempt"] = () =>
                {
                    beforeAll = () => subjectFunction = () =>
                    {
                        functionExecutionCount++;
                        throw new Exception("Something Changed!");
                    };

                    it["should never returned a result"] = () =>
                        actualResult.Should().BeNull();

                    it["should set status to SuccessAfterRetries"] = () =>
                       subject.Status.Should().Be(RetryStatus.Fail);

                    it["should attempt the action once for each retry"] = () =>
                       functionExecutionCount.Should().Be(retries);

                    it["should Set Attempts equal to the number of tries"] = () =>
                       subject.Attempts.Should().Be(retries);

                    it["should save exceptions in the ExceptionList"] = () =>
                        subject.ExceptionList.Count.Should().Be(retries);

                    it["should throw RetryFailedException"] = () =>
                        thrown.Should().BeOfType<RetryFailedException>();
                };

            };
        }
    }
}
