using Moq;
using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Retry.Tests.Unit.specs
{
    class TryIt_Func_Methods :nspec
    {
        void Static_Func_TResult_TryItMethods()
        {
            ITryAndReturnValue<string> subject = null;
            Func<string> subjectFunc = null;
            string subjectResult = null;
            string expectedResult = null;
            int retries = default(int);
            Exception thrown = null;

            beforeAll = () =>
            {
                subjectResult = null;
                expectedResult = "Hello!";
                retries = 3;
                thrown = null;
                subjectFunc = () =>
                {
                    return expectedResult;
                };

            };

            before = () => subject = TryIt.Try(subjectFunc, retries);

            describe["TryIt.Try(func, retries).UsingDelay(delay)"] = () =>
            {
                IDelay newPause = null;
                ITryAndReturnValue<string> result = null;
                before = () =>
                {
                    result = null;
                    newPause = new Mock<IDelay>().Object;
                };

                act = () =>
                {
                    try
                    {
                        result = subject.UsingDelay(newPause);
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                it["Should set the Delay property"] = () =>
                    subject.Delay.Should().Be(newPause);

                it["should return the subject"] = () =>
                    result.Should().Be(subject);

                it["should not throw any exceptions"] = () =>
                    thrown.Should().BeNull();

                context["when the Delay parameter is null"] = () =>
                {
                    before = () => newPause = null;

                    it["should throw an ArgumentNullException"] = () =>
                        thrown.Should().BeOfType<ArgumentNullException>();

                };
            };

            describe["TryIt.Try(func, retries).Go()"] = () =>
            {

                act = () =>
                {
                    subjectResult = subject.Go();
                };

                it["should return the result of calling the Func"] = () =>
                    subjectResult.Should().Be(expectedResult);

                it["should set status to RetryStatus.Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

                it["should have an empty ExceptionList"] = () =>
                    subject.ExceptionList.Should().BeEmpty();

                context["when retries is an invalid value"] = () =>
                {
                    Action action = () => TryIt.Try(subjectFunc, 0);

                    it["should throw an ArgumentOutOfRangeException"] = () =>
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["TryIt.Try(func, retries).ThenTry(retries)"] = () =>
            {
                ITryAndReturnValue<string> child = null;
                before = () => child = subject.ThenTry(retries);

                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                describe["TryIt.Try(func, retries).ThenTry(retries).Go()"] = () =>
                {
                    context["When initial try fails, should excecute ThenTry()"] = () =>
                    {
                        string altSubjectResult = null;
                        string altExpectedResult = "World!";
                        int funcExecutedCount = default(int);
                        beforeAll = () =>
                        {
                            altSubjectResult = null;
                            funcExecutedCount = default(int);
                            altSubjectResult = null;
                            subjectFunc = () =>
                            {
                                funcExecutedCount++;
                                if (funcExecutedCount <= retries)
                                    throw new Exception("Bad input!");

                                return altExpectedResult;
                            };
                        };

                        before = () =>
                        {
                            funcExecutedCount = default(int);
                            subject = TryIt.Try(subjectFunc, retries);
                            child = subject.ThenTry(retries);
                        };

                        act = () => child.Go();

                        it["should return the result of Func"] = () =>
                            altSubjectResult.Should().Be(altSubjectResult);

                        it["should set the Status to SuccessAfterRetries"] = () =>
                            child.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                        it["should have the exceptions of the parent in it's ExceptionsList"] = () =>
                            child.ExceptionList.Count.Should().Be(retries);

                    };
                };
            };

        }

        void Static_Func_T_TResult_TryItMethods()
        {
            ITryAndReturnValue<string> subject = null;
            Func<int, string> subjectFunc = null;
            int arg = 23;
            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i) => { return expectedResult; };

            };

            act = () => subject = TryIt.Try(subjectFunc, arg, retries);
            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set the arg internal property"] = () =>
                {
                    var asTryIt = subject.As<FuncTryIt<int, string>>();
                    asTryIt._arg.Should().Be(arg);
                  };

            };

            describe["TryIt.Try(func, arg, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Func_T1_T2_TResult_TryItMethods()
        {
            ITryAndReturnValue<string> subject = null;
            Func<int, double, string> subjectFunc = null;
            int arg1 = 23;
            double arg2 = Math.E;
            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i, d) => { return expectedResult; };
            };

            act = () => subject = TryIt.Try(subjectFunc, arg1, arg2, retries);
            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set arg1 and arg2 internal properties"] = () =>
                {
                    var asTryIt = subject.As<FuncTryIt<int, double, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                };

            };

            describe["TryIt.Try(func, arg1, arg2, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Func_T1_T2_T3_TResult_TryItMethods()
        {
            ITryAndReturnValue<string> subject = null;
            Func<int, double, long, string> subjectFunc = null;

            int arg1 = 23;
            double arg2 = Math.E;
            long arg3 = long.MinValue;

            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i, d, l) => { return expectedResult; };
            };

            act = () => subject = TryIt.Try(subjectFunc, arg1, arg2, arg3, retries);
            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set arg1, arg2, and arg3 internal properties"] = () =>
                {
                    var asTryIt = subject.As<FuncTryIt<int, double, long, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                };

            };

            describe["TryIt.Try(func, arg1, arg2, arg3, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Func_T1_T2_T3_T4_TResult_TryItMethods() 
        {
            ITryAndReturnValue<string> subject = null;
            Func<int, double, long, string, string> subjectFunc = null;

            int arg1 = 23;
            double arg2 = Math.E;
            long arg3 = long.MinValue;
            string arg4 = "Happy to be here!";

            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i, d, l, s) => { return expectedResult; };
            };

            act = () => subject = TryIt.Try(subjectFunc, arg1, arg2, arg3, arg4, retries);
            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set arg1, arg2, arg3, and arg4 internal properties"] = () =>
                {
                    var asTryIt = subject.As<FuncTryIt<int, double, long, string, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                };

            };

            describe["TryIt.Try(func, arg1, arg2, arg3, arg4, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Func_T1_T2_T3_T4_T5_TResult_TryItMethods()
        {
            ITryAndReturnValue<string> subject = null;
            Func<int, double, long, string, float, string> subjectFunc = null;

            int arg1 = 23;
            double arg2 = Math.E;
            long arg3 = long.MinValue;
            string arg4 = "Happy to be here!";
            float arg5 = 373782.2378862F;

            string expectedResult = "Hi there!";
            int retries = 4;
            string actualResult = null;

            before = () =>
            {
                actualResult = null;
                subjectFunc = (i, d, l, s, f) => { return expectedResult; };
            };

            act = () => subject = TryIt.Try(subjectFunc, arg1, arg2, arg3, arg4, arg5, retries);
            describe["TryIt.Try(func, arg, retries)"] = () =>
            {
                it["should return an ITryAndReturnValue<TResult> instance"] = () =>
                    subject.Should().NotBeNull();


                it["should set arg1, arg2, arg3, arg4, and arg5 internal properties"] = () =>
                {
                    var asTryIt = subject.As<FuncTryIt<int, double, long, string, float, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                };

            };

            describe["TryIt.Try(func, arg1, arg2, arg3, arg4, arg5, retries).Go()"] = () =>
            {
                act = () => actualResult = subject.Go();

                it["should return the expected result"] = () =>
                    actualResult.Should().Be(expectedResult);

                it["should attempt the Try only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }
    }
}
