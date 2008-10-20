using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Collections;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Runner;
using Gallio.Runner.Drivers;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runner.Drivers
{
    [TestsOn(typeof(HostedTestDriver))]
    public class HostedTestDriverTest
    {
        [Test]
        [Row(ProcessorArchitecture.MSIL, new[] { "MbUnit.TestResources.dll" })]
        [Row(ProcessorArchitecture.X86, new[] { "MbUnit.TestResources.x86.dll" })]
        [Row(ProcessorArchitecture.Amd64, new[] { "MbUnit.TestResources.x64.dll" })]
        [Row(ProcessorArchitecture.X86, new[] { "MbUnit.TestResources.x86.dll", "MbUnit.TestResources.dll"})]
        [Row(ProcessorArchitecture.Amd64, new[] { "MbUnit.TestResources.x64.dll", "MbUnit.TestResources.dll" })]
        public void AutomaticallyChoosesAppropriateProcessorArchitecture(
            ProcessorArchitecture expectedProcessorArchitecture, string[] testAssemblies)
        {
            StubbedHostedTestDriver driver = new StubbedHostedTestDriver();
            driver.Initialize(RuntimeAccessor.Instance.GetRuntimeSetup(), new TestLogStreamLogger(TestLog.Default));

            TestPackageConfig testPackageConfig = new TestPackageConfig();
            testPackageConfig.AssemblyFiles.AddRange(testAssemblies);

            Assert.Throws<HostException>(() => driver.Load(testPackageConfig, NullProgressMonitor.CreateInstance()));

            Assert.AreEqual(expectedProcessorArchitecture, driver.ProcessorArchitecture);
        }

        [Test]
        public void ThrowsRunnerExceptionIfAssembliesHaveIncompatibleProcessorArchitectures()
        {
            StubbedHostedTestDriver driver = new StubbedHostedTestDriver();
            driver.Initialize(RuntimeAccessor.Instance.GetRuntimeSetup(), new TestLogStreamLogger(TestLog.Default));

            TestPackageConfig testPackageConfig = new TestPackageConfig();
            testPackageConfig.AssemblyFiles.Add("MbUnit.TestResources.x86.dll");
            testPackageConfig.AssemblyFiles.Add("MbUnit.TestResources.x64.dll");

            RunnerException ex = Assert.Throws<RunnerException>(() => driver.Load(testPackageConfig, NullProgressMonitor.CreateInstance()));
            Assert.Contains(ex.Message, "Cannot run all test assemblies together");
        }

        private sealed class StubbedHostedTestDriver : HostedTestDriver
        {
            public ProcessorArchitecture ProcessorArchitecture = ProcessorArchitecture.None;

            public StubbedHostedTestDriver()
                : base(MockRepository.GenerateStub<IHostFactory>(),
                EmptyArray<ITestFramework>.Instance,
                RuntimeAccessor.Instance)
            {
            }

            protected override void ConfigureHost(HostSetup hostSetup)
            {
                ProcessorArchitecture = hostSetup.ProcessorArchitecture;
                throw new HostException("Abort");
            }
        }
    }
}
