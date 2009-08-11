// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Explores tests in MbUnit v2 assemblies.
    /// </summary>
    internal class MbUnit2TestExplorer : TestExplorer
    {
        internal const string AssemblyKind = "MbUnit v2 Assembly";

        private readonly Dictionary<IAssemblyInfo, MbUnit2TestExplorerEngine> assemblyTestExplorerEngines;
        private readonly List<KeyValuePair<Test, string>> unresolvedDependencies;

        public MbUnit2TestExplorer()
        {
            assemblyTestExplorerEngines = new Dictionary<IAssemblyInfo, MbUnit2TestExplorerEngine>();
            unresolvedDependencies = new List<KeyValuePair<Test, string>>();
        }

        protected override void ExploreImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
            if (assembly == null)
                return;

            try
            {
                MbUnit2TestExplorerEngine engine;
                if (! assemblyTestExplorerEngines.TryGetValue(assembly, out engine))
                {
                    Assembly loadedAssembly = assembly.Resolve(false);

                    if (Reflector.IsUnresolved(loadedAssembly))
                        engine = new MbUnit2ReflectiveTestExplorerEngine(TestModel, assembly);
                    else
                        engine = new MbUnit2NativeTestExplorerEngine(TestModel, loadedAssembly);

                    assemblyTestExplorerEngines.Add(assembly, engine);

                    bool skipChildren = !(codeElement is IAssemblyInfo);
                    engine.ExploreAssembly(skipChildren, unresolvedDependencies);

                    for (int i = 0; i < unresolvedDependencies.Count; i++)
                    {
                        foreach (var entry in assemblyTestExplorerEngines)
                        {
                            if (entry.Key.FullName == unresolvedDependencies[i].Value)
                            {
                                unresolvedDependencies[i].Key.AddDependency(entry.Value.GetAssemblyTest());
                                unresolvedDependencies.RemoveAt(i--);
                                break;
                            }
                        }
                    }
                }

                ITypeInfo type = ReflectionUtils.GetType(codeElement);
                if (type != null)
                {
                    engine.ExploreType(type);
                }
            }
            catch (Exception ex)
            {
                TestModel.AddAnnotation(new Annotation(AnnotationType.Error, assembly,
                    "An exception was thrown while exploring an MbUnit v2 test assembly.", ex));
            }
        }
    }
}
