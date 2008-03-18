// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;

using Gallio.Model;

namespace Gallio.Icarus.Core.CustomEventArgs 
{
    public class TestResultEventArgs : EventArgs
    {
        private string testName, typeName, namespaceName, assemblyName;
        private TestOutcome testOutcome;
        private double duration;

        public string TestName { get { return testName; } }
        public TestOutcome TestOutcome { get { return testOutcome; } }
        public double Duration { get { return duration; } }
        public string TypeName { get { return typeName; } }
        public string NamespaceName { get { return namespaceName; } }
        public string AssemblyName { get { return assemblyName; } }

        public TestResultEventArgs(string testName, TestOutcome testOutcome, double duration, 
            string typeName, string namespaceName, string assemblyName)
        {
            this.testName = testName;
            this.testOutcome = testOutcome;
            this.duration = duration;
            this.typeName = typeName;
            this.namespaceName = namespaceName;
            this.assemblyName = assemblyName;
        }
    }
}
