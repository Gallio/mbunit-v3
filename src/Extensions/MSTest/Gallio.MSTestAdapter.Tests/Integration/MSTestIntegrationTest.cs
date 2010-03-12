using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runner;
using Gallio.Tests;
using Gallio.Common.Platform;

namespace Gallio.MSTestAdapter.Tests.Integration
{
    public abstract class MSTestIntegrationTest : BaseTestWithSampleRunner
    {
        protected override void ConfigureRunner()
        {
            base.ConfigureRunner();

            // MSTest cannot run in a 64bit process.
            Runner.TestRunnerFactoryName = ProcessSupport.Is64BitProcess
                ? StandardTestRunnerFactoryNames.IsolatedProcess
                : StandardTestRunnerFactoryNames.IsolatedAppDomain;
        }
    }
}
