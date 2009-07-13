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

using System;
using System.Collections.Generic;
using Gallio.Common;
using Gallio.Model;
using Gallio.Common.Reflection;
using System.Text;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Explores tests implemented using the pattern test framework.
    /// </summary>
    internal class PatternTestExplorer : TestExplorer
    {
        private readonly PatternTestFrameworkExtensionProvider extensionProvider;
        private readonly Dictionary<IAssemblyInfo, bool> assemblies;
        private readonly Dictionary<string, IPatternScope> frameworkScopes;
        private ITestModelBuilder testModelBuilder;
        private IPatternEvaluator evaluator;

        public PatternTestExplorer(PatternTestFrameworkExtensionProvider extensionProvider)
        {
            this.extensionProvider = extensionProvider;

            assemblies = new Dictionary<IAssemblyInfo, bool>();
            frameworkScopes = new Dictionary<string, IPatternScope>();
        }

        protected override void ExploreImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            if (testModelBuilder == null)
            {
                testModelBuilder = new DefaultTestModelBuilder(reflectionPolicy, (PatternTestModel) TestModel);
                evaluator = new DefaultPatternEvaluator(testModelBuilder, DeclarativePatternResolver.Instance);
            }

            IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
            if (assembly != null)
            {
                bool skipChildren = ! (codeElement is IAssemblyInfo);
                if (BuildAssemblyTest(assembly, skipChildren))
                {
                    foreach (IPatternScope scope in evaluator.GetScopes(assembly))
                        scope.PopulateDeferredComponents(null);

                    assemblies[assembly] = true;
                }
                else
                {
                    ITypeInfo type = ReflectionUtils.GetType(codeElement);
                    if (type != null)
                    {
                        foreach (IPatternScope scope in evaluator.GetScopes(assembly))
                            scope.PopulateDeferredComponents(type);
                    }
                }
            }
        }

        public override void Finish()
        {
            testModelBuilder.ApplyDeferredActions();
        }

        protected override TestModel CreateTestModel()
        {
            return new PatternTestModel();
        }

        private bool BuildAssemblyTest(IAssemblyInfo assembly, bool skipChildren)
        {
            bool fullyPopulated;
            if (assemblies.TryGetValue(assembly, out fullyPopulated))
                return fullyPopulated;

            IList<PatternTestFrameworkExtensionInfo> extensions = extensionProvider(assembly);
            if (extensions.Count == 0)
                fullyPopulated = true;

            assemblies.Add(assembly, fullyPopulated);

            if (!fullyPopulated)
            {
                IPatternScope frameworkScope = BuildFrameworkTest(extensions);

                InitializeAssembly(frameworkScope, assembly);

                frameworkScope.Consume(assembly, skipChildren, TestAssemblyPatternAttribute.DefaultInstance);
            }

            return fullyPopulated;
        }

        private IPatternScope BuildFrameworkTest(IList<PatternTestFrameworkExtensionInfo> extensions)
        {
            string id = BuildFrameworkTestId(extensions);
            string frameworkKind = extensions.Count == 1 ? extensions[0].FrameworkKind : null;
            if (frameworkKind == null)
                frameworkKind = TestKinds.Framework;

            IPatternScope frameworkScope;
            if (!frameworkScopes.TryGetValue(id, out frameworkScope))
            {
                frameworkScope = evaluator.CreateTopLevelTestScope(BuildFrameworkTestName(extensions), null);
                frameworkScope.TestBuilder.Kind = frameworkKind;
                frameworkScope.TestBuilder.LocalIdHint = id;

                foreach (var tool in extensions)
                    frameworkScope.TestBuilder.AddMetadata(TestKinds.Framework, tool.Id);

                // Define the anonymous data source on the top-level test as a backstop
                // for data bindings without associated data sources.
                frameworkScope.TestDataContextBuilder.DefineDataSource("");

                frameworkScopes.Add(id, frameworkScope);
            }

            return frameworkScope;
        }

        private static void InitializeAssembly(IPatternScope frameworkScope, IAssemblyInfo assembly)
        {
            foreach (TestAssemblyInitializationAttribute attrib in AttributeUtils.GetAttributes<TestAssemblyInitializationAttribute>(assembly, false))
            {
                try
                {
                    attrib.Initialize(frameworkScope, assembly);
                }
                catch (Exception ex)
                {
                    frameworkScope.TestModelBuilder.PublishExceptionAsAnnotation(assembly, ex);
                }
            }
        }

        private static string BuildFrameworkTestId(IEnumerable<PatternTestFrameworkExtensionInfo> extensions)
        {
            Hash64 hash = new Hash64();
            hash.Add("PatternTestFramework");

            foreach (PatternTestFrameworkExtensionInfo extension in extensions)
                hash = hash.Add(extension.Id);

            return hash.ToString();
        }

        private static string BuildFrameworkTestName(IEnumerable<PatternTestFrameworkExtensionInfo> extensions)
        {
            StringBuilder name = new StringBuilder();

            foreach (PatternTestFrameworkExtensionInfo tool in extensions)
            {
                if (name.Length != 0)
                    name.Append(", ");
                name.Append(tool.Name);
            }

            return name.ToString();
        }
    }
}
