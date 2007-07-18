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
        private TestProject testProject;
        private TemplateInfo templateTreeRoot;
        private TestInfo testTreeRoot;

        /// <inheritdoc />
        public void Dispose()
        {
            if (!disposed)
            {
                InternalDispose();

                testProject = null;
                templateTreeRoot = null;
                testTreeRoot = null;
                disposed = true;
            }
        }

        /// <inheritdoc />
        public virtual TestProject TestProject
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
        public virtual TemplateInfo TemplateTreeRoot
        {
            get
            {
                ThrowIfDisposed();
                return templateTreeRoot;
            }
            protected set
            {
                templateTreeRoot = value;
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
        public void LoadProject(TestProject project)
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
        public void BuildTemplates()
        {
            ThrowIfDisposed();
            InternalBuildTemplates();
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
            templateTreeRoot = null;
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
        protected abstract void InternalLoadProject(TestProject project);

        /// <summary>
        /// Internal implementation of <see cref="BuildTemplates" />.
        /// </summary>
        /// <returns>The root template</returns>
        protected abstract void InternalBuildTemplates();

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
