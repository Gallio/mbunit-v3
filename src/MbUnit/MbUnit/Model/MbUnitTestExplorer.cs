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

namespace MbUnit.Model
{
    /// <summary>
    /// A test explorer for MbUnit.
    /// </summary>
    public class MbUnitTestExplorer : BaseTestExplorer
    {
        private const string ExplorerStateKey = "MbUnit:ExplorerState";
        private const string MbUnitAssemblyDisplayName = @"MbUnit";

        private sealed class ExplorerState
        {
            public readonly ITestModelBuilder Builder;
            public readonly List<IAssemblyInfo> Assemblies;

            public ExplorerState(TestModel testModel)
            {
                Builder = new DefaultTestModelBuilder(testModel, PatternAttributeReflectionPolicy.Instance);
                Assemblies = new List<IAssemblyInfo>();
            }
        }

        /// <inheritdoc />
        public override bool IsTest(ICodeElementInfo element)
        {
            // FIXME: This check is not entirely accurate because it
            //        ignores any custom rules that might be defined elsewhere.
            return element.HasAttribute(typeof(PatternAttribute), true);
        }

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, TestModel testModel, Action<ITest> consumer)
        {
            ExplorerState state = GetExplorerState(testModel);

            BuildAssemblyTest(assembly, state);

            foreach (ITestBuilder testBuilder in state.Builder.GetTestBuilders(assembly))
            {
                testBuilder.Test.Populate(true);

                if (consumer != null)
                    consumer(testBuilder.Test);
            }
        }

        /// <inheritdoc />
        public override void ExploreType(ITypeInfo type, TestModel testModel, Action<ITest> consumer)
        {
            ExplorerState state = GetExplorerState(testModel);
            IAssemblyInfo assembly = type.Assembly;

            BuildAssemblyTest(assembly, state);

            foreach (ITestBuilder testBuilder in state.Builder.GetTestBuilders(assembly))
                testBuilder.Test.Populate(false);

            foreach (ITestBuilder testBuilder in state.Builder.GetTestBuilders(type))
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

        private static void BuildAssemblyTest(IAssemblyInfo assembly, ExplorerState state)
        {
            if (state.Assemblies.Contains(assembly))
                return;

            state.Assemblies.Add(assembly);

            Version frameworkVersion = GetFrameworkVersion(assembly);
            if (frameworkVersion == null)
                return;

            ITestBuilder frameworkTestBuilder = state.Builder.GetFrameworkTestBuilder(frameworkVersion);

            BootstrapAssemblyPattern.Instance.Consume(frameworkTestBuilder, assembly);
        }

        private static ExplorerState GetExplorerState(TestModel testModel)
        {
            ExplorerState state = testModel.UserData.GetValue<ExplorerState>(ExplorerStateKey);

            if (state == null)
            {
                state = new ExplorerState(testModel);
                testModel.UserData.SetValue(ExplorerStateKey, state);
            }

            return state;
        }
    }
}