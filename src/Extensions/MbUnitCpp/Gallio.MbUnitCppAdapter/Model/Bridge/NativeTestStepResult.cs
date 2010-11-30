// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Gallio.Model;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// A native structure that holds the results of the test step.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeTestStepResult
    {
        /// <summary>
        /// The native test outcome.
        /// </summary>
        public NativeOutcome NativeOutcome;

        /// <summary>
        /// The number of assertions processed during the execution of the test step.
        /// </summary>
        public int AssertCount;

        /// <summary>
        /// A descriptor of the assertion failure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To be ignored if the outcome is not "Failed".
        /// </para>
        /// </remarks>
        public NativeAssertionFailure Failure;

        /// <summary>
        /// The ID key of the unmanaged string that holds some contents to be appened to the test log.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Zero if no contents were defined.
        /// </para>
        /// </remarks>
        public int TestLogId;

        /// <summary>
        /// The time taken by the test step to run expressed in milliseconds.
        /// </summary>
        public int DurationMilliseconds;
    }
}