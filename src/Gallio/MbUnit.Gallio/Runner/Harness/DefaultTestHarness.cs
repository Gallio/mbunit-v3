// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Collections;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Hosting;
using MbUnit.Model;
using MbUnit.Model.Execution;
using MbUnit.Utilities;

namespace MbUnit.Runner.Harness
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
        private ITestPlanFactory testPlanFactory;

        private bool isDisposed;
        private List<Assembly> assemblies;
        private TestPackage package;
        private TemplateTreeBuilder templateTreeBuilder;
        private TestTreeBuilder testTreeBuilder;
        private TestEventDispatcher eventDispatcher;

        private List<ITestFramework> frameworks;
        private List<ITestEnvironment> environments;

        /// <summary>
        /// Creates a test harness.
        /// </summary>
        /// <param name="testPlanFactory">The test plan factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPlanFactory"/> is null</exception>
        public DefaultTestHarness(ITestPlanFactory testPlanFactory)
        {
            if (testPlanFactory == null)
                throw new ArgumentNullException(@"testPlanFactory");

            this.testPlanFactory = testPlanFactory;

            assemblies = new List<Assembly>();
            eventDispatcher = new TestEventDispatcher();
            frameworks = new List<ITestFramework>();
            environments = new List<ITestEnvironment>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                templateTreeBuilder = null;
                testTreeBuilder = null;
                assemblies = null;
                testPlanFactory = null;
                eventDispatcher = null;
                frameworks = null;
                environments = null;
            }
        }

        /// <inheritdoc />
        public IList<Assembly> Assemblies
        {
            get
            {
                ThrowIfDisposed();
                return GenericUtils.ToArray(assemblies);
            }
        }

        /// <inheritdoc />
        public TestEventDispatcher EventDispatcher
        {
            get { return eventDispatcher; }
        }

        /// <inheritdoc />
        public TestPackage Package
        {
            get { return package; }
        }

        /// <inheritdoc />
        public TemplateTreeBuilder TemplateTreeBuilder
        {
            get
            {
                ThrowIfDisposed();

                if (templateTreeBuilder == null)
                    throw new InvalidOperationException("Templates have not been built yet.");
                return templateTreeBuilder;
            }
        }

        /// <inheritdoc />
        public TestTreeBuilder TestTreeBuilder
        {
            get
            {
                ThrowIfDisposed();

                if (testTreeBuilder == null)
                    throw new InvalidOperationException("Tests have not been built yet.");
                return testTreeBuilder;
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
        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(@"assembly");

            if (!assemblies.Contains(assembly))
                assemblies.Add(assembly);
        }

        /// <inheritdoc />
        public Assembly LoadAssemblyFrom(string assemblyFile)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException(@"assemblyFile");

            Status("Loading assembly: " + assemblyFile);

            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyFile);
                AddAssembly(assembly);
                return assembly;
            }
            catch (Exception ex)
            {
                throw new RunnerException(String.Format(CultureInfo.CurrentCulture,
                    "Could not load test assembly from '{0}'.", assemblyFile), ex);
            }
        }

        /// <inheritdoc />
        public void LoadPackage(TestPackage package, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (package == null)
                throw new ArgumentNullException(@"package");

            ThrowIfDisposed();

            if (this.package != null)
                throw new InvalidOperationException("A package has already been loaded.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading test package.", 10);

                progressMonitor.SetStatus("Performing pre-processing.");
                this.package = package;

                foreach (string path in package.HintDirectories)
                    Loader.AssemblyResolverManager.AddHintDirectory(path);

                foreach (string assemblyFile in package.AssemblyFiles)
                    Loader.AssemblyResolverManager.AddHintDirectory(FileUtils.GetFullPathOfParentDirectory(assemblyFile));

                progressMonitor.Worked(1);

                LoadAssemblies(new SubProgressMonitor(progressMonitor, 8), package.AssemblyFiles);

                progressMonitor.SetStatus("Performing post-processing.");

                progressMonitor.Worked(1);
            }
        }

        private void LoadAssemblies(IProgressMonitor progressMonitor, IList<string> assemblyFiles)
        {
            using (progressMonitor)
            {
                if (assemblyFiles.Count != 0)
                {
                    progressMonitor.BeginTask("Loading test assemblies.", assemblyFiles.Count);

                    foreach (string assemblyFile in package.AssemblyFiles)
                    {
                        progressMonitor.SetStatus("Loading: " + assemblyFile + ".");

                        LoadAssemblyFrom(assemblyFile);

                        progressMonitor.Worked(1);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void BuildTemplates(TemplateEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (options == null)
                throw new ArgumentNullException(@"options");

            ThrowIfDisposed();

            if (package == null)
                throw new InvalidOperationException("No package has been loaded.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Building test templates.", 10);

                testTreeBuilder = null;

                RootTemplate rootTemplate = new RootTemplate();
                templateTreeBuilder = new TemplateTreeBuilder(rootTemplate, options);

                foreach (ITestFramework framework in frameworks)
                    framework.PrepareAssemblies(assemblies);
                foreach (ITestFramework framework in frameworks)
                    framework.BuildTemplates(templateTreeBuilder, assemblies);

                templateTreeBuilder.FinishBuilding();
            }
        }

        /// <inheritdoc />
        public void BuildTests(TestEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (options == null)
                throw new ArgumentNullException(@"options");

            ThrowIfDisposed();

            if (templateTreeBuilder == null)
                throw new InvalidOperationException("The template tree has not been built yet.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Building tests.", 10);

                TemplateBindingScope scope = new TemplateBindingScope(null);
                List<ITemplateBinding> rootBindings = new List<ITemplateBinding>(scope.Bind(templateTreeBuilder.Root));

                if (rootBindings.Count != 1)
                    throw new InvalidOperationException("The root template did not yield exactly one root template binding when it was bound.");

                ITemplateBinding rootBinding = rootBindings[0];
                RootTest rootTest = new RootTest(rootBinding);
                testTreeBuilder = new TestTreeBuilder(rootTest, options);

                rootBinding.BuildTests(testTreeBuilder, rootTest);

                testTreeBuilder.FinishBuilding();
            }
        }

        /// <inheritdoc />
        public void RunTests(TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (options == null)
                throw new ArgumentNullException(@"options");

            ThrowIfDisposed();

            if (testTreeBuilder == null)
                throw new InvalidOperationException("The test tree has not been built yet.");

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
                    progressMonitor.SetStatus(@"");

                    ITestPlan plan = testPlanFactory.CreateTestPlan(eventDispatcher);
                    try
                    {
                        plan.ScheduleTests(new SubProgressMonitor(progressMonitor, 5), testTreeBuilder.Root, options);
                        plan.RunTests(new SubProgressMonitor(progressMonitor, 80));
                    }
                    finally
                    {
                        plan.CleanUpTests(new SubProgressMonitor(progressMonitor, 5));
                    }
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

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The test harness has been disposed.");
        }

        private void Status(string message)
        {
            eventDispatcher.NotifyMessageEvent(new MessageEventArgs(MessageType.Status, message));
        }
    }
}
