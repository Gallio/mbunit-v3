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

using Gallio.Icarus.Core.Model;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class TestRunnerModelTest
    {
        [Test]
        [Ignore("Incomplete")]
        public void LoadUpAssemblyAndGetTestTree_Test()
        {
            //TestPackage testpackage = new TestPackage();
            //testpackage.AssemblyFiles.Add("C:\\Source\\MbUnitGoogle\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.MbUnit2\\bin\\MbUnit.TestResources.MbUnit2.dll");
            
            TestRunnerModel main = new TestRunnerModel();

            //TestModel t = main.LoadUpAssembly(runner, testpackage);
        }

    }
    
}
