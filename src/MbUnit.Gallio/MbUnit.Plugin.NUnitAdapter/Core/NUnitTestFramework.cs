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
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Plugin.NUnitAdapter.Core
{
    /// <summary>
    /// Builds a test object model based on reflection against NUnit framework attributes.
    /// </summary>
    public class NUnitTestFramework : ITestFramework
    {
        /// <inheritdoc />
        public string Name
        {
            get { return "NUnit"; }
        }

        /// <inheritdoc />
        public void Apply(ITestHarness harness)
        {
            harness.BuildingTemplates += harness_BuildingTemplates;
        }

        void harness_BuildingTemplates(ITestHarness harness, EventArgs e)
        {
            MultiMap<AssemblyName, Assembly> map = ReflectionUtils.GetReverseAssemblyReferenceMap(harness.Assemblies, "nunit.framework");
            foreach (KeyValuePair<AssemblyName, IList<Assembly>> entry in map)
            {
                // Add a framework template with suitable rules to populate tests using the
                // NUnit test enumerator.  We don't actually represent each test as a
                // template because we can't perform any interesting meta-operations
                // on them like binding test parameters or composing tests.
                Version frameworkVersion = entry.Key.Version;
                TemplateGroup frameworkTemplate = new TemplateGroup("NUnit v" + frameworkVersion, CodeReference.Unknown);
                frameworkTemplate.Kind = TemplateKind.Framework;
                harness.TemplateTreeBuilder.Root.AddChild(frameworkTemplate);

                foreach (Assembly assembly in entry.Value)
                {
                    TemplateGroup assemblyTemplate = new TemplateGroup(assembly.FullName, CodeReference.CreateFromAssembly(assembly));
                    assemblyTemplate.Kind = TemplateKind.Assembly;
                    frameworkTemplate.AddChild(assemblyTemplate);

                    // TODO: Add rules to populate the test graph using NUnit.
                }
            }
        }
    }
}
