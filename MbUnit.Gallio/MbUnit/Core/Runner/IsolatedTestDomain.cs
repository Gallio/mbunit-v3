using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// An isolated test domain loads test assemblies and runs tests within
    /// the context of a separate isolated <see cref="AppDomain" /> in the
    /// current process.  It uses .Net remoting for communication across
    /// <see cref="AppDomain" /> boundaries.  The <see cref="AppDomain" /> is
    /// created when a test project is loaded and is released on disposal.
    /// </summary>
    public class IsolatedTestDomain : ITestDomain
    {
        private AppDomain appDomain;

        /// <inheritdoc />
        public void Dispose()
        {
            if (appDomain != null)
            {
                AppDomain.Unload(appDomain);
                appDomain = null;
            }
        }

        /// <inheritdoc />
        public TestProjectInfo TestProject
        {
            get
            {
                ThrowIfDisposed();

                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <inheritdoc />
        public void LoadProject(TestProjectInfo project)
        {
            ThrowIfDisposed();

            throw new Exception("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public void UnloadProject()
        {
            ThrowIfDisposed();

            throw new Exception("The method or operation is not implemented.");
        }

        /// <inheritdoc />
        public TestTemplateInfo GetTestTemplateTreeRoot()
        {
            ThrowIfDisposed();

            throw new Exception("The method or operation is not implemented.");
        }

        private void ThrowIfDisposed()
        {
            if (appDomain == null)
                throw new ObjectDisposedException("Isolated test domain");
        }
    }
}
