using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MbUnit.Core.Harness;
using MbUnit.Core.Reporting;
using MbUnit.Core.Runner;
using MbUnit.Core.Runner.Monitors;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit._Framework.Tests.Integration
{
    /// <summary>
    /// Runs a list of sample test fixtures and provides the results back
    /// for verification.  The idea is to make it easier to write tests that
    /// verify the end-to-end behavior of the test runner.
    /// </summary>
    public class SampleRunner
    {
        private readonly TestPackage package;
        private readonly List<Filter<ITest>> filters;

        private Report report;

        public SampleRunner()
        {
            package = new TestPackage();
            filters = new List<Filter<ITest>>();
        }

        public Report Report
        {
            get { return report; }
        }

        public void AddFixture(Type fixtureType)
        {
            filters.Add(new TypeFilter<ITest>(fixtureType.AssemblyQualifiedName, false));

            string assemblyFile = fixtureType.Assembly.Location;
            if (!package.AssemblyFiles.Contains(assemblyFile))
                package.AssemblyFiles.Add(assemblyFile);
        }

        public void Run()
        {
            InitializeRuntimeHack();

            using (LocalRunner runner = new LocalRunner())
            {
                ReportMonitor reportMonitor = new ReportMonitor();
                reportMonitor.Attach(runner);
                report = reportMonitor.Report;

                DebugMonitor debugMonitor = new DebugMonitor(Console.Out);
                debugMonitor.Attach(runner);

                runner.LoadPackage(package, new NullProgressMonitor());
                runner.BuildTemplates(new NullProgressMonitor());
                runner.BuildTests(new NullProgressMonitor());

                runner.TestExecutionOptions.Filter = new OrFilter<ITest>(filters.ToArray());
                runner.Run(new NullProgressMonitor());
            }
        }

        /// <summary>
        /// Temporary hack until we start running these tests with Gallio.
        /// </summary>
        private static void InitializeRuntimeHack()
        {
            if (RuntimeHolder.Instance == null)
            {
                RuntimeSetup runtimeSetup = new RuntimeSetup();
                DefaultAssemblyResolverManager assemblyResolverManager = new DefaultAssemblyResolverManager();
                assemblyResolverManager.AddMbUnitDirectories();

                WindsorRuntime runtime = new WindsorRuntime(assemblyResolverManager, runtimeSetup);
                runtime.Initialize();

                RuntimeHolder.Instance = runtime;
            }
        }
    }
}
