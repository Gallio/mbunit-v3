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
using MbUnit.Core.Harness;
using MbUnit.Collections;
using MbUnit.Model;
using MbUnit.Plugin.MbUnit2Adapter.Properties;

namespace MbUnit.Plugin.MbUnit2Adapter.Model
{
    /// <summary>
    /// Builds a test object model based on reflection against MbUnit v2 framework attributes.
    /// </summary>
    public class MbUnit2TestFramework : ITestFramework
    {
        /// <inheritdoc />
        public string Name
        {
            get { return Resources.MbUnit2TestFramework_FrameworkName; }
        }

        /// <inheritdoc />
        public void Apply(ITestHarness harness)
        {
            harness.BuildingTemplates += harness_BuildingTemplates;
        }

        private static void harness_BuildingTemplates(ITestHarness harness, EventArgs e)
        {
            IMultiMap<AssemblyName, Assembly> map = ModelUtils.MapByAssemblyReference(harness.Assemblies, @"MbUnit.Framework");
            foreach (KeyValuePair<AssemblyName, IList<Assembly>> entry in map)
            {
                // Add a framework template with suitable rules to populate tests using the
                // MbUnit v2 test enumerator.  We don't actually represent each test as a
                // template because we can't perform any interesting meta-operations
                // on them like binding test parameters or composing tests.
                Version frameworkVersion = entry.Key.Version;
                BaseTemplate frameworkTemplate = new BaseTemplate(
                    String.Format(Resources.MbUnit2TestFramework_FrameworkTemplateName, frameworkVersion),
                    CodeReference.Unknown);
                frameworkTemplate.Kind = ComponentKind.Framework;
                frameworkTemplate.IsGenerator = true;
                harness.TemplateTreeBuilder.Root.AddChild(frameworkTemplate);

                foreach (Assembly assembly in entry.Value)
                {
                    MbUnit2AssemblyTemplate assemblyTemplate = new MbUnit2AssemblyTemplate(assembly);
                    frameworkTemplate.AddChild(assemblyTemplate);

                    // Add assembly-level metadata.
                    ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTemplate.Metadata);
                }
            }
        }
    }
}
