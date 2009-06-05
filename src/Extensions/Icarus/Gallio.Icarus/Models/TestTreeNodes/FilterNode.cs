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

using Gallio.Model;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    internal class FilterNode : TestTreeNode
    {
        public FilterNode(TestStatus testStatus)
            : base(testStatus.ToString(), testStatus.ToString())
        {
            switch (testStatus)
            {
                case TestStatus.Passed:
                    NodeTypeIcon = Properties.Resources.FilterPassed;
                    break;
                case TestStatus.Failed:
                    NodeTypeIcon = Properties.Resources.FilterFailed;
                    break;
                case TestStatus.Skipped:
                    NodeTypeIcon = Properties.Resources.FilterSkipped;
                    break;
            }
            TestStatus = testStatus;
        }
    }
}
