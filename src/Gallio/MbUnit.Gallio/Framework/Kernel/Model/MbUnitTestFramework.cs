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
using MbUnit.Collections;
using MbUnit.Core.Harness;
using MbUnit.Framework.Kernel.Attributes;
using MbUnit.Model;

namespace MbUnit.Framework.Kernel.Model
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
            harness.AssemblyAdded += harness_AssemblyAdded;
            harness.BuildingTemplates += harness_BuildingTemplates;
            harness.Disposing += harness_Disposing;
        }

        private void harness_AssemblyAdded(ITestHarness harness, AssemblyAddedEventArgs e)
        {
            foreach (AssemblyResolverAttribute resolverAttribute in
                e.Assembly.GetCustomAttributes(typeof(AssemblyResolverAttribute), false))
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

        private void harness_BuildingTemplates(ITestHarness sender, EventArgs e)
        {
            IMultiMap<AssemblyName, Assembly> map = ModelUtils.MapByAssemblyReference(sender.Assemblies, @"MbUnit.Gallio");
            foreach (KeyValuePair<AssemblyName, IList<Assembly>> entry in map)
            {
                // Build templates for the contents of the assemblies that reference MbUnit Gallio
                // via reflection.  The attributes exercise a great deal of control over this
                // process so that it can be easily extended by users.
                Version frameworkVersion = entry.Key.Version;
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