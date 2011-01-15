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
using System.Text;
using Gallio.Model;
using System.Runtime.InteropServices;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// Represents the address of a native unmanaged object/variable.
    /// </summary>
    public struct NativeIntPtr
    {
        private readonly IntPtr value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">The address of the unmanaged object.</param>
        public NativeIntPtr(long value)
        {
            this.value = new IntPtr(value);
        }

        /// <summary>
        /// Parse the specified field.
        /// </summary>
        /// <param name="field">The text representation of the address.</param>
        /// <returns>The address of a native unmanaged object/variable.</returns>
        public static NativeIntPtr Parse(string field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return new NativeIntPtr(Int64.Parse(field));
        }

        /// <summary>
        /// Prints the address as an hexadecimal number.
        /// </summary>
        /// <returns>The text representation of the address</returns>
        public override string ToString()
        {
            return "0x" + value.ToString("x8");
        }
    }
}
