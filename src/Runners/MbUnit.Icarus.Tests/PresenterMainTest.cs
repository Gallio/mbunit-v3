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

using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Core.Runtime;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Icarus.Core.Model;
using MbUnit.TestResources.MbUnit2;

namespace MbUnit.Icarus.Tests
{
    [TestFixture]
    public class PresenterMainTest
    {
        [Test]
        public void LoadUpAssemblyAndGetTestTree_Test()
        {
            RuntimeSetup runtimeSetup = new RuntimeSetup();
            AutoRunner runner = AutoRunner.CreateRunner(runtimeSetup);

            TestPackage testpackage = new TestPackage();
            testpackage.AssemblyFiles.Add(typeof(SimpleTest).Assembly.Location);
            
            TestRunnerModel main = new TestRunnerModel();
            
            TestModel t = main.LoadUpAssembly(runner, testpackage);
        }

    }
    
}
