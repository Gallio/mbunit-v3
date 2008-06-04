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
// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Icarus.Core.CustomEventArgs 
{
    public class ProgressUpdateEventArgs : EventArgs
    {
        private readonly string taskName, subTaskName;
        private readonly double completedWorkUnits, totalWorkUnits;

        public string TaskName
        {
            get { return taskName; }
        }

        public string SubTaskName
        {
            get { return subTaskName; }
        }

        public double CompletedWorkUnits
        {
            get { return completedWorkUnits; }
        }

        public double TotalWorkUnits
        {
            get { return totalWorkUnits; }
        }

        public ProgressUpdateEventArgs(string taskName, string subTaskName, double completedWorkUnits, double totalWorkUnits)
        {
            this.taskName = taskName;
            this.subTaskName = subTaskName;
            this.completedWorkUnits = completedWorkUnits;
            this.totalWorkUnits = totalWorkUnits;
        }
    }
}
