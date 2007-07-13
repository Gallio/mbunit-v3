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
using System.Text;
using MbUnit.Framework.Model;
using MbUnit.Core.Serialization;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A local implementation of a test domain that performs all processing
    /// with the current app-domain including loading assemblies.
    /// </summary>
    public class LocalTestDomain : BaseTestDomain
    {
        private IAssemblyResolverManager resolverManager;
        private TestProject modelProject;

        /// <summary>
        /// Creates a local test domain using the specified resolver manager.
        /// </summary>
        /// <param name="resolverManager">The assembly resolver manager</param>
        public LocalTestDomain(IAssemblyResolverManager resolverManager)
        {
            this.resolverManager = resolverManager;
        }

        /// <inheritdoc />
        protected override void InternalDispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void InternalLoadProject(TestProjectInfo project)
        {
            modelProject = new TestProject();

            foreach (string path in project.HintDirectories)
                resolverManager.AddHintDirectory(path);

            foreach (string assemblyFile in project.AssemblyFiles)
                resolverManager.AddHintDirectoryContainingFile(assemblyFile);

            foreach (string assemblyFile in project.AssemblyFiles)
            {
                modelProject.Assemblies.Add(LoadTestAssembly(assemblyFile));
            }
        }

        /// <inheritdoc />
        protected override void InternalBuildTestTemplates()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void InternalBuildTests()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void InternalRunTests()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void InternalUnloadProject()
        {
            modelProject = null;
        }

        private Assembly LoadTestAssembly(string assemblyFile)
        {
            try
            {
                return Assembly.LoadFrom(assemblyFile);
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException(String.Format(CultureInfo.CurrentCulture,
                    "Could not load test assembly from '{0}'.", assemblyFile), ex);
            }
        }
    }
}
