using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Gallio.Framework;
using Gallio.Runner;
using MbUnit.Framework;

namespace MbUnit.TestResources.ProcessorArchitecture
{
    /// <summary>
    /// Verifies that the test is being run using the right host process assuming it is
    /// being run using the <see cref="StandardTestRunnerFactoryNames.IsolatedProcess" /> test runner.
    /// </summary>
    public class MSILSample
    {
        [Test]
        public void VerifyHostProcessName()
        {
            Assert.Contains(Process.GetCurrentProcess().MainModule.FileName, "Gallio.Host.exe");
        }
    }
}
