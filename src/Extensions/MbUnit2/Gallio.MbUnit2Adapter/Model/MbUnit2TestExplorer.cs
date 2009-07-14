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
using Gallio.MbUnit2Adapter.Properties;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using TestFixturePatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestFixturePatternAttribute;
using TestPatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestPatternAttribute;
using FixtureCategoryAttribute2 = MbUnit2::MbUnit.Framework.FixtureCategoryAttribute;
using TestCategoryAttribute2 = MbUnit2::MbUnit.Framework.TestCategoryAttribute;
using TestImportance2 = MbUnit2::MbUnit.Framework.TestImportance;
using AuthorAttribute2 = MbUnit2::MbUnit.Framework.AuthorAttribute;
using TestsOnAttribute2 = MbUnit2::MbUnit.Framework.TestsOnAttribute;
using ImportanceAttribute2 = MbUnit2::MbUnit.Framework.ImportanceAttribute;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Explores tests in MbUnit v2 assemblies.
    /// </summary>
    internal class MbUnit2TestExplorer : TestExplorer
    {
        internal const string AssemblyKind = "MbUnit v2 Assembly";
        private const string MbUnitFrameworkAssemblyDisplayName = @"MbUnit.Framework";

        private readonly Dictionary<IAssemblyInfo, Test> assemblyTests;
        private readonly List<KeyValuePair<Test, string>> unresolvedDependencies;

        public MbUnit2TestExplorer()
        {
            assemblyTests = new Dictionary<IAssemblyInfo, Test>();
            unresolvedDependencies = new List<KeyValuePair<Test, string>>();
        }

        protected override void ExploreImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
            if (assembly != null)
            {
                Version frameworkVersion = GetFrameworkVersion(assembly);

                if (frameworkVersion != null)
                {
                    // TODO: Optimize me to only populate what's strictly required when code element is a type.
                    GetAssemblyTest(assembly, TestModel.RootTest, frameworkVersion);
                }
            }
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MbUnitFrameworkAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private Test GetAssemblyTest(IAssemblyInfo assembly, Test parentTest, Version frameworkVersion)
        {
            Test assemblyTest;
            if (assemblyTests.TryGetValue(assembly, out assemblyTest))
                return assemblyTest;

            try
            {
                Assembly loadedAssembly = assembly.Resolve(false);

                if (loadedAssembly != null)
                    assemblyTest = MbUnit2NativeTestExplorer.BuildAssemblyTest(loadedAssembly, unresolvedDependencies);
                else
                    assemblyTest = MbUnit2ReflectiveTestExplorer.BuildAssemblyTest(TestModel, assembly, unresolvedDependencies);

                string frameworkName = String.Format(Resources.MbUnit2TestExplorer_FrameworkNameWithVersionFormat, frameworkVersion);
                assemblyTest.Metadata.SetValue(MetadataKeys.Framework, frameworkName);
                assemblyTest.Metadata.SetValue(MetadataKeys.File, assembly.Path);
                assemblyTest.Kind = AssemblyKind;
            }
            catch (Exception ex)
            {
                TestModel.AddAnnotation(new Annotation(AnnotationType.Error, assembly,
                    "An exception was thrown while exploring an MbUnit v2 test assembly.", ex));
                return null;
            }

            for (int i = 0; i < unresolvedDependencies.Count; i++)
            {
                foreach (KeyValuePair<IAssemblyInfo, Test> entry in assemblyTests)
                {
                    if (entry.Key.FullName == unresolvedDependencies[i].Value)
                    {
                        unresolvedDependencies[i].Key.AddDependency(entry.Value);
                        unresolvedDependencies.RemoveAt(i--);
                        break;
                    }
                }
            }

            parentTest.AddChild(assemblyTest);

            assemblyTests.Add(assembly, assemblyTest);
            return assemblyTest;
        }
    }
}
