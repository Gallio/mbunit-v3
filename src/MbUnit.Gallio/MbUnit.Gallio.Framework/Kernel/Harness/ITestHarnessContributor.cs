using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Harness
{
    /// <summary>
    /// <para>
    /// A test harness contributor extends the <see cref="ITestHarness" /> by
    /// attaching event handlers to modify its lifecycle.  A test harness contributor
    /// is generally a singleton component whereas a new test harness is created
    /// for each test project that is loaded.
    /// </para>
    /// <para>
    /// A new third party test framework may be supported by registering
    /// a suitable implementation of this interface that extends the test harness
    /// with code to enumerate and execute that framework's tests.
    /// </para>
    /// </summary>
    public interface ITestHarnessContributor
    {
        /// <summary>
        /// Applies the contributions of this test harness contributor to the
        /// specified test harness.  This method is called before the test
        /// harness <see cref="ITestHarness.Initialize" /> event is fired.
        /// </summary>
        /// <param name="harness">The test harness</param>
        void Apply(ITestHarness harness);
    }
}
