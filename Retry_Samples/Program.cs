using Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retry_Samples
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
            string response = null;

            response = Try_A_Method();
            Console.WriteLine("Try a method - Response length: {0}", response.Length);

            response = await Try_A_Method_Async();
            Console.WriteLine("Try a method ASYNC - Response length: {0}", response.Length);

            response = Try_A_Task();
            Console.WriteLine("Try a task - Response length: {0}", response.Length);

            response = await Try_A_Task_Async();
            Console.WriteLine("Try a task ASYNC - Response length: {0}", response.Length);

            response = Try_A_Method_A_B();
            Console.WriteLine("Try and retry a method (A, B) - Response length: {0}", response.Length);

            response = Try_A_Method_A_With_ALT_Method();
            Console.WriteLine("Try and retry a method with an alternate method.");
            Console.WriteLine("Response: \"{0}\"", response);

            Console.WriteLine("Try a method and Fail fast if there's a policy violation");
            Try_A_Method_And_Handle_Error();

            Console.WriteLine("Try and retry a method (A, B) Fail fast if there's a policy violation");
            response = Try_A_Method_A_B_Handle_Error();
            Console.WriteLine("Response length: {0}", response.Length);

            response = await Try_A_Method_A_B_Async();
            Console.WriteLine("Try and retry a method (A, B) ASYNC - Response length: {0}", response.Length);

            response = Try_A_Task_A_B();
            Console.WriteLine("Try and retry a task (A, B) - Response length: {0}", response.Length);

            response = await Try_A_Task_A_B_Async();
            Console.WriteLine("Try and retry a task (A, B) ASYNC - Response length: {0}", response.Length);



            Console.WriteLine();
            Console.WriteLine("Try/retry a method quickly then try/retry with a backoff delay (A, B)");
            response = Try_Method_Quick_Then_BackOff();
            Console.WriteLine("Response: \"{0}\"", response);

            Console.WriteLine();
            Console.WriteLine("Try/retry a method quickly then try/retry with a backoff delay (A, B) ASYNC");
            RunTask(Try_Method_Quick_Then_BackOff_Async());


            Console.WriteLine();
            Console.WriteLine("Try/retry a task quickly then try/retry with a backoff delay (A, B)");
            response = Try_Task_Quick_Then_BackOff();
            Console.WriteLine("Response: \"{0}\"", response);

            Console.WriteLine();
            Console.WriteLine("Try/retry a task quickly then try/retry with a backoff delay (A, B) ASYNC");
            RunTask(Try_Task_Quick_Then_BackOff_Async());

            Console.WriteLine();
            Console.WriteLine("Alternate between 2 connections. Repeat 10 times - backing off after each try.");
            response = Try_a_Method_AB_Alternating();
            Console.WriteLine("Response: \"{0}\"", response);

            Console.WriteLine();
            response = Capture_Exception();
            Console.WriteLine("Capturing a RetryFailedException: {0}", response);


            Console.WriteLine();
            Console.WriteLine("Preformance Comparison...");
            Action testAction = () => { };
            Console.Write("When calling the method directly: ");
            PerformanceTest(testAction);
            Console.Write("When calling via Try: ");
            PerformanceTest(TryIt.Try(testAction, 1).Go);
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

        static void RunTask(Task<string> task)
        {
            var start = DateTime.Now;
            var ticks = 0;
            while (!task.GetAwaiter().IsCompleted)
            {
                ticks++;
            }
            var sts = task.Status;
            var response = task.Result;
            Console.WriteLine("Response: \"{0}\"", response);
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

        static string Try_A_Method()
        {
            //Try request.DownloadString(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";
            string result = TryIt.Try(DownloadString, url, 5)
                .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                .Go();
            return result;
        }

        static string Try_A_Method_Quick_Then_Backoff()
        {
            //Try request.DownloadString(url) 3 quickly
            //then try 5 times using a Backoff delay.
            var url = "http://www.google.com";
            string result = TryIt.Try(DownloadString, url, 3)
                .ThenTry(5)
                .UsingDelay(Delay.Timed(TimeSpan.FromMilliseconds(200)))
                .Go();
            return result;
        }

        static async Task<string> Try_A_Method_Async()
        {
            //Try request.DownloadString(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";
            string response = await TryIt.Try(DownloadString, url, 3)
               .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
               .GoAsync();
            return response;
        }


        static string Capture_Exception()
        {
            try
            {
                TryIt.Try(MyFailingMethod, 5).Go();
            }
            catch (RetryFailedException ex)
            {

                return ex.Message;
            }
            return "I shouldn't get here.";
        }

        static void MyFailingMethod() { throw new Exception("I Failed"); }


        static string Try_A_Task()
        {
            //Try request.DownloadStringTaskAsync(url) 5 times using a Backoff delay.
            var url = "http://www.google.com";
            string response = TryIt.TryTask(DownloadStringAsync, url, 5)
                .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                .Go();
            return response;
        }

        static async Task<string> Try_A_Task_Async()
        {
            //Try request.DownloadStringTaskAsync(url) 5 times using a Backoff delay.
            var url = "http://www.google.com";
            var response = await TryIt.TryTask(DownloadStringAsync, url, 3)
                .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                .GoAsync();
            return response;

        }

        static string Try_A_Method_A_B()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.Try(DownloadString, urlA, 3)
               .UsingDelay(backoff)
               .ThenTry(urlB, 3)
               .Go();
            return response;
        }

        static string Try_A_Method_A_With_ALT_Method()
        {
            var url = "http://www.IdontExist.spoon";

            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.Try(DownloadString, url, 3)
               .UsingDelay(backoff)
               .ThenTry(GetDefaultContent, 1)
               .Go();
            return response;
        }

        static string GetDefaultContent()
        {
            return "Default content returned.";
        }
        static void Try_A_Method_And_Handle_Error()
        {
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

        static string Try_A_Method_A_B_Handle_Error()
        {
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
            return response;
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


        static async Task<string> Try_A_Method_A_B_Async()
        {
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
            return response;
        }



        static string Try_A_Task_A_B()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = TryIt.TryTask(DownloadStringAsync, urlA, 3)
              .UsingDelay(backoff)
                    .ThenTry(urlB, 3)
                    .Go();
            return response;
        }


        static async Task<string> Try_A_Task_A_B_Async()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));
            string response = await TryIt.TryTask(DownloadStringAsync, urlA, 3)
                .UsingDelay(backoff)
                .ThenTry(urlB, 3)
                .GoAsync();
            return response;
        }


        static string Try_Method_Quick_Then_BackOff()
        {
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
                      .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using the same backoff delay.
                    .ThenTry(connB, 6)

                    .Go();
                return result;
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }
            return result;
        }



        static async Task<string> Try_Method_Quick_Then_BackOff_Async()
        {

            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            var start = DateTime.Now;
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

            var duration = DateTime.Now.Subtract(start);
            Console.WriteLine("Time to execute: {0}", duration);
            return result;
        }



        static string Try_Task_Quick_Then_BackOff()
        {
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
                return result;
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            var duration = DateTime.Now.Subtract(start);
            Console.WriteLine("Time to execute: {0}", duration);
            return result;
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


        static async Task<string> Try_Task_Quick_Then_BackOff_Async()
        {
            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            var start = DateTime.Now;
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
                return result;
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }

            var duration = DateTime.Now.Subtract(start);
            Console.WriteLine("Time to execute: {0}", duration);
            return result;
        }


        static string Try_a_Method_AB_Alternating()
        {
            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;

            Func<string> alternateConnections = () =>
                {
                    var localResult = TryIt.Try(GetDBResults, connA, 1)
                    .ThenTry(connB, 1).Go();
                    return localResult;
                };
            try
            {
                result = alternateConnections.Try(10)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))
                    .Go();
            }
            catch (Exception ex)
            {
                result = string.Format("Exception: {0}", ex.Message);
            }
            return result;
        }


        static System.Random rnd = new Random();

        private static string GetDBResults(string connectionString)
        {
            const int n = 4;
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

            return "I did it!";
        }


        private static async Task<string> GetDBResultsAsync(string connectionString)
        {
            return await Task<string>.Factory.StartNew(() => GetDBResults(connectionString));
        }
    }
}
