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
            Console.WriteLine("{0} ms ({1} iterations)", end.TotalMilliseconds, i);
        }

        static void RunTask(Task<string> task)
        {
            var start = DateTime.Now;
            var ticks = 0;
            while (task.GetAwaiter().IsCompleted == false)
            {
                //Console.WriteLine(task.Status);
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

        static string Try_A_Method()
        {
            //Try request.DownloadString(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";
            using (var request = new WebClient())
            {
                string response = TryIt.Try((u) => request.DownloadString(u), url, 3)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                    .Go();
                return response;
            }
        }

        static async Task<string> Try_A_Method_Async()
        {
            //Try request.DownloadString(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";
            using (var request = new WebClient())
            {
                 string response = await TryIt.Try((u) => request.DownloadString(u), url, 3)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                    .GoAsync();
                return response;
            }
        }


        static string Try_A_Task()
        {
            //Try request.DownloadStringTaskAsync(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";

            using (var request = new WebClient())
            {
                var response = TryIt.Try((u) => request.DownloadStringTaskAsync(u), url, 3)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                    .Go();
                return response;
            }

        }

        static async Task<string> Try_A_Task_Async()
        {
            //Try request.DownloadStringTaskAsync(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";

            using (var request = new WebClient())
            {
                var response = await TryIt.Try((u) => request.DownloadStringTaskAsync(u), url, 3)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                    .GoAsync();
                return response;
            }

        }

        static string Try_A_Method_A_B()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));

            using (var request = new WebClient())
            {
                Func<string, string> func  = request.DownloadString;
                string response = func.Try(urlA, 3)
                    .UsingDelay(backoff)
                    .ThenTry(urlB, 3).UsingDelay(backoff)
                    .Go();
                return response;
            }
        }


        static async Task<string> Try_A_Method_A_B_Async()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));

            using (var request = new WebClient())
            {
                Func<string, string> func = request.DownloadString;
                string response =  await func.Try(urlA, 3)
                   .UsingDelay(backoff)
                    .ThenTry(urlB, 3).UsingDelay(backoff)
                    .GoAsync();
                return response;
            }
        }



        static string Try_A_Task_A_B()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));

            using (var request = new WebClient())
            {
                string response = TryIt.Try((u) => request.DownloadStringTaskAsync(u), urlA, 3)
                    .UsingDelay(backoff)
                    .ThenTry(urlB, 3).UsingDelay(backoff)
                    .Go();
                return response;
            }
        }


        static async Task<string> Try_A_Task_A_B_Async()
        {
            //Try request.DownloadString(urlA) 3 times using a Backoff delay.
            //Then try request.DownloadString(urlB) 3 times using a Backoff delay.
            var urlA = "http://www.IdontExist.spoon";
            var urlB = "http://www.google.com";
            var backoff = Delay.Backoff(TimeSpan.FromMilliseconds(200));

            using (var request = new WebClient())
            {
                string response = await TryIt.Try((u) => request.DownloadStringTaskAsync(u), urlA, 3)
                    .UsingDelay(backoff)
                    .ThenTry(urlB, 3).UsingDelay(backoff)
                    .GoAsync();
                return response;
            }
        }


        static string Try_Method_Quick_Then_BackOff()
        {
            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            try
            {
                //First try connA 3 times with no delay (default)...
                result = TryIt.Try((c) => GetDBResults(c), connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using a backoff delay.
                    .ThenTry(connB, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    .Go();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
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
                result = await TryIt.Try((c) => GetDBResults(c), connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using a backoff delay.
                    .ThenTry(connB, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    .GoAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
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
                result = TryIt.Try((c) => GetDBResultsAsync(c), connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using a backoff delay.
                    .ThenTry(connB, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    .Go();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            var duration = DateTime.Now.Subtract(start);
            Console.WriteLine("Time to execute: {0}", duration);
            return result;
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
                result = await TryIt.Try((c) => GetDBResultsAsync(c), connA, 3)

                    //...then try connB 3 times with no delay (default)...
                    .ThenTry(connB, 3)

                    //...then try connA 6 times with using a back-off delay that starts at 100ms...
                    .ThenTry(connA, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    //...finaly try connB 6 times with using a backoff delay.
                    .ThenTry(connB, 6).UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(100)))

                    .GoAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }

            var duration = DateTime.Now.Subtract(start);
            Console.WriteLine("Time to execute: {0}", duration);
            return result;
        }


        static System.Random rnd = new Random();

        private static string GetDBResults(string connectionString)
        {
            const int n = 16;
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
