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

using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Helpers
{
    internal class TestTreeNodeFactory
    {
        public static TestTreeNode CreateNode(TestData testData)
        {
            string testKind = testData.Metadata.GetValue(MetadataKeys.TestKind);
            TestTreeNode testTreeNode;

            switch (testKind)
            {
                case TestKinds.Assembly:
                    testTreeNode = new AssemblyNode(testData);
                    break;

                case TestKinds.Fixture:
                    testTreeNode = new FixtureNode(testData);
                    break;

                case TestKinds.Test:
                    testTreeNode = new TestNode(testData);
                    break;

                case TestKinds.Group:
                    testTreeNode = new GroupNode(testData);
                    break;

                case TestKinds.Framework:
                    testTreeNode = new FrameworkNode(testData);
                    break;

                case TestKinds.Root:
                    testTreeNode = new RootNode(testData);
                    break;

                default:
                    testTreeNode = new TestDataNode(testData) 
                        { TestKind = testKind };
                    break;
            }

            return testTreeNode;
        }
    }
}
