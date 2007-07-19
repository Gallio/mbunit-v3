using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A test domain factory.
    /// </summary>
    public interface ITestDomainFactory
    {
        /// <summary>
        /// Creates a test domain.
        /// </summary>
        /// <returns>The test domain</returns>
        ITestDomain CreateDomain();
    }
}
