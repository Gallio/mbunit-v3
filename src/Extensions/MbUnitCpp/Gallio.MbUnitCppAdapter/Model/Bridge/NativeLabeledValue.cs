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
    /// The native structure that contains a labeled value for an assertion failure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeLabeledValue
    {
        /// <summary>
        /// The ID of the string that holds the label.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Zero if no label was specified (e.g. we do not specify any label for
        /// predifined values such as the actual and the expected ones.)
        /// </para>
        /// </remarks>
        public int LabelId;

        /// <summary>
        /// The ID of the string that holds the text version of the value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Zero if no value was specified.
        /// </para>
        /// </remarks>
        public int ValueId;

        /// <summary>
        /// The native type of the value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ignored if <see cref="ValueId"/> is zero.
        /// </para>
        /// </remarks>
        public NativeValueType ValueType;

        /// <summary>
        /// Indicates whether the labeled value has some contents.
        /// </summary>
        public bool IsValid
        { 
            get 
            { 
                return ValueId > 0; 
            } 
        }
    }
}
