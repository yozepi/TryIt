using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yozepi.Retry.Delays
{
    /// <summary>
    ///  Implements the <see cref="IDelay"/> interface to create a delay that backs off using the Fibonacci sequence.
    /// </summary>
    public class FibonacciDelay : Delay
    {

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="initialDelay">A <see cref="TimeSpan"/> representing the initial delay to begin with.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided TimeSpan is less than zero.</exception>
        public FibonacciDelay(TimeSpan initialDelay)
        {
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("Value must be greater than a zero TimeSpan", initialDelay, "initialDelay");

            InitialDelay = initialDelay;
        }

        /// <summary>
        /// The <see cref="TimeSpan"/> of the initial delay.
        /// </summary>
        public TimeSpan InitialDelay { get; private set; }



        protected override async Task WaitAsync(int tryCount)
        {
            var ticks = InitialDelay.Ticks * CalcFibonacci(tryCount);
            await WaitAsync(TimeSpan.FromTicks(ticks));
        }

        const double GR = 1.61803398874989484820458683436;
        const double OneMinusGR = 1 - GR;
        static double FiveDiv = Math.Sqrt(5.0);
        private long CalcFibonacci(int tryCount)
        {

            var n = (double)tryCount;

            var result = (Math.Pow(GR, n) - Math.Pow(OneMinusGR, n)) / FiveDiv;
            return (long)result;

        }

    }
}
