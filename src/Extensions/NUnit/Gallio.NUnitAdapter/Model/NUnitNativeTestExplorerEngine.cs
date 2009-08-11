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
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.NUnitAdapter.Properties;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Explores tests in NUnit assemblies using the native NUnit test enumeration mechanism.
    /// </summary>
    internal class NUnitNativeTestExplorerEngine : NUnitTestExplorerEngine
    {
        private readonly TestModel testModel;
        private readonly Assembly assembly;

        private NUnitAssemblyTest assemblyTest;

        public NUnitNativeTestExplorerEngine(TestModel testModel, Assembly assembly)
        {
            this.testModel = testModel;
            this.assembly = assembly;
        }

        public override void ExploreAssembly(bool skipChildren)
        {
            if (assemblyTest == null)
            {
                assemblyTest = BuildAssemblyTest(testModel.RootTest);
            }
        }

        private NUnitAssemblyTest BuildAssemblyTest(Test parent)
        {
            IAssemblyInfo assemblyInfo = Reflector.Wrap(assembly);
            NUnit.Core.TestRunner runner = CreateTestRunner(assembly.Location);

            NUnitAssemblyTest assemblyTest = new NUnitAssemblyTest(assemblyInfo, runner);
            PopulateMetadata(assemblyTest);
            PopulateAssemblyTestMetadata(assemblyTest, assemblyInfo);

            foreach (NUnit.Core.ITest assemblyTestSuite in runner.Test.Tests)
                BuildTestChildren(assemblyInfo, assemblyTest, assemblyTestSuite);

            parent.AddChild(assemblyTest);
            return assemblyTest;
        }

        private static NUnit.Core.TestRunner CreateTestRunner(string assemblyLocation)
        {
            NUnit.Core.TestPackage package = new NUnit.Core.TestPackage(@"Tests");

            // The SetupFixture feature requires namespace suites even though we
            // would prefer to turn this off since the namespaces are mainly a presentation concern.
            package.Settings.Add(@"AutoNamespaceSuites", true);
            package.Assemblies.Add(assemblyLocation);

            NUnit.Core.TestRunner runner = new NUnit.Core.RemoteTestRunner();
            if (!runner.Load(package))
                throw new ModelException(Resources.NUnitTestExplorer_CannotLoadNUnitTestAssemblies);

            return runner;
        }
    }
}
