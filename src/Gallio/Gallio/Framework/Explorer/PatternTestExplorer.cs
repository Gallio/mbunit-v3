// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Explorer;
using Gallio.Framework.Patterns;
using Gallio.Utilities;
using System.Text;

namespace Gallio.Framework.Explorer
{
    /// <summary>
    /// A test explorer for <see cref="PatternTestFramework" />.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestExplorer : BaseTestExplorer
    {
        private readonly IPatternTestModelBuilder builder;
        private readonly List<IAssemblyInfo> assemblies;
        private readonly Dictionary<string, IPatternTestBuilder> topLevelTestBuilders;

        /// <summary>
        /// Creates a test explorer.
        /// </summary>
        /// <param name="testModel">The test model</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> is null</exception>
        public PatternTestExplorer(TestModel testModel)
            : base(testModel)
        {
            builder = new DefaultPatternTestModelBuilder(testModel, DeclarativePatternResolver.Instance);
            assemblies = new List<IAssemblyInfo>();
            topLevelTestBuilders = new Dictionary<string, IPatternTestBuilder>();
        }

        /// <inheritdoc />
        public override bool IsTest(ICodeElementInfo element)
        {
            // FIXME: This check is not entirely accurate because it
            //        ignores any custom rules that might be defined elsewhere.
            return AttributeUtils.HasAttribute<PatternAttribute>(element, true);
        }

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
            BuildAssemblyTest(assembly);

            foreach (IPatternTestBuilder testBuilder in builder.GetTestBuilders(assembly))
            {
                testBuilder.Test.Populate(true);

                if (consumer != null)
                    consumer(testBuilder.Test);
            }
        }

        /// <inheritdoc />
        public override void ExploreType(ITypeInfo type, Action<ITest> consumer)
        {
            IAssemblyInfo assembly = type.Assembly;

            BuildAssemblyTest(assembly);

            foreach (IPatternTestBuilder testBuilder in builder.GetTestBuilders(assembly))
                testBuilder.Test.Populate(false);

            foreach (IPatternTestBuilder testBuilder in builder.GetTestBuilders(type))
            {
                testBuilder.Test.Populate(true);

                if (consumer != null)
                    consumer(testBuilder.Test);
            }
        }

        private void BuildAssemblyTest(IAssemblyInfo assembly)
        {
            if (assemblies.Contains(assembly))
                return;

            assemblies.Add(assembly);

            IList<ToolInfo> tools = GetReferencedToolsSortedById(assembly);
            if (tools.Count == 0)
                return;

            IPatternTestBuilder topLevelTestBuilder = GetTopLevelTestBuilder(tools);

            InitializeAssembly(topLevelTestBuilder, assembly);

            BootstrapAssemblyPattern.Instance.Consume(topLevelTestBuilder, assembly);
        }

        private IPatternTestBuilder GetTopLevelTestBuilder(IList<ToolInfo> tools)
        {
            string id = BuildTopLevelTestId(tools);

            IPatternTestBuilder topLevelTestBuilder;
            if (!topLevelTestBuilders.TryGetValue(id, out topLevelTestBuilder))
            {
                PatternTest topLevelTest = new PatternTest(BuildTopLevelTestName(tools), null);
                topLevelTest.Kind = TestKinds.Framework;
                topLevelTest.BaselineLocalId = id;

                topLevelTestBuilder = builder.AddTopLevelTest(topLevelTest);
                topLevelTestBuilders.Add(id, topLevelTestBuilder);
            }

            return topLevelTestBuilder;
        }

        private void InitializeAssembly(IPatternTestBuilder topLevelTestBuilder, IAssemblyInfo assembly)
        {
            foreach (AssemblyInitializationAttribute attrib in AttributeUtils.GetAttributes<AssemblyInitializationAttribute>(assembly, false))
            {
                try
                {
                    attrib.Initialize(topLevelTestBuilder, assembly);
                }
                catch (Exception ex)
                {
                    TestModel.RootTest.AddChild(new ErrorTest(assembly, String.Format("Error initializing assembly '{0}'", assembly.Name), ex));
                }
            }
        }

        private static IList<ToolInfo> GetReferencedToolsSortedById(IAssemblyInfo assembly)
        {
            List<ToolInfo> tools = new List<ToolInfo>();

            foreach (IPatternTestFrameworkExtension extension in Runtime.Instance.ResolveAll<IPatternTestFrameworkExtension>())
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

        private static string BuildTopLevelTestId(IList<ToolInfo> tools)
        {
            Hash64 hash = new Hash64();
            hash.Add("PatternTestFramework");

            foreach (ToolInfo tool in tools)
                hash = hash.Add(tool.Id);

            return hash.ToString();
        }

        private static string BuildTopLevelTestName(IList<ToolInfo> tools)
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