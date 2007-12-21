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
using Gallio.Model;
using Gallio.Model.Reflection;
using MbUnit.Model.Builder;
using MbUnit.Model.Patterns;

namespace MbUnit.Model
{
    /// <summary>
    /// A test explorer for MbUnit.
    /// </summary>
    public class MbUnitTestExplorer : BaseTestExplorer
    {
        private const string MbUnitAssemblyDisplayName = @"MbUnit";

        private readonly ITestModelBuilder builder;
        private readonly List<IAssemblyInfo> assemblies;

        /// <summary>
        /// Creates a test explorer.
        /// </summary>
        /// <param name="testModel">The test model</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testModel"/> is null</exception>
        public MbUnitTestExplorer(TestModel testModel)
            : base(testModel)
        {
            builder = new DefaultTestModelBuilder(testModel, DeclarativePatternResolver.Instance);
            assemblies = new List<IAssemblyInfo>();
        }

        /// <inheritdoc />
        public override bool IsTest(ICodeElementInfo element)
        {
            // FIXME: This check is not entirely accurate because it
            //        ignores any custom rules that might be defined elsewhere.
            return element.HasAttribute(typeof(PatternAttribute), true);
        }

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
            BuildAssemblyTest(assembly);

            foreach (ITestBuilder testBuilder in builder.GetTestBuilders(assembly))
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

            foreach (ITestBuilder testBuilder in builder.GetTestBuilders(assembly))
                testBuilder.Test.Populate(false);

            foreach (ITestBuilder testBuilder in builder.GetTestBuilders(type))
            {
                testBuilder.Test.Populate(true);

                if (consumer != null)
                    consumer(testBuilder.Test);
            }
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MbUnitAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private void BuildAssemblyTest(IAssemblyInfo assembly)
        {
            if (assemblies.Contains(assembly))
                return;

            assemblies.Add(assembly);

            Version frameworkVersion = GetFrameworkVersion(assembly);
            if (frameworkVersion == null)
                return;

            ITestBuilder frameworkTestBuilder = builder.GetFrameworkTestBuilder(frameworkVersion);

            BootstrapAssemblyPattern.Instance.Consume(frameworkTestBuilder, assembly);
        }
    }
}