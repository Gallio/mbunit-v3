using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// Default implementation of a test harness.
    /// </summary>
    public class DefaultTestHarness : ITestHarness
    {
        private bool isDisposed;
        private IRuntime runtime;
        private IList<Assembly> assemblies;
        private TemplateTreeBuilder templateTreeBuilder;

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
        public event TypedEventHandler<ITestHarness, EventArgs> Initialized;

        /// <inheritdoc />
        public event TypedEventHandler<ITestHarness, EventArgs> BuildingTemplates;

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
                assemblies.Add(assembly);
        }

        /// <inheritdoc />
        public Assembly LoadAssemblyFrom(string assemblyFile)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

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
        public void Initialize()
        {
            ThrowIfDisposed();

            if (Initialized != null)
                Initialized(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void BuildTemplates()
        {
            ThrowIfDisposed();

            templateTreeBuilder = new TemplateTreeBuilder(this);

            if (BuildingTemplates != null)
                BuildingTemplates(this, EventArgs.Empty);

            templateTreeBuilder.FinishBuilding();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!isDisposed)
            {
                if (Disposing != null)
                    Disposing(this, EventArgs.Empty);

                isDisposed = true;

                Initialized = null;
                BuildingTemplates = null;
                Disposing = null;

                templateTreeBuilder = null;
                assemblies = null;
                runtime = null;
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The test harness has been disposed.");
        }
    }
}
