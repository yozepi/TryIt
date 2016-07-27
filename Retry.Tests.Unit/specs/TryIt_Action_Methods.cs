using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace Retry.Tests.Unit.specs
{
    class TryIt_Action_Methods : nspec
    {
        void Static_Action_TryIt_Methods()
        {
            ITry subject = null;
            Action subjectAction = null;
            int actionExecutionCount = default(int);
            int retries = default(int);
            Exception thrown = null;

            beforeAll = () =>
            {
                actionExecutionCount = 0;
                retries = 3;
                thrown = null;
                subjectAction = () => actionExecutionCount++;
            };

            before = () =>
            {
                thrown = null;
                subject = TryIt.Try(subjectAction, retries);
            };


            describe["TryIt.Try(Action action, int retries)"] = () =>
            {

                it["should return an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                context["when the action is null"] = () =>
                {
                    before = () =>
                    {
                        try
                        {
                            TryIt.Try((Action)null, retries);
                        }
                        catch (Exception ex)
                        {
                            thrown = ex;
                        }
                    };
                    it["should throw ArgumentNullException"] = () =>
                        thrown.Should().BeOfType<ArgumentNullException>();
                };

            };

            describe["TryIt.Try(Action action, int retries).UsingDelay(IPause delay)"] = () =>
            {
                IDelay newPause = null;
                ITry result = null;
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

            describe["TryIt.Try(Action action, int retries).Go()"] = () =>
            {

                act = () =>
                {
                    try
                    {
                        subject.Go();
                    }
                    catch (Exception ex)
                    {
                        thrown = ex;
                    }
                };

                it["should call the action"] = () =>
                    actionExecutionCount.Should().Be(1);

                it["should have attempted the action only once"] = () =>
                    subject.Attempts.Should().Be(1);

                it["should set status to RetryStatus.Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

                it["should have an empty ExceptionList"] = () =>
                    subject.ExceptionList.Should().BeEmpty();

                context["when Try().Go() fails the first couple of times"] = () =>
                {
                    beforeAll = () =>
                    {
                        subjectAction = () =>
                        {
                            actionExecutionCount++;
                            if (actionExecutionCount < 2)
                                throw new Exception("Bad input!");
                        };
                    };

                    before = () => actionExecutionCount = 0;

                    it["should set the status to SuccessAfterRetries"] = () =>
                        subject.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                    it["should record the number of attempts Try made"] = () =>
                        subject.Attempts.Should().Be(actionExecutionCount);
                };

                context["when Try().Go() fails on every attempt"] = () =>
                {
                    beforeAll = () =>
                    {
                        subjectAction = () => { throw new Exception("Say WHAT!?!"); };
                    };

                    it["should have attempted the action asmay times aa RetryCount"] = () =>
                        subject.Attempts.Should().Be(subject.RetryCount);

                    it["should throw RetryFailedException"] = () =>
                        thrown.Should().BeOfType<RetryFailedException>();
                };

                context["when retries is an invalid value"] = () =>
                {
                    Action action = () => TryIt.Try(subjectAction, 0);
                    it["should throw an ArgumentOutOfRangeException"] = () =>
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["TryIt.Try(action, retries).ThenTry(retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                describe["TryIt.Try(action, retries).ThenTry(retries).Go()"] = () =>
                {
                    beforeAll = () =>
                    {
                        subjectAction = () => { };
                    };

                    act = () => subject.Go();

                    context["when the first try succeeds"] = () =>
                    {
                        it["should not ever execute ThenTry"] = () =>
                          child.Attempts.Should().Be(0);

                        it["Try should have executed once"] = () =>
                            subject.Attempts.Should().Be(1);
                    };

                };
            };

        }

        void Static_Action_T_TryIt_Methods()
        {
            ITry subject = null;
            Action<string> subjectAction = null;
            string actionExecutionString = null;
            string expectedActionExecutionString = null;
            int retries = default(int);
            Exception thrown = null;

            beforeAll = () =>
            {
                actionExecutionString = null;
                expectedActionExecutionString = "Hello!";
                retries = 3;
                thrown = null;
                subjectAction = (s) =>
                {
                    actionExecutionString = s;
                };

            };

            before = () => subject = TryIt.Try(subjectAction, expectedActionExecutionString, retries);

            describe["TryIt.Try(action, arg, retries).UsingDelay(delay)"] = () =>
            {
                IDelay newPause = null;
                ITry result = null;
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

            describe["TryIt.Try(action, arg, retries).Go()"] = () =>
            {

                act = () =>
                {
                    subject.Go();
                };

                it["should call the action"] = () =>
                    actionExecutionString.Should().Be(expectedActionExecutionString);

                it["should set status to RetryStatus.Success"] = () =>
                    subject.Status.Should().Be(RetryStatus.Success);

                it["should have an empty ExceptionList"] = () =>
                    subject.ExceptionList.Should().BeEmpty();

                context["when retries is an invalid value"] = () =>
                {
                    Action action = () => TryIt.Try(subjectAction, expectedActionExecutionString, 0);

                    it["should throw an ArgumentOutOfRangeException"] = () =>
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                };
            };

            describe["TryIt.Try(action, arg, retries).ThenTry(arg, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(expectedActionExecutionString, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                describe["TryIt.Try(action, arg, retries).ThenTry(altArg, retries).Go()"] = () =>
                {
                    context["When initial try fails, using alternate arg in ThenTry()"] = () =>
                    {
                        string expectedAltActionExecutionString = null;

                        beforeAll = () =>
                        {
                            expectedAltActionExecutionString = "World!";
                            subjectAction = (s) =>
                            {
                                if (s == expectedActionExecutionString)
                                    throw new Exception("Bad input!");

                                actionExecutionString = s;
                            };
                        };

                        before = () =>
                        {
                            subject = TryIt.Try(subjectAction, expectedActionExecutionString, retries);
                            child = subject.ThenTry(expectedAltActionExecutionString, 3);
                        };

                        act = () => child.Go();

                        it["should call the child with the alternate arg"] = () =>
                            actionExecutionString.Should().Be(expectedAltActionExecutionString);

                        it["should set the Status to SuccessAfterRetries"] = () =>
                            child.Status.Should().Be(RetryStatus.SuccessAfterRetries);

                        it["should have the exceptions of the parent in it's ExceptionsList"] = () =>
                            child.ExceptionList.Count.Should().Be(retries);

                        context["when the initial try succeeds"] = () =>
                        {

                            beforeAll = () =>
                            {
                                subjectAction = (s) =>
                                {
                                    actionExecutionString = s;
                                };
                            };

                            it["should set the status to Success"] = () =>
                                subject.Status.Should().Be(RetryStatus.Success);

                            it["shoud attempt to try only once"] = () =>
                                subject.Attempts.Should().Be(1);

                            it["should never attempt to execute ThenTry()"] = () =>
                                child.Attempts.Should().Be(0);

                        };
                    };
                };
            };

        }

        void Static_Action_T1_T2_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            int retries = 4;

            Action<string, int> subjectAction = (s, i) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1 and arg2 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).Go()"] = () =>
            {

                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2, arg 3 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                };
            };
        }

        void Static_Action_T1_T2_T3_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;

            int retries = 4;

            Action<string, int, double> subjectAction = (s, i, d) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, and arg3 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).Go()"] = () =>
            {

                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, retries).ThenTry(arg1, arg2, arg3, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, arg3, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2 and arg3 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int, double>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                };
            };
        }

        void Static_Action_T1_T2_T3_T4_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;

            int retries = 4;

            Action<string, int, double, long> subjectAction = (s, i, d, l) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, and arg4 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                };
            };

            describe["TryIt.Try(action, arg, arg2, arg3, arg4, retries).Go()"] = () =>
            {

                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, retries).ThenTry(arg1, arg2, arg3, arg4, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2, arg3, arg4 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int, double, long>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                };
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";

            int retries = 4;

            Action<string, int, double, long, string> subjectAction = (s1, i, d, l, s2) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, and arg5 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, retries).ThenTry(arg1, arg2, arg3, arg4, arg5, retries)"] = () =>
            {
                ITry child = null;
                before = () => child = subject.ThenTry(arg1, arg2, arg3, arg4, arg5, 3);
                it["should create a child TryIt instance"] = () =>
                    child.Should().NotBeNull();

                it["the child should be distinct from it's parent"] = () =>
                    child.Should().NotBe(subject);

                it["should set the arg1, arg2, arg3, arg4, arg5 internal properties on the child"] = () =>
                {
                    var asTryIt = child.As<ActionTryIt<string, int, double, long, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                };
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;

            int retries = 4;

            Action<string, int, double, long, string, bool> subjectAction = (s1, i, d, l, s2, b) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, and arg6 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_T7_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;
            long arg7 = long.MaxValue;

            int retries = 4;

            Action<string, int, double, long, string, bool, long> subjectAction = (s1, i, d, l1, s2, b, l2) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, arg6, and arg7 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool, long>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                    asTryIt._arg7.Should().Be(arg7);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, arg7, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_T7_T8_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;
            long arg7 = long.MaxValue;
            int arg8 = 33;

            int retries = 4;

            Action<string, int, double, long, string, bool, long, int> subjectAction = (s1, i1, d, l1, s2, b, l2, i2) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, arg6, arg7, and arg8 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool, long, int>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                    asTryIt._arg7.Should().Be(arg7);
                    asTryIt._arg8.Should().Be(arg8);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, arg7, arg8, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }

        void Static_Action_T1_T2_T3_T4_T5_T6_T7_T8_T9_TryIt_Methods()
        {
            string arg1 = "blah blah";
            int arg2 = 42;
            double arg3 = Math.PI;
            long arg4 = long.MinValue;
            string arg5 = "woof woof";
            bool arg6 = true;
            long arg7 = long.MaxValue;
            int arg8 = 33;
            string arg9 = "fore score";

            int retries = 4;

            Action<string, int, double, long, string, bool, long, int, string> subjectAction = (s1, i1, d, l1, s2, b, l2, i2, s3) => { };
            ITry subject = null;

            act = () => { subject = TryIt.Try(subjectAction, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, retries); };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, retries)"] = () =>
            {

                it["should create an ITry instance"] = () =>
                    subject.Should().NotBeNull();

                it["should set the arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, and arg9 internal properties"] = () =>
                {
                    var asTryIt = subject.As<ActionTryIt<string, int, double, long, string, bool, long, int, string>>();
                    asTryIt._arg1.Should().Be(arg1);
                    asTryIt._arg2.Should().Be(arg2);
                    asTryIt._arg3.Should().Be(arg3);
                    asTryIt._arg4.Should().Be(arg4);
                    asTryIt._arg5.Should().Be(arg5);
                    asTryIt._arg6.Should().Be(arg6);
                    asTryIt._arg7.Should().Be(arg7);
                    asTryIt._arg8.Should().Be(arg8);
                    asTryIt._arg9.Should().Be(arg9);
                };
            };

            describe["TryIt.Try(action, arg1, arg2, arg3, arg4,arg5, arg6, arg7, arg8, arg9, retries).Go()"] = () =>
            {
                act = () => subject.Go();
                it["should execute the action only once"] = () =>
                    subject.Attempts.Should().Be(1);
            };
        }
    }
}