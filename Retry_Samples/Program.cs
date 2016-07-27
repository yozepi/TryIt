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
            string response = null;

            response = SimpleTry();
            Console.WriteLine("SimpleTry() Response length: {0}", response.Length);

            response = SimpleTryAsync().Result;
            Console.WriteLine("SimpleTryAsync() Response length: {0}", response.Length);

            response = TryA_B();
            Console.WriteLine("TryA_B() Response length: {0}", response.Length);

            response = TryA_B_Async().Result;
            Console.WriteLine("TryA_B_Async() Response length: {0}", response.Length);



            Console.WriteLine();
            Console.WriteLine("Quick_Then_BackOff_Try()");
            response = Try_Quick_Then_BackOff();
            Console.WriteLine("Response: \"{0}\"", response);


            Console.WriteLine();
            Console.WriteLine("Quick_Then_BackOff_Try_Async()");
            var ticks = 0;
            var task = Try_Quick_Then_BackOff_Async();
            var sts = task.Status;
            while (task.GetAwaiter().IsCompleted == false)
            {
                //Console.WriteLine(task.Status);
                ticks++;
                sts = task.Status;
            }
            response = task.Result;
            Console.WriteLine("Response: \"{0}\"", response);
            Console.WriteLine();
            Console.WriteLine("Exit status = {0}", sts);
            Console.WriteLine("{0} ticks while executing task", ticks);
            //Thread.Sleep(7000);


        }


        static string SimpleTry()
        {
            //Try request.DownloadString(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";
            using (var request = new WebClient())
            {
                string response = TryIt.Try(request.DownloadString, url, 3)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                    .Go();
                return response;
            }
        }

        static async Task<string> SimpleTryAsync()
        {
            //Try request.DownloadString(url) 3 times using a Backoff delay.
            var url = "http://www.google.com";
            using (var request = new WebClient())
            {
                 string response = await TryIt.Try(request.DownloadString, url, 3)
                    .UsingDelay(Delay.Backoff(TimeSpan.FromMilliseconds(200)))
                    .GoAsync();
                return response;
            }
        }



        static string TryA_B()
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


        static async Task<string> TryA_B_Async()
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


        static string Try_Quick_Then_BackOff()
        {
            string connA = "Connection string A";
            string connB = "Connection string B";

            string result = null;
            var start = DateTime.Now;
            try
            {
                //First try connA 3 times with no delay (default)...
                result = TryIt.Try(GetDBResults, connA, 3)

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



        static async Task<string> Try_Quick_Then_BackOff_Async()
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


        static System.Random rnd = new Random();

        private static string GetDBResults(string connectionString)
        {
            const int n = 12;
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
    }
}
