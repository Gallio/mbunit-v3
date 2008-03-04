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

using System;

namespace Gallio.Utilities
{
    /// <summary>
    /// A 64bit hash code value type.
    /// Provides somewhat more protection against collisions than 32 bit hashes.
    /// </summary>
    /// <todo author="jeff">
    /// Add equality comparisons and GetHashCode implementation.
    /// Currently these methods are not required.
    /// </todo>
    public struct Hash64
    {
        private readonly long value;

        /// <summary>
        /// Gets a hash code containing the specified 64bit value.
        /// </summary>
        /// <param name="value">The 64bit value</param>
        public Hash64(long value)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates a proabilistically unique 64bit hash code.
        /// </summary>
        public static Hash64 CreateUniqueHash()
        {
            return new Hash64().Add(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Produces a new hash code by combining information from the specified string
        /// with this hash code.
        /// </summary>
        /// <param name="str">The string, may be null if none</param>
        /// <returns>The augmented hash code</returns>
        public Hash64 Add(string str)
        {
            long newValue = value;

            if (str != null)
            {
                int length = str.Length;

                for (int i = 0; i < length; i++)
                    newValue = (newValue << 5) ^ (newValue >> 59) ^ str[i];
            }

            unchecked { newValue *= 0x3E36B306AD118E71L; } // a large prime to scatter the bits around

            return new Hash64(newValue);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Convert.ToString(value, 16);
        }
    }
}
