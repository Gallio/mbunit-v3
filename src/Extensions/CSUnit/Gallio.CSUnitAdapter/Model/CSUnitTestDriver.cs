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
using Gallio.Model.Helpers;
using Gallio.Runtime.Hosting;
using Gallio.Model;
using Gallio.Common.Reflection;
using System.Reflection;
using System.IO;

namespace Gallio.CSUnitAdapter.Model
{
    internal class CSUnitTestDriver : SimpleTestDriver
    {
        protected override string FrameworkName
        {
            get { return "csUnit"; }
        }

        protected override TestExplorer CreateTestExplorer()
        {
            return new CSUnitTestExplorer();
        }

        protected override TestController CreateTestController()
        {
            return new DelegatingTestController(test =>
            {
                var topTest = test as CSUnitAssemblyTest;
                return topTest != null ? new CSUnitTestController(topTest.AssemblyLocation) : null;
            });
        }

        protected override void ConfigureHostSetup(HostSetup hostSetup, TestPackage testPackage, string assemblyPath, AssemblyMetadata assemblyMetadata)
        {
            base.ConfigureHostSetup(hostSetup, testPackage, assemblyPath, assemblyMetadata);

            // csUnit always creates its own AppDomain with shadow copy enabled so we do not need this.
            hostSetup.ShadowCopy = false;
        }
    }
}
