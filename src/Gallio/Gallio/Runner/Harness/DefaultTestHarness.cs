// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Threading;
using Gallio.Concurrency;
using Gallio.Runtime.Loader;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
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
    /// The <see cref="RuntimeAccessor" /> must be initialized prior to using this factory
    /// because the tests will run within the current <see cref="AppDomain" /> and
    /// <see cref="RuntimeAccessor"/>.
    /// </remarks>
    public class DefaultTestHarness : ITestHarness
    {
        private readonly ITestContextTracker contextTracker;
        private readonly ILoader loader;

        private bool isDisposed;

        private List<ITestFramework> frameworks;
        private List<ITestEnvironment> environments;

        private string workingDirectory;
        private TestPackage package;
        private TestModel model;

        /// <summary>
        /// Creates a test harness.
        /// </summary>
        /// <param name="contextTracker">The test context tracker</param>
        /// <param name="loader">The loader</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextTracker"/>
        /// or <paramref name="loader "/> is null</exception>
        public DefaultTestHarness(ITestContextTracker contextTracker, ILoader loader)
        {
            if (contextTracker == null)
                throw new ArgumentNullException("contextTracker");
            if (loader == null)
                throw new ArgumentNullException("loader");

            this.contextTracker = contextTracker;
            this.loader = loader;

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
        public void Load(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
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

                workingDirectory = packageConfig.HostSetup.WorkingDirectory;

                using (SwitchWorkingDirectory())
                {
                    package = new TestPackage(packageConfig, Reflector.NativeReflectionPolicy, loader);

                    foreach (string path in packageConfig.HintDirectories)
                        loader.AssemblyResolverManager.AddHintDirectory(path);

                    foreach (string assemblyFile in packageConfig.AssemblyFiles)
                        loader.AssemblyResolverManager.AddHintDirectory(FileUtils.GetFullPathOfParentDirectory(assemblyFile));

                    progressMonitor.Worked(1);

                    LoadAssemblies(progressMonitor.CreateSubProgressMonitor(8), packageConfig.AssemblyFiles);
                }
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
                IAssemblyInfo assembly = Reflector.Wrap(loader.LoadAssemblyFrom(assemblyFile));
                package.AddAssembly(assembly);
            }
            catch (Exception ex)
            {
                throw new RunnerException(String.Format(CultureInfo.CurrentCulture,
                    "Could not load test assembly from '{0}'.", assemblyFile), ex);
            }
        }

        /// <inheritdoc />
        public void Explore(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            if (options == null)
                throw new ArgumentNullException(@"options");
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            ThrowIfDisposed();

            if (package == null)
                throw new InvalidOperationException("No test package has been loaded.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Building test model.", 10);

                using (SwitchWorkingDirectory())
                {
                    model = new TestModel(package);

                    AggregateTestExplorer explorer = new AggregateTestExplorer(model);
                    foreach (ITestFramework framework in frameworks)
                        explorer.AddTestExplorer(framework.CreateTestExplorer(model));

                    foreach (IAssemblyInfo assembly in package.Assemblies)
                        explorer.ExploreAssembly(assembly, null);

                    explorer.FinishModel();
                }
            }
        }

        /// <inheritdoc />
        public void Run(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
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

                using (SwitchWorkingDirectory())
                {
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

                        RunAllTestCommands(rootTestCommand, options, progressMonitor.CreateSubProgressMonitor(85));
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
        }

        private void RunAllTestCommands(ITestCommand rootTestCommand, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            ThreadTask task = new ThreadTask("Test Runner", delegate
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
                                    controller.RunTests(rootTestCommand, null, options, progressMonitor);
                            }
                        }
                    }
                }
            });

            // Use STA as the default for all tests.  A test framework may of course choose
            // to create its own threads with different apartment states.
            task.ApartmentState = ApartmentState.STA;
            task.Run(null);

            if (task.Result.Exception != null)
                throw new RunnerException("A fatal exception occurred while running all test commands.",
                    task.Result.Exception);
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The test harness has been disposed.");
        }

        /// <inheritdoc />
        public void Unload(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            ThrowIfDisposed();

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Unloading tests.", 1);

                workingDirectory = null;
                package = null;
                model = null;
            }
        }

        private CurrentDirectorySwitcher SwitchWorkingDirectory()
        {
            if (String.IsNullOrEmpty(workingDirectory))
                return null;
            return new CurrentDirectorySwitcher(workingDirectory);
        }
    }
}
