using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Services.Reflection
{
    /// <summary>
    /// Provides information about a test.
    /// </summary>
    /// <remarks>
    /// TODO: This class is a stub.
    /// </remarks>
    public class TestInfo
    {
        /// <summary>
        /// Creates a TestInfo object representing the root of the test context tree.
        /// </summary>
        internal static TestInfo CreateRootTestInfo()
        {
            return new TestInfo();
        }
    }
}
