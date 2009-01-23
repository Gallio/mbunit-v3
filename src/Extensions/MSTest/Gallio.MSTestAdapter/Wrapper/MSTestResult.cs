using System;
using Gallio.Model;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Describes the result of an MSTest test.
    /// </summary>
    internal sealed class MSTestResult
    {
        public string Guid { get; set; }
        public TimeSpan? Duration { get; set; }
        public TestOutcome Outcome { get; set; }
        public string StdOut { get; set; }
        public string Errors { get; set; }
    }
}