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

using System;
using Gallio.Model;
using Gallio.XunitAdapter.Properties;

namespace Gallio.XunitAdapter.Model
{
    /// <summary>
    /// Builds a test object model based on reflection against Xunit framework attributes.
    /// </summary>
    public class XunitTestFramework : BaseTestFramework
    {
        private static readonly Guid FrameworkId = new Guid("{CA37318B-0097-4fbe-B013-CB5256F8CF45}");

        /// <inheritdoc />
        public override Guid Id
        {
            get { return FrameworkId; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return Resources.XunitTestFramework_XunitFrameworkName; }
        }

        /// <inheritdoc />
        public override ITestExplorer CreateTestExplorer(TestModel testModel)
        {
            return new XunitTestExplorer(testModel);
        }
    }
}
