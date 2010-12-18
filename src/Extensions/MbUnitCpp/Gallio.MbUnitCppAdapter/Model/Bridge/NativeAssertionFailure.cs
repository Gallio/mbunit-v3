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
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Diagnostics;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// The native structure that represents an assertion failure in MbUnitCpp.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeAssertionFailure
    {
        /// <summary>
        /// The ID of the string that holds the description of the failure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Should be always available.
        /// </para>
        /// </remarks>
        public int DescriptionId;

        /// <summary>
        /// The ID of the string that holds the optional user message of the failure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Zero if not message was specified by the user.
        /// </para>
        /// </remarks>
        public int MessageId;

        /// <summary>
        /// An optional labeled value for the expected value.
        /// </summary>
        public NativeLabeledValue ExpectedValue;

        /// <summary>
        /// An optional labeled value for the unexpected value.
        /// </summary>
        public NativeLabeledValue UnexpectedValue;

        /// <summary>
        /// An optional labeled value for the actual value.
        /// </summary>
        public NativeLabeledValue ActualValue;

        /// <summary>
        /// An optional extra labeled value.
        /// </summary>
        public NativeLabeledValue Extra_0;

        /// <summary>
        /// An optional extra labeled value.
        /// </summary>
        public NativeLabeledValue Extra_1;
    }
}
