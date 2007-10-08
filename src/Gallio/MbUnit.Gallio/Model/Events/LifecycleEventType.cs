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

using MbUnit.Model;

namespace MbUnit.Model.Events
{
    /// <summary>
    /// Describes the types of test lifecycle events.
    /// </summary>
    public enum LifecycleEventType
    {
        /// <summary>
        /// The test step is starting.
        /// </summary>
        Start,

        /// <summary>
        /// The test step is entering a phase of its execution such as setup or teardown.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        SetPhase,

        /// <summary>
        /// The test step is receiving additional metadata.
        /// </summary>
        AddMetadata,

        /// <summary>
        /// The test step is finished.
        /// </summary>
        Finish
    }
}