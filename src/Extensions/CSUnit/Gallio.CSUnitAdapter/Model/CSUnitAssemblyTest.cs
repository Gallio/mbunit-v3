// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using csUnit.Core;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;

namespace Gallio.CSUnitAdapter.Model
{
    public class CSUnitAssemblyTest : CSUnitTest
    {
        private readonly RemoteLoader loader;

        public CSUnitAssemblyTest(IAssemblyInfo assembly, RemoteLoader loader)
            : base(assembly.Name, assembly)
        {
            Kind = TestKinds.Assembly;

            this.loader = loader;
        }

        /// <inheritdoc />
        public override Func<ITestController> TestControllerFactory
        {
            get { return CreateTestController; }
        }

        private ITestController CreateTestController()
        {
            return new CSUnitTestController(loader);
        }

    }
}
