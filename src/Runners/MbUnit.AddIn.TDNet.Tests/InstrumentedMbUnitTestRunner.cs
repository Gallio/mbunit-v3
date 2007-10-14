using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Runner;

namespace MbUnit.AddIn.TDNet.Tests
{
    /// <summary>
    /// Makes it possible to unit test the <see cref="MbUnitTestRunner" /> class.
    /// In particular we need to disable the initialization of a new runtime
    /// because it will conflict with the test execution environment.
    /// </summary>
    public class InstrumentedMbUnitTestRunner : MbUnitTestRunner
    {
        protected override TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            launcher.TestRunnerFactory = delegate { return new LocalTestRunner(); };
            return base.RunLauncher(launcher);
        }
    }
}
