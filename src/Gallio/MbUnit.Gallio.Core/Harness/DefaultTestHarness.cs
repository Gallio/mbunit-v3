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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// Default implementation of a test harness.
    /// </summary>
    public class DefaultTestHarness : ITestHarness
    {
        private bool isDisposed;
        private IRuntime runtime;
        private List<Assembly> assemblies;
        private TestPackage package;
        private TemplateTreeBuilder templateTreeBuilder;
        private TestTreeBuilder testTreeBuilder;
        private EventDispatcher eventDispatcher;

        /// <summary>
        /// Creates a test harness.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public DefaultTestHarness(IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.runtime = runtime;

            assemblies = new List<Assembly>();
            eventDispatcher = new EventDispatcher();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!isDisposed)
            {
                if (Disposing != null)
                    Disposing(this, EventArgs.Empty);

                isDisposed = true;

                PackageLoaded = null;
                BuildingTemplates = null;
                Disposing = null;

                templateTreeBuilder = null;
                testTreeBuilder = null;
                assemblies = null;
                runtime = null;
                eventDispatcher = null;
            }
        }

        /// <inheritdoc />
        public IRuntime Runtime
        {
            get
            {
                ThrowIfDisposed();
                return runtime;
            }
        }

        /// <inheritdoc />
        public IAssemblyResolverManager AssemblyResolverManager
        {
            get { return runtime.Resolve<IAssemblyResolverManager>(); }
        }

        /// <inheritdoc />
        public IList<Assembly> Assemblies
        {
            get
            {
                ThrowIfDisposed();
                return assemblies;
            }
        }

        /// <inheritdoc />
        public EventDispatcher EventDispatcher
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
        public event TypedEventHandler<ITestHarness, EventArgs> PackageLoading;

        /// <inheritdoc />
        public event TypedEventHandler<ITestHarness, EventArgs> PackageLoaded;

        /// <inheritdoc />
        public event TypedEventHandler<ITestHarness, AssemblyAddedEventArgs> AssemblyAdded;

        /// <inheritdoc />
        public event TypedEventHandler<ITestHarness, EventArgs> BuildingTemplates;

        /// <inheritdoc />
        public event TypedEventHandler<ITestHarness, EventArgs> BuildingTests;

        /// <inheritdoc />
        public event TypedEventHandler<ITestHarness, EventArgs> Disposing;

        /// <inheritdoc />
        public void AddContributor(ITestHarnessContributor contributor)
        {
            if (contributor == null)
                throw new ArgumentNullException("contributor");

            contributor.Apply(this);
        }

        /// <inheritdoc />
        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (!assemblies.Contains(assembly))
            {
                assemblies.Add(assembly);

                if (AssemblyAdded != null)
                    AssemblyAdded(this, new AssemblyAddedEventArgs(assembly));
            }
        }

        /// <inheritdoc />
        public Assembly LoadAssemblyFrom(string assemblyFile)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

            Status("Loading assembly: " + assemblyFile);

            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyFile);
                AddAssembly(assembly);
                return assembly;
            }
            catch (Exception ex)
            {
                throw new TestHarnessException(String.Format(CultureInfo.CurrentCulture,
                    "Could not load test assembly from '{0}'.", assemblyFile), ex);
            }
        }

        /// <inheritdoc />
        public void LoadPackage(TestPackage package, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (package == null)
                throw new ArgumentNullException("package");

            ThrowIfDisposed();

            if (this.package != null)
                throw new InvalidOperationException("A package has already been loaded.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading test package.", 10);

                progressMonitor.SetStatus("Performing pre-processing.");
                this.package = package;

                if (PackageLoading != null)
                    PackageLoading(this, EventArgs.Empty);

                foreach (string path in package.HintDirectories)
                    AssemblyResolverManager.AddHintDirectory(path);

                foreach (string assemblyFile in package.AssemblyFiles)
                    AssemblyResolverManager.AddHintDirectoryContainingFile(assemblyFile);

                progressMonitor.Worked(1);

                LoadAssemblies(new SubProgressMonitor(progressMonitor, 8), package.AssemblyFiles);

                progressMonitor.SetStatus("Performing post-processing.");

                if (PackageLoaded != null)
                    PackageLoaded(this, EventArgs.Empty);

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
                throw new ArgumentNullException("progressMonitor");
            if (options == null)
                throw new ArgumentNullException("options");

            ThrowIfDisposed();

            if (package == null)
                throw new InvalidOperationException("No package has been loaded.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Building test templates.", 10);

                testTreeBuilder = null;

                RootTemplate rootTemplate = new RootTemplate();
                templateTreeBuilder = new TemplateTreeBuilder(rootTemplate, options);

                if (BuildingTemplates != null)
                    BuildingTemplates(this, EventArgs.Empty);

                templateTreeBuilder.FinishBuilding();
            }
        }

        /// <inheritdoc />
        public void BuildTests(TestEnumerationOptions options, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (options == null)
                throw new ArgumentNullException("options");

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

                if (BuildingTests != null)
                    BuildingTests(this, EventArgs.Empty);

                testTreeBuilder.FinishBuilding();
            }
        }

        /// <inheritdoc />
        public void RunTests(TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (options == null)
                throw new ArgumentNullException("options");

            ThrowIfDisposed();

            if (testTreeBuilder == null)
                throw new InvalidOperationException("The test tree has not been built yet.");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", 100);

                progressMonitor.SetStatus("Sorting tests.");

                Dictionary<ITest, TestBatch> filteredTests = new Dictionary<ITest, TestBatch>();
                PopulateClosureOfFilteredTests(testTreeBuilder.Root, options.Filter, filteredTests, null);

                MultiMap<TestBatch, ITest> batches = new MultiMap<TestBatch, ITest>();
                foreach (ITest test in filteredTests.Keys)
                {
                    TestBatch batch = test.Batch;
                    if (batch != null)
                        batches.Add(batch, test);
                }

                progressMonitor.Worked(1);

                int testCount = 0;
                foreach (KeyValuePair<TestBatch, IList<ITest>> batch in batches)
                {
                    testCount += batch.Value.Count;
                }

                foreach (KeyValuePair<TestBatch, IList<ITest>> batch in batches)
                {
                    IList<ITest> tests = batch.Value;
                    ITestController controller = batch.Key.CreateController();

                    controller.Run(new SubProgressMonitor(progressMonitor, testCount == 0 ? double.NaN : tests.Count * 99.0 / testCount),
                        options, eventDispatcher, tests);
                }
            }
        }

        private static bool PopulateClosureOfFilteredTests(ITest test, Filter<ITest> filter, Dictionary<ITest, TestBatch> filteredTests, TestBatch parentBatch)
        {
            TestBatch batch = test.Batch;
            if (parentBatch != null && batch != parentBatch)
                throw new TestHarnessException("Detected nested test batches in the test tree!");

            bool childWasIncluded = false;
            foreach (ITest child in test.Children)
                if (PopulateClosureOfFilteredTests(child, filter, filteredTests, batch))
                    childWasIncluded = true;

            // Ensure that we include the parent test if one of its children was matched by the filter
            // and included in the list of filtered tests.
            if (childWasIncluded || filter.IsMatch(test))
            {
                filteredTests.Add(test, batch);
                return true;
            }

            return false;
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
