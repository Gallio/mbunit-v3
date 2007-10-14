using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Runner;

namespace MbUnit.Tasks.MSBuild.Tests
{
    /// <summary>
    /// Makes it possible to unit test the <see cref="MbUnit" /> task.
    /// In particular we need to disable the initialization of a new runtime
    /// because it will conflict with the test execution environment.
    /// </summary>
    public class InstrumentedMbUnitTask : MbUnit
    {
        protected override TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            launcher.TestRunnerFactory = delegate { return new LocalTestRunner(); };
            return base.RunLauncher(launcher);
        }
    }
}
