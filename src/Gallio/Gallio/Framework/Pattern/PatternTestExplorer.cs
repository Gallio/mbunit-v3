// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Utilities;
using System.Text;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test explorer for <see cref="PatternTestFramework" />.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestExplorer : BaseTestExplorer
    {
        private readonly IPatternTestFrameworkExtension[] extensions;

        private readonly PatternEvaluator evaluator;
        private readonly Dictionary<IAssemblyInfo, bool> assemblies;
        private readonly Dictionary<string, PatternEvaluationScope> frameworkScopes;

        /// <summary>
        /// Creates a test explorer.
        /// </summary>
        /// <param name="testModel">The test model</param>
        /// <param name="extensions">The test framework extensions</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> or
        /// <paramref name="extensions" /> is null</exception>
        public PatternTestExplorer(TestModel testModel, IPatternTestFrameworkExtension[] extensions)
            : base(testModel)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            this.extensions = extensions;

            evaluator = new PatternEvaluator(testModel, DeclarativePatternResolver.Instance);
            assemblies = new Dictionary<IAssemblyInfo, bool>();
            frameworkScopes = new Dictionary<string, PatternEvaluationScope>();
        }

        /// <inheritdoc />
        public override bool IsTest(ICodeElementInfo element)
        {
            return BootstrapTestAssemblyPattern.Instance.IsTest(evaluator, element);
        }

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
            if (BuildAssemblyTest(assembly, false))
            {
                foreach (PatternEvaluationScope scope in evaluator.GetScopes(assembly))
                    scope.PopulateChildrenChain.Action(null);

                assemblies[assembly] = true;
            }

            if (consumer != null)
            {
                foreach (PatternEvaluationScope scope in evaluator.GetScopes(assembly))
                    if (scope.IsTestDeclaration)
                        consumer(scope.Test);
            }
        }

        /// <inheritdoc />
        public override void ExploreType(ITypeInfo type, Action<ITest> consumer)
        {
            IAssemblyInfo assembly = type.Assembly;

            if (! BuildAssemblyTest(assembly, true))
            {
                foreach (PatternEvaluationScope scope in evaluator.GetScopes(assembly))
                    scope.PopulateChildrenChain.Action(type);
            }
            
            if (consumer != null)
            {
                foreach (PatternEvaluationScope scope in evaluator.GetScopes(type))
                    if (scope.IsTestDeclaration)
                        consumer(scope.Test);
            }
        }

        /// <inheritdoc />
        public override void FinishModel()
        {
            evaluator.FinishModel();
        }

        private bool BuildAssemblyTest(IAssemblyInfo assembly, bool skipChildren)
        {
            bool fullyPopulated;
            if (assemblies.TryGetValue(assembly, out fullyPopulated))
                return fullyPopulated;

            IList<ToolInfo> tools = GetReferencedToolsSortedById(assembly);
            if (tools.Count == 0)
                fullyPopulated = true;

            assemblies.Add(assembly, fullyPopulated);

            if (!fullyPopulated)
            {
                PatternEvaluationScope frameworkScope = BuildFrameworkTest(tools);

                InitializeAssembly(frameworkScope, assembly);

                BootstrapTestAssemblyPattern.Instance.Consume(frameworkScope, assembly, skipChildren);
            }

            return fullyPopulated;
        }

        private PatternEvaluationScope BuildFrameworkTest(IList<ToolInfo> tools)
        {
            string id = BuildFrameworkTestId(tools);

            PatternEvaluationScope frameworkScope;
            if (!frameworkScopes.TryGetValue(id, out frameworkScope))
            {
                PatternTest topLevelTest = new PatternTest(BuildFrameworkTestName(tools), null, new PatternTestDataContext(null));
                topLevelTest.Kind = TestKinds.Framework;
                topLevelTest.BaselineLocalId = id;

                frameworkScope = evaluator.AddTest(topLevelTest);
                frameworkScopes.Add(id, frameworkScope);
            }

            return frameworkScope;
        }

        private static void InitializeAssembly(PatternEvaluationScope frameworkScope, IAssemblyInfo assembly)
        {
            foreach (TestAssemblyInitializationAttribute attrib in AttributeUtils.GetAttributes<TestAssemblyInitializationAttribute>(assembly, false))
            {
                try
                {
                    attrib.Initialize(frameworkScope, assembly);
                }
                catch (Exception ex)
                {
                    frameworkScope.Evaluator.PublishExceptionAsAnnotation(assembly, ex);
                }
            }
        }

        private IList<ToolInfo> GetReferencedToolsSortedById(IAssemblyInfo assembly)
        {
            List<ToolInfo> tools = new List<ToolInfo>();

            foreach (IPatternTestFrameworkExtension extension in extensions)
            {
                try
                {
                    tools.AddRange(extension.GetReferencedTools(assembly));
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("A pattern test framework extension threw an exception while enumerating referenced tools.", ex);
                }
            }

            tools.Sort(delegate(ToolInfo a, ToolInfo b)
            {
                return a.Id.CompareTo(b.Id);
            });

            return tools;
        }

        private static string BuildFrameworkTestId(IList<ToolInfo> tools)
        {
            Hash64 hash = new Hash64();
            hash.Add("PatternTestFramework");

            foreach (ToolInfo tool in tools)
                hash = hash.Add(tool.Id);

            return hash.ToString();
        }

        private static string BuildFrameworkTestName(IList<ToolInfo> tools)
        {
            StringBuilder name = new StringBuilder();

            foreach (ToolInfo tool in tools)
            {
                if (name.Length != 0)
                    name.Append(", ");
                name.Append(tool.Name);
            }

            return name.ToString();
        }
    }
}