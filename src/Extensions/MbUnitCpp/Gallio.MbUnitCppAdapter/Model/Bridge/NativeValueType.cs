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
    /// The native type of the actual/expected value sent by MbUnitCpp.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When a assertion failure occurs, the actual and expected values are sent as
    /// a text to the adapter. We use the value type to parse it as an equivalent
    /// managed type and to display it properly in the report/log.
    /// </para>
    /// <para>
    /// Values should match "enum ValueType" as defined in "mbunit.h".
    /// </para>
    /// </remarks>
    public enum NativeValueType
    {
        /// <summary>
        /// Raw string to represents custom user types. 
        /// <remarks>
        /// <para>
        /// Displayed as is, without any diffing.
        /// </para>
        /// </remarks>
        /// </summary>
        Raw = 0,

        /// <summary>
        /// Simple unicode text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Displayed with diffing if both the actual and the expected values are available.
        /// </para>
        /// </remarks>
        String = 1,

        /// <summary>
        /// A boolean value parsed with <see cref="System.Boolean.Parse"/>.
        /// </summary>
        Boolean = 2,

        /// <summary>
        /// A unicode character value parsed with <see cref="System.Char.Parse"/>.
        /// </summary>
        Char = 3,

        /// <summary>
        /// An unsigned 8-bit value parsed with <see cref="System.Byte.Parse"/>.
        /// </summary>
        Byte = 4,

        /// <summary>
        /// An signed 16-bit value parsed with <see cref="System.Int16.Parse"/>.
        /// </summary>
        Int16 = 5,

        /// <summary>
        /// An unsigned 16-bit value parsed with <see cref="System.UInt16.Parse"/>.
        /// </summary>
        UInt16 = 6,

        /// <summary>
        /// An signed 32-bit value parsed with <see cref="System.Int32.Parse"/>.
        /// </summary>
        Int32 = 7,

        /// <summary>
        /// An unsigned 32-bit value parsed with <see cref="System.UInt32.Parse"/>.
        /// </summary>
        UInt32 = 8,

        /// <summary>
        /// An signed 64-bit value parsed with <see cref="System.Int64.Parse"/>.
        /// </summary>
        Int64 = 9,

        /// <summary>
        /// An unsigned 64-bit value parsed with <see cref="System.UInt64.Parse"/>.
        /// </summary>
        UInt64 = 10,

        /// <summary>
        /// An floating point 32-bit value parsed with <see cref="System.Single.Parse"/>.
        /// </summary>
        Single = 11,

        /// <summary>
        /// An floating point 64-bit value parsed with <see cref="System.Double.Parse"/>.
        /// </summary>
        Double = 12,

        /// <summary>
        /// The address of an objet/variable. Parsed with <see cref="System.UInt64.Parse"/> and
        /// displayed in hexadecimal.
        /// </summary>
        Address = 13,
    }
}