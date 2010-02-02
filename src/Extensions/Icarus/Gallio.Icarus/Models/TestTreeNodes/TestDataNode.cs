// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Schema;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    public sealed class TestDataNode : TestTreeNode, ITestDescriptor
    {
        private readonly TestData testData;

        public TestDataNode(TestData testData)
            : base(testData.Id, testData.Name)
        {
            this.testData = testData;

            CheckState = testData.Metadata.ContainsKey(MetadataKeys.IgnoreReason) || 
                testData.Metadata.ContainsKey(MetadataKeys.PendingReason) ? 
                System.Windows.Forms.CheckState.Unchecked : System.Windows.Forms.CheckState.Checked;
        }

        public string Name
        {
            get { return testData.Name; }
        }

        public ICodeElementInfo CodeElement
        {
            get { return testData.CodeElement; }
        }

        public PropertyBag Metadata
        {
            get { return testData.Metadata; }
        }

        public override string TestKind
        {
            get
            {
                return testData.Metadata.GetValue(MetadataKeys.TestKind) ?? TestKinds.Group;
            }
        }

        public string FileName
        {
            get
            {
                return testData.Metadata.GetValue(MetadataKeys.File);
            }
        }

        public bool SourceCodeAvailable
        {
            get
            {
                return testData.CodeLocation != CodeLocation.Unknown;
            }
        }

        public bool IsTest
        {
            get { return testData.IsTestCase; }
        }
    }
}
