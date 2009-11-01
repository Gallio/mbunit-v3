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

using System.Windows.Forms;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models.TestTreeNodes
{
    [Category("TestTreeNodes"), TestsOn(typeof(FilterNode))]
    public class FilterNodeTest
    {
        [Test]
        public void Name_should_match_test_status()
        {
            const TestStatus testStatus = TestStatus.Passed;
            
            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(testStatus.ToString(), filterNode.Name);
        }

        [Test]
        public void Text_should_match_test_status()
        {
            const TestStatus testStatus = TestStatus.Failed;

            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(testStatus.ToString(), filterNode.Text);
        }

        [Test]
        public void TestStatus_should_match_test_status()
        {
            const TestStatus testStatus = TestStatus.Skipped;

            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(testStatus, filterNode.TestStatus);
        }

        [Test]
        public void CheckState_should_be_checked()
        {
            const TestStatus testStatus = TestStatus.Inconclusive;

            var filterNode = new FilterNode(testStatus);

            Assert.AreEqual(CheckState.Checked, filterNode.CheckState);
        }
    }
}
