// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Utilities;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// Default implementation of a test harness.
    /// </summary>
    /// <remarks>
    /// The <see cref="Runtime" /> must be initialized prior to using this factory
    /// because the tests will run within the current <see cref="AppDomain" /> and
    /// <see cref="Runtime"/>.
    /// </remarks>
    public class DefaultTestHarness : ITestHarness
    {
        private ITestContextTracker contextTracker;

        private bool isDisposed;

        private List<ITestFramework> frameworks;
        private List<ITestEnvironment> environments;

        private TestPackage package;
        private TestModel model;

        /// <summary>
        /// Creates a test harness.
        /// </summary>
        /// <param name="contextTracker">The test context tracker</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextTracker"/> is null</exception>
        public DefaultTestHarness(ITestContextTracker contextTracker)
        {
            if (contextTracker == null)
                throw new ArgumentNullException("contextTracker");

            this.contextTracker = contextTracker;

            frameworks = new List<ITestFramework>();
            environments = new List<ITestEnvironment>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                package = null;
                model = null;
                contextTracker = null;
                frameworks = null;
                environments = null;
            }
        }

        /// <inheritdoc />
        public TestPackage TestPackage
        {
            get
            {
                ThrowIfDisposed();
                return package;
            }
        }

        /// <inheritdoc />
        public TestModel TestModel
        {
            get
            {
                ThrowIfDisposed();
                return model;
            }
        }

        /// <inheritdoc />
        public void AddTestFramework(ITestFramework framework)
        {
            if (framework == null)
                throw new ArgumentNullException(@"framework");

            ThrowIfDisposed();
            frameworks.Add(framework);
        }

        /// <inheritdoc />
        public void AddTestEnvironment(ITestEnvironment environment)
        {
            if (environment == null)
                throw new ArgumentNullException(@"environment");

            ThrowIfDisposed();
            environments.Add(environment);
        }

        /// <inheritdoc />
        public void LoadTestPackage(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (packageConfig == null)
                throw new ArgumentNullException("packageConfig");

            ThrowIfDisposed();

            if (package != null)
                throw new InvalidOperationException("A package has already been loaded.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading test package.", 10);
                progressMonitor.SetStatus("Performing pre-processing.");

                package = new TestPackage(packageConfig, Reflector.ReflectionPolicy);

                foreach (string path in packageConfig.HintDirectories)
                    Loader.AssemblyResolverManager.AddHintDirectory(path);

                foreach (string assemblyFile in packageConfig.AssemblyFiles)
                    Loader.AssemblyResolverManager.AddHintDirectory(FileUtils.GetFullPathOfParentDirectory(assemblyFile));

                progressMonitor.Worked(1);

                LoadAssemblies(progressMonitor.CreateSubProgressMonitor(8), packageConfig.AssemblyFiles);
            }
        }

        private void LoadAssemblies(IProgressMonitor progressMonitor, ICollection<string> assemblyFiles)
        {
            using (progressMonitor)
            {
                if (assemblyFiles.Count != 0)
                {
                    progressMonitor.BeginTask("Loading test assemblies.", assemblyFiles.Count);

                    foreach (string assemblyFile in package.Config.AssemblyFiles)
                    {
                        progressMonitor.SetStatus("Loading: " + assemblyFile + ".");

                        LoadAssemblyFrom(assemblyFile);

                        progressMonitor.Worked(1);
                    }
                }
            }
        }

        private void LoadAssemblyFrom(string assemblyFile)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException(@"assemblyFile");

            try
            {
                IAssemblyInfo assembly = Reflector.Wrap(Assembly.LoadFrom(assemblyFile));
                package.AddAssembly(assembly);
            }
            catch (Exception ex)
            {
                throw new RunnerException(String.Format(CultureInfo.CurrentCulture,
                    "Could not load test assembly from '{0}'.", assemblyFile), ex);
            }
        }

        /// <inheritdoc />
        public void BuildTestModel(TestEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (options == null)
                throw new ArgumentNullException(@"options");

            ThrowIfDisposed();

            if (package == null)
                throw new InvalidOperationException("No test package has been loaded.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Building test model.", 10);

                model = new TestModel(package);

                AggregateTestExplorer explorer = new AggregateTestExplorer();
                foreach (ITestFramework framework in frameworks)
                    explorer.AddTestExplorer(framework.CreateTestExplorer(model));

                foreach (IAssemblyInfo assembly in package.Assemblies)
                    explorer.ExploreAssembly(assembly, null);
            }
        }

        /// <inheritdoc />
        public void RunTests(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
        {
            if (options == null)
                throw new ArgumentNullException(@"options");
            if (listener == null)
                throw new ArgumentNullException("listener");
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            ThrowIfDisposed();

            if (model == null)
                throw new InvalidOperationException("The test model has not been built.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", 100);

                List<IDisposable> environmentStates = new List<IDisposable>();
                try
                {
                    progressMonitor.SetStatus("Setting up the test environment.");
                    foreach (ITestEnvironment environment in environments)
                        environmentStates.Add(environment.SetUp());
                    progressMonitor.Worked(5);

                    progressMonitor.SetStatus("Sorting tests.");
                    ObservableTestContextManager contextManager = new ObservableTestContextManager(contextTracker, listener);
                    ITestCommand rootTestCommand = TestCommandFactory.BuildCommands(model, options.Filter, contextManager);
                    progressMonitor.Worked(5);
                    progressMonitor.SetStatus(@"");

                    RunTestCommandsWithinANullContext(rootTestCommand, progressMonitor.CreateSubProgressMonitor(85));
                }
                finally
                {
                    progressMonitor.SetStatus("Tearing down the test environment.");
                    foreach (IDisposable environmentState in environmentStates)
                        environmentState.Dispose();
                    progressMonitor.Worked(5);
                    progressMonitor.SetStatus(@"");
                }
            }
        }

        private void RunTestCommandsWithinANullContext(ITestCommand rootTestCommand, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                if (rootTestCommand != null)
                {
                    using (contextTracker.EnterContext(null))
                    {
                        Func<ITestController> rootTestControllerFactory = rootTestCommand.Test.TestControllerFactory;

                        if (rootTestControllerFactory != null)
                        {
                            using (ITestController controller = rootTestControllerFactory())
                                controller.RunTests(progressMonitor, rootTestCommand, null);
                        }
                    }
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The test harness has been disposed.");
        }
    }
}
