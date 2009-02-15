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

using System.Xml.Serialization;

namespace Gallio.Model
{
    /// <summary>
    /// <para>
    /// Describes whether a test passed, failed, was skipped or was inconclusive.
    /// </para>
    /// <para>
    /// The status codes are ranked in order of severity from least to greatest
    /// as follows: <see cref="Passed" />, <see cref="Skipped"/>, <see cref="Inconclusive" />,
    /// <see cref="Failed" />.
    /// </para>
    /// </summary>
    public enum TestStatus
    {
        /// <summary>
        /// The test passed.
        /// </summary>
        [XmlEnum("passed")]
        Passed = 0,

        /// <summary>
        /// The test did not run so nothing is known about its status.
        /// </summary>
        [XmlEnum("skipped")]
        Skipped = 1,

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