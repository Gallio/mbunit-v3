using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Serialization;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Base implementation of a test domain.
    /// </summary>
    /// <remarks>
    /// The base implementation inherits from <see cref="MarshalByRefObject" />
    /// so that test domain's services can be accessed remotely if needed.
    /// </remarks>
    public abstract class BaseTestDomain : LongLivingMarshalByRefObject, ITestDomain
    {
        private bool disposed;
        private TestProjectInfo testProject;
        private TestTemplateInfo testTemplateTreeRoot;
        private TestInfo testTreeRoot;

        /// <inheritdoc />
        public void Dispose()
        {
            if (!disposed)
            {
                InternalDispose();

                testProject = null;
                testTemplateTreeRoot = null;
                testTreeRoot = null;
                disposed = true;
            }
        }

        /// <inheritdoc />
        public virtual TestProjectInfo TestProject
        {
            get
            {
                ThrowIfDisposed();
                return testProject;
            }
            protected set
            {
                testProject = value;
            }
        }

        /// <inheritdoc />
        public virtual TestTemplateInfo TestTemplateTreeRoot
        {
            get
            {
                ThrowIfDisposed();
                return testTemplateTreeRoot;
            }
            protected set
            {
                testTemplateTreeRoot = value;
            }
        }

        /// <inheritdoc />
        public virtual TestInfo TestTreeRoot
        {
            get
            {
                ThrowIfDisposed();
                return testTreeRoot;
            }
            protected set
            {
                testTreeRoot = value;
            }
        }

        /// <inheritdoc />
        public void LoadProject(TestProjectInfo project)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            ThrowIfDisposed();

            InternalUnloadProject();

            try
            {
                this.testProject = project;
                InternalLoadProject(project);
            }
            catch (Exception)
            {
                UnloadProject();
                throw;
            }
        }

        /// <inheritdoc />
        public void BuildTestTemplates()
        {
            ThrowIfDisposed();
            InternalBuildTestTemplates();
        }

        /// <inheritdoc />
        public void BuildTests()
        {
            ThrowIfDisposed();
            InternalBuildTests();
        }

        /// <inheritdoc />
        public void UnloadProject()
        {
            ThrowIfDisposed();
            InternalUnloadProject();

            testProject = null;
            testTreeRoot = null;
            testTemplateTreeRoot = null;
        }

        /// <inheritdoc />
        public void RunTests()
        {
            ThrowIfDisposed();
            InternalRunTests();
        }

        /// <summary>
        /// Internal implementation of <see cref="Dispose" />.
        /// </summary>
        protected abstract void InternalDispose();

        /// <summary>
        /// Internal implementation of <see cref="LoadProject" />.
        /// </summary>
        /// <param name="project">The test project</param>
        protected abstract void InternalLoadProject(TestProjectInfo project);

        /// <summary>
        /// Internal implementation of <see cref="BuildTestTemplates" />.
        /// </summary>
        /// <returns>The root template</returns>
        protected abstract void InternalBuildTestTemplates();

        /// <summary>
        /// Internal implementation of <see cref="BuildTests" />.
        /// </summary>
        /// <returns>The root test</returns>
        protected abstract void InternalBuildTests();

        /// <summary>
        /// Internal implementation of <see cref="RunTests" />.
        /// </summary>
        protected abstract void InternalRunTests();

        /// <summary>
        /// Internal implementation of <see cref="UnloadProject" />.
        /// </summary>
        protected abstract void InternalUnloadProject();

        /// <summary>
        /// Throws <see cref="ObjectDisposedException"/> if the domain has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("Isolated test domain");
        }
    }
}
