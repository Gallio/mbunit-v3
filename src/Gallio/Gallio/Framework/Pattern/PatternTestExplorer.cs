// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
        private ITestModelBuilder testModelBuilder;
        private IPatternEvaluator evaluator;

        public PatternTestExplorer(PatternTestFrameworkExtensionProvider extensionProvider)
        {
            this.extensionProvider = extensionProvider;

            assemblies = new Dictionary<IAssemblyInfo, bool>();
        }

        private void InitializeExplorerIfNeeded(IReflectionPolicy reflectionPolicy)
        {
            if (testModelBuilder != null)
                return;

            testModelBuilder = new DefaultTestModelBuilder(reflectionPolicy, (PatternTestModel)TestModel);
            evaluator = new DefaultPatternEvaluator(testModelBuilder, DeclarativePatternResolver.Instance);
        }

        protected override void ExploreImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            InitializeExplorerIfNeeded(reflectionPolicy);

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
            PatternTestModel testModel = new PatternTestModel();
            return testModel;
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
                IPatternScope rootScope = evaluator.RootScope;

                InitializeAssembly(rootScope, assembly);

                rootScope.Consume(assembly, skipChildren, TestAssemblyPatternAttribute.DefaultInstance);

                foreach (PatternTest assemblyTest in evaluator.GetDeclaredTests(assembly))
                {
                    if (extensions.Count == 1 && assemblyTest.Kind == TestKinds.Assembly)
                        assemblyTest.Kind = extensions[0].AssemblyKind;

                    foreach (var extension in extensions)
                        assemblyTest.Metadata.Add(MetadataKeys.Framework, extension.Name);
                }
            }

            return fullyPopulated;
        }

        private static void InitializeAssembly(IPatternScope rootScope, IAssemblyInfo assembly)
        {
            foreach (TestAssemblyInitializationAttribute attrib in AttributeUtils.GetAttributes<TestAssemblyInitializationAttribute>(assembly, false))
            {
                try
                {
                    attrib.Initialize(rootScope, assembly);
                }
                catch (Exception ex)
                {
                    rootScope.TestModelBuilder.PublishExceptionAsAnnotation(assembly, ex);
                }
            }
        }
    }
}
