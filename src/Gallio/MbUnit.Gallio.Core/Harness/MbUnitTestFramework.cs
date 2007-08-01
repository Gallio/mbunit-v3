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
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Attributes;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// Provides support for the MbUnit Gallio test framework.
    /// </summary>
    public class MbUnitTestFramework : ITestFramework
    {
        /// <inheritdoc />
        public string Name
        {
            get { return "MbUnit Gallio"; }
        }

        /// <inheritdoc />
        public void Apply(ITestHarness harness)
        {
            harness.Initialized += harness_Initialized;
            harness.BuildingTemplates += harness_BuildingTemplates;
            harness.Disposing += harness_Disposing;
        }

        private void harness_Initialized(ITestHarness harness, EventArgs e)
        {
            foreach (Assembly assembly in harness.Assemblies)
            {
                foreach (AssemblyResolverAttribute resolverAttribute in
                    assembly.GetCustomAttributes(typeof(AssemblyResolverAttribute), false))
                {
                    Type type = resolverAttribute.AssemblyResolverType;
                    try
                    {
                        IAssemblyResolver resolver = (IAssemblyResolver)Activator.CreateInstance(type);
                        harness.AssemblyResolverManager.AddAssemblyResolver(resolver);
                    }
                    catch (Exception ex)
                    {
                        throw new ModelException(String.Format("Failed to create custom assembly resolver type '{0}'.", type), ex);
                    }
                }
            }
        }

        private void harness_BuildingTemplates(ITestHarness sender, EventArgs e)
        {
            MultiMap<Version, Assembly> map = ReflectionUtils.GetReverseAssemblyReferenceMap(sender.Assemblies, "MbUnit.Gallio.Framework");
            foreach (KeyValuePair<Version, IList<Assembly>> entry in map)
            {
                // Build templates for the contents of the assemblies that reference MbUnit Gallio
                // via reflection.  The attributes exercise a great deal of control over this
                // process so that it can be easily extended by users.
                Version frameworkVersion = entry.Key;
                MbUnitFrameworkTemplate frameworkTemplate = new MbUnitFrameworkTemplate(frameworkVersion);
                sender.TemplateTreeBuilder.Root.AddChild(frameworkTemplate);

                foreach (Assembly assembly in entry.Value)
                {
                    AssemblyPatternAttribute.ProcessAssembly(sender.TemplateTreeBuilder, frameworkTemplate, assembly);
                }
            }
        }

        private void harness_Disposing(ITestHarness sender, EventArgs e)
        {
        }
    }
}