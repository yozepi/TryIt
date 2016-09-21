using Retry;
using Retry.Builders;
using Retry.Delays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TryIt_Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = MainAsync(args);
            task.Wait();
        }

        static async Task MainAsync(string[] args)
        {

            Try_A_Method();

            await Try_A_Method_Async();

            Try_A_Method_with_Cancellation();

            Try_A_Task();

            await Try_A_Task_Async();

            Try_A_Method_A_B();

            Try_A_Method_A_B_Handle_Error();

            await Try_A_Method_A_B_Async();

            Try_A_Task_A_B();

            await Try_A_Task_A_B_Async();

            Try_A_Method_A_With_ALT_Method();

            Try_A_Method_And_Handle_Error();

            Try_Method_Quick_Then_BackOff();

            RunTask(Try_Method_Quick_Then_BackOff_Async());

            Try_Task_Quick_Then_BackOff();

            RunTask(Try_Task_Quick_Then_BackOff_Async());

            Try_a_Method_ABC_Alternating();

            Try_a_Method_ABC_Alternating_WithCancellationToken();

            Build_Runners_Via_Code();

            Capture_Exception();


            Console.WriteLine();
            Console.WriteLine("Preformance Comparison...");
            Action testAction = () => { };
            Console.Write("When calling the method directly: ");
            PerformanceTest(testAction);
            Console.Write("When calling via Try: ");
            PerformanceTest(TryIt.Try(testAction, 1).Go);

            Console.WriteLine();
            //Console.WriteLine("Press any key to continue . . .");
            //Console.ReadKey();

        }

        private static void PerformanceTest(Action testAction)
        {
            int i = 100000;
            //Prime the action.
            testAction();
            var start = DateTime.Now;
            for (int c = 0; c < i; c++)
            {
                testAction();
            }
            var end = DateTime.Now.Subtract(start);
            Console.WriteLine("{0} ms ({1:###,###,##0} iterations)", end.TotalMilliseconds, i);
        }

        static void RunTask(Task task)
        {
            var start = DateTime.Now;
            var ticks = 0;
            while (!task.GetAwaiter().IsCompleted)
            {
                ticks++;
            }
            var sts = task.Status;
            Console.WriteLine();
            Console.WriteLine("Exit status = {0}", sts);
            Console.WriteLine("I did {0:###,###,###,###,##0} things while trying the task!", ticks);

            var duration = DateTime.Now.Subtract(start);
            Console.WriteLine("Time to execute: {0}", duration);

        }

        static string DownloadString(string url)
        {
            using (var request = new WebClient())
            {
                string response = request.DownloadString(url);
                return response;
            }
        }

        static async Task<string> DownloadStringAsync(string url)
        {
            using (var request = new WebClient())
            {
                var response = await TryIt.TryTask(request.DownloadStringTaskAsync, url, 3)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                    .GoAsync();
                return response;
            }
        }


        static Func<string, string> DownloadStringCommand()
        {
            return (url) => DownloadString(url);
        }


        static string Example1_WithoutTryIt()
        {
            //Without using TryIt
            var url = "http://www.google.com";
            string result = DownloadString(url);
            return result;
        }

        static string Example1_Using_TryIt()
        {
            //TryIt 3 times
            var url = "http://www.google.com";
            string result = TryIt.Try(DownloadString, url, 5).Go();
            return result;
        }

        static void Try_A_Method()
        {
            Console.WriteLine();
            Console.Write("Try a method");

            var url = "http://www.google.com";
            string result = TryIt.Try(DownloadString, url, 5)
                .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                .Go();
            Console.WriteLine(" - Response length: {0}", result.Length);
        }

        static void Try_A_Method_with_Cancellation()
        {
            Console.WriteLine();
            Console.Write("Try a method. Cancel if it takes too long to try.");
            var url = "http://www.google.com";

            try
            {
                string result;
                using (var tokenSource = new CancellationTokenSource(4500))
                {
                    result = TryIt.Try(DownloadString, url, 5)
                        .UsingDelay(Delay.Basic(TimeSpan.FromMilliseconds(200)))
                        .Go(tokenSource.Token);
                }
                Console.WriteLine(" - Response length: {0}", result.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }
 
        }

        static string Try_A_Method_Quick_Then_Backoff()
        {
            //Try request.DownloadString(url) 3 quickly
            //then try 5 times using a Backoff delay.
            var url = "http://www.google.com";
            string result = TryIt.Try(DownloadString, url, 3)
                .ThenTry(5)
                .UsingDelay(Delay.Basic(TimeSpan.FromMilliseconds(200)))
                .Go();
            return result;
        }

        static async Task Try_A_Method_Async()
        {
            Console.WriteLine();
            Console.Write("Try a method ASYNC");

            var url = "http://www.google.com";
            string response = await TryIt.Try(DownloadString, url, 3)
               .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
               .GoAsync();

            Console.WriteLine(" - Response length: {0}", response.Length);
        }


        static void Capture_Exception()
        {
            Console.WriteLine();
            Console.WriteLine("Capturing a RetryFailedException: ");

            string response = null;
            try
            {
                TryIt.Try(MyFailingMethod, 5).Go();
                response = "I shouldn't get here.";
            }
            catch (RetryFailedException ex)
            {
                Console.WriteLine("{0}", ex.Message);
                Console.WriteLine("ExceptionList Count: {0}", ex.ExceptionList.Count);
            }

        }

        static void MyFailingMethod() { throw new Exception("I Failed"); }


        static void Try_A_Task()
        {
            Console.WriteLine();
            Console.Write("Try a task");

            var url = "http://www.google.com";
            string response = TryIt.TryTask(DownloadStringAsync, url, 5)
                .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                .Go();

            Console.WriteLine(" - Response length: {0}", response.Length);
        }

        static async Task Try_A_Task_Async()
        {
            Console.WriteLine();
            Console.Write("Try a task ASYNC");

            var url = "http://www.google.com";
            var response = await TryIt.TryTask(DownloadStringAsync, url, 3)
                .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                .GoAsync();

            Console.WriteLine(" - Response length: {0}", response.Length);

        }

        static void Try_A_Method_A_B()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.

            Console.WriteLine();
            Console.Write("Try and retry a method (A, B)");

            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.Try(DownloadString, urlA, 3)
               .UsingDelay(backoff)
               .ThenTry(urlB, 3)
               .Go();

            Console.WriteLine(" - Response length: {0}", response.Length);
        }

        static void Try_A_Method_A_With_ALT_Method()
        {
            Console.WriteLine();
            Console.WriteLine("Try and retry a method with an alternate method.");

            var url = "http://www.IdontExist.spoon";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.Try(DownloadString, url, 3)
               .UsingDelay(backoff)
               .ThenTry(GetDefaultContent, 1)
               .Go();

            Console.WriteLine("Response: \"{0}\"", response);
        }

        static string GetDefaultContent()
        {
            return "Alternate content returned.";
        }

        static void Try_A_Method_And_Handle_Error()
        {
            Console.WriteLine();
            Console.WriteLine("Try a method and Fail fast if there's a policy violation");

            var url = "http://www.IdontExist.spoon";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            try
            {
                string response = TryIt.Try(DownloadString, url, 4)
                    .WithErrorPolicy((ex, retries) =>
                    {
                        var policyEx = ex as WebException;
                        if (policyEx?.Status == WebExceptionStatus.NameResolutionFailure)
                            throw (ex);

                        return true;
                    })
                   .UsingDelay(backoff)
                   .Go();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Try_A_Method_A_B_Handle_Error()
        {
            Console.WriteLine();
            Console.WriteLine("Try and retry a method (A, B) Fail fast if there's a policy violation");

            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.Try(DownloadString, urlA, 3)
              .UsingDelay(backoff)
              .WithErrorPolicy((ex, retries) =>
              {
                  var policyEx = ex as WebException;
                  if (policyEx?.Status == WebExceptionStatus.NameResolutionFailure)
                      return false;

                  return true;
              })
             .ThenTry(urlB, 3)
             .WithErrorPolicy(null)
             .Go();

            Console.WriteLine("Response length: {0}", response.Length);
        }


        static string Try_A_Method_A_B_YOURE_DOING_IT_WRONG()
        {
            //This will cause url A to be tried back to back without a delay.
            //Url B will be tried with the delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.Try(DownloadString, urlA, 3)
                .ThenTry(urlB, 3)
                .UsingDelay(backoff)
                .Go();
            return response;
        }


        static async Task Try_A_Method_A_B_Async()
        {
            Console.WriteLine();
            Console.Write("Try and retry a method (A, B) ASYNC");

            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            var command = DownloadStringCommand(); // returns Func<string, string>
            string response = await command.Try(urlA, 5)
                .UsingDelay(backoff)
                .ThenTry(urlB, 5)
                .GoAsync();

            Console.WriteLine(" - Response length: {0}", response.Length);
        }



        static void Try_A_Task_A_B()
        {
            Console.WriteLine();
            Console.Write("Try and retry a task (A, B)");

            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.TryTask(DownloadStringAsync, urlA, 3)
              .UsingDelay(backoff)
                    .ThenTry(urlB, 3)
                    .Go();

            Console.WriteLine(" - Response length: {0}", response.Length);
        }


        static async Task Try_A_Task_A_B_Async()
        {
            Console.WriteLine();
            Console.Write("Try and retry a task (A, B) ASYNC");

            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = await TryIt.TryTask(DownloadStringAsync, urlA, 3)
                .UsingDelay(backoff)
                .ThenTry(urlB, 3)
                .GoAsync();

            Console.WriteLine(" - Response length: {0}", response.Length);
        }


        static void Try_Method_Quick_Then_BackOff()
        {
            Console.WriteLine();
            Console.WriteLine("Try/retry a method quickly then try/retry with a fibonacci delay (A, B)");

            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            try
            {
                //First try connA 3 times with no delay (default)...
                result = TryIt.Try(GetDBResults, connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6)
                      .UsingDelay(Delay.Fibonacci(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using the same backoff delay.
                    .ThenTry(connB, 6)

                    .Go();
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            Console.WriteLine("Response: \"{0}\"", result);
        }



        static async Task Try_Method_Quick_Then_BackOff_Async()
        {

            Console.WriteLine();
            Console.WriteLine("Try/retry a method quickly then try/retry with a backoff delay (A, B) ASYNC");

            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            try
            {
                //First try connA 3 times with no delay (default)...
                result = await TryIt.Try(GetDBResults, connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6)
                      .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using a backoff delay.
                    .ThenTry(connB, 6)

                    .GoAsync();
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            Console.WriteLine("Response: \"{0}\"", result);
        }



        static void Try_Task_Quick_Then_BackOff()
        {
            Console.WriteLine();
            Console.WriteLine("Try/retry a task quickly then try/retry with a backoff delay (A, B)");

            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            var start = DateTime.Now;
            try
            {
                //First try connA 3 times with no delay (default)...
                result = TryIt.TryTask(GetDBResultsAsync, connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6)
                      .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using the same backoff delay.
                    .ThenTry(connB, 6)

                    .Go();
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            var duration = DateTime.Now.Subtract(start);

            Console.WriteLine("Response: \"{0}\"", result);
            Console.WriteLine("Time to execute: {0}", duration);
        }

        static string Try_WithSuccess_Policy()
        {
            var url = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.Try(DownloadString, url, 3)
                .UsingDelay(backoff)
                .WithSuccessPolicy((result, trycount) =>
                {
                    if (string.IsNullOrEmpty(result))
                        throw new InvalidOperationException("Unacceptable results!");
                })
                .WithErrorPolicy((ex, tryCount) =>
                {
                    if (ex.GetType() == typeof(InvalidOperationException))
                        return false;
                    return true;
                })
                .Go();
            return response;

        }


        static async Task Try_Task_Quick_Then_BackOff_Async()
        {
            Console.WriteLine();
            Console.WriteLine("Try/retry a task quickly then try/retry with a backoff delay (A, B) ASYNC");

            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            try
            {
                //First try connA 3 times with no delay (default)...
                result = await TryIt.TryTask(GetDBResultsAsync, connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6)
                      .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using the same backoff delay.
                    .ThenTry(connB, 6)

                    .GoAsync();
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            Console.WriteLine("Response: \"{0}\"", result);
        }


        static void Try_a_Method_ABC_Alternating()
        {
            Console.WriteLine();
            Console.WriteLine("Alternate between 3 connections. Repeat 10 times - backing off after each try.");

            string connA = "Connection string A";
            string connB = "Connection string B";
            string connC = "Connection string C";
            string[] connStrings = { connA, connB, connC };

            string result = null;

            try
            {
                result = TryIt.TryTask(GetDBResultsAsyncRoundRobin, connStrings, 10)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))
                    .Go();
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            Console.WriteLine("Response: \"{0}\"", result);
        }


        static void Try_a_Method_ABC_Alternating_WithCancellationToken()
        {
            Console.WriteLine();
            Console.WriteLine("Alternate between 3 connections. Repeat 10 times - backing off after each try.");
            Console.WriteLine("Cancel the whole thing if processing takes more than 1/2 second.");

            string connA = "Connection string A";
            string connB = "Connection string B";
            string connC = "Connection string C";
            string[] connStrings = { connA, connB, connC };

            string result = null;

            try
            {
                using (var tokenSource = new CancellationTokenSource(500))
                {
                    var token = tokenSource.Token;
                    result = TryIt.TryTask(GetDBResultsAsyncRoundRobinCancelable, token, connStrings, 10)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))
                    .Go(token);
                }
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            Console.WriteLine("Response: \"{0}\"", result);
        }


        static Task<string> GetDBResultsAsyncRoundRobin(string[] connStrings)
        {
            FuncRetryBuilder<string> tryIt = null;
            foreach (var conn in connStrings)
            {
                if (tryIt == null)
                {
                    tryIt = TryIt.TryTask(GetDBResultsAsync, conn, 1);
                }
                else
                {
                    tryIt.ThenTry(conn, 1);
                }
            }
            return tryIt.GoAsync();
        }

        static Task<string> GetDBResultsAsyncRoundRobinCancelable(CancellationToken token, string[] connStrings)
        {
            FuncRetryBuilder<string> tryIt = null;
            foreach (var conn in connStrings)
            {
                if (tryIt == null)
                {
                    tryIt = TryIt.TryTask(GetDBResultsAsync, conn, 1);
                }
                else
                {
                    tryIt.ThenTry(conn, 1);
                }
            }
            return tryIt.GoAsync(token);
        }

        static void Build_Runners_Via_Code()
        {
            Console.WriteLine();
            Console.WriteLine("Programatically stand up and run a TryIt builder instance.");

            string[] connStrings = {
                "Connection string A",
                "Connection string B",
                "Connection string C",
                "Connection string D" };

            var delay = Delay.Backoff(TimeSpan.FromMilliseconds(100));

            int retries = 4;
            string result = null;

            FuncRetryBuilder<string> tryIt = null;
            foreach (var conn in connStrings)
            {
                if (tryIt == null)
                {
                    tryIt = TryIt.Try(GetDBResults, conn, retries);
                    tryIt.UsingDelay(delay);
                }
                else
                {
                    tryIt.ThenTry(conn, retries);
                }
            }

            try
            {
                result = tryIt.Go();
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            Console.WriteLine("Response: \"{0}\"", result);
        }

        static System.Random rnd = new Random();

        private static string GetDBResults(string connectionString)
        {
            const int n = 8;
            //One in n chance of succeeding (very flaky service :-) )
            Console.Write("Using connectionString: {0}", connectionString);

            var x = rnd.Next(n);
            bool shouldThrow = (x < n - 1);
            if (shouldThrow)
            {
                Console.WriteLine("...FAIL!");
                throw new Exception("I'll almost always throw!");
            }
            Console.WriteLine("...SUCCESS!");

            return string.Format("I did it with {0}!", connectionString);
        }

        private static async Task<string> GetDBResultsAsync(string connectionString)
        {
            return await Task<string>.Run(() => 
            {
                var d = rnd.Next(200);
                Thread.Sleep(d);
                return GetDBResults(connectionString);
            });
        }
    }
}
