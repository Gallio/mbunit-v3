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

using System.Xml.Serialization;

namespace Gallio.Model
{
    /// <summary>
    /// Describes whether a test was skipped, passed, failed or was inconclusive.
    /// </summary>
    public enum TestStatus
    {
        /// <summary>
        /// The test did not run so nothing is known about its status.
        /// </summary>
        [XmlEnum("skipped")]
        Skipped = 0,

        /// <summary>
        /// The test passed.
        /// </summary>
        [XmlEnum("passed")]
        Passed = 1,

        /// <summary>
        /// The test did not run or its result was inconclusive.
        /// </summary>
        [XmlEnum("inconclusive")]
        Inconclusive = 2,

        /// <summary>
        /// The test failed or encountered an error during execution.
        /// </summary>
        [XmlEnum("failed")]
        Failed = 3,
    }
}