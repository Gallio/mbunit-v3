using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Echo
{
    /// <summary>
    /// Describes the result codes used by the application.
    /// </summary>
    public static class ResultCode
    {
        /// <summary>
        /// The tests ran successfully or there were no tests to run.
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// Some tests failed.
        /// </summary>
        public const int Failure = 1;

        /// <summary>
        /// A fatal runtime exception occurred.
        /// </summary>
        public const int FatalException = 2;

        /// <summary>
        /// Invalid arguments were supplied on the command-line.
        /// </summary>
        public const int InvalidArguments = 10;
    }
}
