// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gallio.Model
{
    /// <summary>
    /// Describes whether the test ran and how it terminated.
    /// </summary>
    public enum TestStatus
    {
        /// <summary>
        /// The test did not run or has not yet run but it has not been
        /// ignored, skipped or canceled.
        /// </summary>
        [XmlEnum("notRun")]
        NotRun,

        /// <summary>
        /// The test did not run because it was ignored.
        /// </summary>
        [XmlEnum("ignored")]
        Ignored,

        /// <summary>
        /// The test did not run because it was skipped.
        /// </summary>
        [XmlEnum("skipped")]
        Skipped,

        /// <summary>
        /// The test ran.
        /// </summary>
        [XmlEnum("executed")]
        Executed,

        /// <summary>
        /// The test started execution but was canceled prematurely
        /// by direct user intervention.
        /// </summary>
        [XmlEnum("canceled")]
        Canceled,

        /// <summary>
        /// A fatal error occurred that prevented the test runner from executing
        /// the test to completion.
        /// </summary>
        [XmlEnum("error")]
        Error
    }
}