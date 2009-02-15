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
using System.IO;
using System.Reflection;
using Gallio.Model;
using Gallio.CSUnitAdapter.Properties;
using Gallio.Runtime.Hosting;

namespace Gallio.CSUnitAdapter.Model
{
    /// <summary>
    /// Builds a test object model based on reflection against CSUnit framework attributes.
    /// </summary>
    public class CSUnitTestFramework : BaseTestFramework
    {
        private static readonly Guid FrameworkId = new Guid("{B55A8096-EFB9-4570-B977-75695D614E3B}");

        /// <inheritdoc />
        public override Guid Id
        {
            get { return FrameworkId; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return Resources.CSUnitTestFramework_FrameworkName; }
        }

        /// <inheritdoc />
        public override ITestExplorer CreateTestExplorer(TestModel testModel)
        {
            return new CSUnitTestExplorer(testModel);
        }
    }
}
