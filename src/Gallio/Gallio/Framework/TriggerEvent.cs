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

using System;
using Gallio.Common;

namespace Gallio.Framework
{
    /// <summary>
    /// Specifies a trigger condition for automatic execution of an action.
    /// </summary>
    /// <seealso cref="TestContext.AutoExecute(TriggerEvent, Action)"/>
    public enum TriggerEvent
    {
        /// <summary>
        /// Perform the action when the test finishes.
        /// </summary>
        TestFinished,

        /// <summary>
        /// Perform the action when the test passed.
        /// </summary>
        TestPassed,

        /// <summary>
        /// Perform the action when the test passed or was inconclusive.
        /// </summary>
        TestPassedOrInconclusive,

        /// <summary>
        /// Perform the action when the test was inconclusive.
        /// </summary>
        TestInconclusive,

        /// <summary>
        /// Perform the action when the test failed.
        /// </summary>
        TestFailed,

        /// <summary>
        /// Perform the action when the test failed or was inconclusive.
        /// </summary>
        TestFailedOrInconclusive
    }
}
