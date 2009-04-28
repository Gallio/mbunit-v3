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
using Gallio.Runtime;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Utilities;
using System.Text;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Explores tests implemented using the pattern test framework.
    /// </summary>
    internal class PatternTestExplorer : BaseTestExplorer
    {
        private readonly List<ExtensionProvider> extensionProviders = new List<ExtensionProvider>();

        public delegate IEnumerable<PatternTestFrameworkExtensionInfo> ExtensionProvider(IAssemblyInfo assembly);

        public void RegisterExtensionProvider(ExtensionProvider extensionProvider)
        {
            extensionProviders.Add(extensionProvider);
        }

        public override bool IsTest(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            var evaluator = CreateReflectionOnlyPatternEvaluator(reflectionPolicy);
            return evaluator.IsTest(element, GetAutomaticPattern(codeElement));
        }

        public override bool IsTestPart(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            var evaluator = CreateReflectionOnlyPatternEvaluator(reflectionPolicy);
            return evaluator.IsTestPart(element, GetAutomaticPattern(codeElement));
        }

        public override void Explore(TestModel testModel, TestSource testSource, Action<ITest> consumer)
        {
            var state = new ExplorerState(testModel, extensionProviders);

            foreach (IAssemblyInfo assembly in testSource.Assemblies)
                state.ExploreAssembly(assembly, consumer);

            foreach (ITypeInfo type in testSource.Types)
                state.ExploreType(type, consumer);

            state.FinishModel();
        }

        private static IPatternEvaluator CreateReflectionOnlyPatternEvaluator(IReflectionPolicy reflectionPolicy)
        {
            var testModelBuilder = new ReflectionOnlyTestModelBuilder(reflectionPolicy);
            return new DefaultPatternEvaluator(testModelBuilder, DeclarativePatternResolver.Instance);
        }

        private static IPattern GetAutomaticPattern(ICodeElementInfo element)
        {
            switch (element.Kind)
            {
                case CodeElementKind.Type:
                    return TestTypePatternAttribute.AutomaticInstance;

                case CodeElementKind.Field:
                case CodeElementKind.Property:
                    return TestParameterPatternAttribute.AutomaticInstance;

                case CodeElementKind.Assembly:
                case CodeElementKind.Constructor:
                case CodeElementKind.Parameter:
                case CodeElementKind.GenericParameter:
                case CodeElementKind.Namespace:
                case CodeElementKind.Event:
                case CodeElementKind.Method:
                    return null;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private sealed class ExplorerState
        {
            private readonly TestModel testModel;
            private readonly List<ExtensionProvider> extensionProviders =
                new List<ExtensionProvider>();
            private readonly ITestModelBuilder testModelBuilder;
            private readonly IPatternEvaluator evaluator;
            private readonly Dictionary<IAssemblyInfo, bool> assemblies;
            private readonly Dictionary<string, IPatternScope> frameworkScopes;

            public ExplorerState(TestModel testModel, List<ExtensionProvider> extensionProviders)
            {
                this.testModel = testModel;
                this.extensionProviders = extensionProviders;
                testModelBuilder = new DefaultTestModelBuilder(testModel);
                evaluator = new DefaultPatternEvaluator(testModelBuilder, DeclarativePatternResolver.Instance);
                assemblies = new Dictionary<IAssemblyInfo, bool>();
                frameworkScopes = new Dictionary<string, IPatternScope>();
            }

            public void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
            {
                if (BuildAssemblyTest(assembly, false))
                {
                    foreach (IPatternScope scope in evaluator.GetScopes(assembly))
                        scope.PopulateDeferredComponents(null);

                    assemblies[assembly] = true;
                }

                if (consumer != null)
                {
                    foreach (PatternTest test in evaluator.GetDeclaredTests(assembly))
                        consumer(test);
                }
            }

            public void ExploreType(ITypeInfo type, Action<ITest> consumer)
            {
                IAssemblyInfo assembly = type.Assembly;

                if (! BuildAssemblyTest(assembly, true))
                {
                    foreach (IPatternScope scope in evaluator.GetScopes(assembly))
                        scope.PopulateDeferredComponents(type);
                }
                
                if (consumer != null)
                {
                    foreach (PatternTest test in evaluator.GetDeclaredTests(type))
                        consumer(test);
                }
            }

            public void FinishModel()
            {
                testModelBuilder.ApplyDeferredActions();
            }

            private bool BuildAssemblyTest(IAssemblyInfo assembly, bool skipChildren)
            {
                bool fullyPopulated;
                if (assemblies.TryGetValue(assembly, out fullyPopulated))
                    return fullyPopulated;

                IList<PatternTestFrameworkExtensionInfo> tools = GetReferencedExtensionsSortedById(assembly);
                if (tools.Count == 0)
                    fullyPopulated = true;

                assemblies.Add(assembly, fullyPopulated);

                if (!fullyPopulated)
                {
                    IPatternScope frameworkScope = BuildFrameworkTest(tools);

                    InitializeAssembly(frameworkScope, assembly);

                    frameworkScope.Consume(assembly, skipChildren, TestAssemblyPatternAttribute.DefaultInstance);
                }

                return fullyPopulated;
            }

            private IPatternScope BuildFrameworkTest(IList<PatternTestFrameworkExtensionInfo> tools)
            {
                string id = BuildFrameworkTestId(tools);

                IPatternScope frameworkScope;
                if (!frameworkScopes.TryGetValue(id, out frameworkScope))
                {
                    frameworkScope = evaluator.CreateTopLevelTestScope(BuildFrameworkTestName(tools), null);
                    frameworkScope.TestBuilder.Kind = TestKinds.Framework;
                    frameworkScope.TestBuilder.LocalIdHint = id;

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

            private IList<PatternTestFrameworkExtensionInfo> GetReferencedExtensionsSortedById(IAssemblyInfo assembly)
            {
                List<PatternTestFrameworkExtensionInfo> extensions = new List<PatternTestFrameworkExtensionInfo>();

                foreach (ExtensionProvider extensionProvider in extensionProviders)
                {
                    try
                    {
                        extensions.AddRange(extensionProvider(assembly));
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("A pattern test framework extension threw an exception while enumerating referenced extensions.", ex);
                    }
                }

                extensions.Sort(delegate(PatternTestFrameworkExtensionInfo a, PatternTestFrameworkExtensionInfo b)
                {
                    return a.Id.CompareTo(b.Id);
                });

                return extensions;
            }

            private static string BuildFrameworkTestId(IList<PatternTestFrameworkExtensionInfo> extensions)
            {
                Hash64 hash = new Hash64();
                hash.Add("PatternTestFramework");

                foreach (PatternTestFrameworkExtensionInfo extension in extensions)
                    hash = hash.Add(extension.Id);

                return hash.ToString();
            }

            private static string BuildFrameworkTestName(IList<PatternTestFrameworkExtensionInfo> extensions)
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
}
