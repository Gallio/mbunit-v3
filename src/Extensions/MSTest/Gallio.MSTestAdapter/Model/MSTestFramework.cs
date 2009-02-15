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
using Gallio.Model;

namespace Gallio.MSTestAdapter.Model
{
    /// <summary>
    /// Builds a test object model based on reflection against MSTest framework attributes.
    /// </summary>
    public class MSTestFramework : BaseTestFramework
    {
        private static readonly Guid FrameworkId = new Guid("{559AA77B-E0E5-43bb-AF48-EF50D0025D3C}");

        /// <inheritdoc />
        public override Guid Id
        {
            get { return FrameworkId; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return "MSTest"; }
        }

        /// <inheritdoc />
        public override ITestExplorer CreateTestExplorer(TestModel testModel)
        {
            return new MSTestExplorer(testModel);
        }
    }
}
