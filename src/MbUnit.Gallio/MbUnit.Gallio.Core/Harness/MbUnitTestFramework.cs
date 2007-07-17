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
using System.Reflection;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Attributes;
using MbUnit.Framework.Services.Runtime;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// Builds a test object model based on reflection against MbUnit framework attributes.
    /// </summary>
    public class MbUnitTestFramework : ITestFramework
    {
        private IAssemblyResolverManager assemblyResolverManager;

        /// <summary>
        /// Initializes the test framework.
        /// </summary>
        /// <param name="assemblyResolverManager">The assembly resolver manager</param>
        public MbUnitTestFramework(IAssemblyResolverManager assemblyResolverManager)
        {
            this.assemblyResolverManager = assemblyResolverManager;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return "MbUnit Gallio"; }
        }

        /// <inheritdoc />
        public void BuildTemplates(TestTemplateTreeBuilder builder, ITestTemplate parent)
        {
            MultiMap<AssemblyName, Assembly> map = ReflectionUtils.GetReverseAssemblyReferenceMap(builder.Project.Assemblies, "MbUnit.Gallio.Framework");
            foreach (KeyValuePair<AssemblyName, IList<Assembly>> entry in map)
            {
                // Build templates for the contents of the assemblies that reference MbUnit Gallio
                // via reflection.  The attributes exercise a great deal of control over this
                // process so that it can be easily extended by users.
                Version frameworkVersion = entry.Key.Version;
                MbUnitTestFrameworkTemplate frameworkTemplate = new MbUnitTestFrameworkTemplate(frameworkVersion);
                parent.AddChild(frameworkTemplate);

                foreach (Assembly assembly in entry.Value)
                {
                    AssemblyPatternAttribute.ProcessAssembly(builder, frameworkTemplate, assembly);
                }
            }
        }

        /// <inheritdoc />
        public void InitializeTestAssembly(Assembly assembly)
        {
            foreach (AssemblyResolverAttribute resolverAttribute in
                assembly.GetCustomAttributes(typeof(AssemblyResolverAttribute), false))
            {
                Type type = resolverAttribute.AssemblyResolverType;
                try
                {
                    IAssemblyResolver resolver = (IAssemblyResolver) Activator.CreateInstance(type);
                    assemblyResolverManager.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
                    {
                        return resolver.Resolve(e.Name);
                    };
                }
                catch (Exception ex)
                {
                    throw new ModelException(String.Format("Failed to create custom assembly resolver type '{0}'.", type), ex);
                }
            }
        }
    }
}