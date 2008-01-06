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

using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for manipulating reflection flags enumerations.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public class ReflectorFlagsUtils
    {
        /// <summary>
        /// Adds <paramref name="flagToAdd"/> to the <paramref name="flags"/> enumeration
        /// if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="flags">The flags enumeration to update</param>
        /// <param name="flagToAdd">The flag to add if <paramref name="condition"/> is true</param>
        /// <param name="condition">The condition to check</param>
        public static void AddFlagIfTrue(ref TypeAttributes flags, TypeAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        /// <summary>
        /// Adds <paramref name="flagToAdd"/> to the <paramref name="flags"/> enumeration
        /// if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="flags">The flags enumeration to update</param>
        /// <param name="flagToAdd">The flag to add if <paramref name="condition"/> is true</param>
        /// <param name="condition">The condition to check</param>
        public static void AddFlagIfTrue(ref MethodAttributes flags, MethodAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        /// <summary>
        /// Adds <paramref name="flagToAdd"/> to the <paramref name="flags"/> enumeration
        /// if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="flags">The flags enumeration to update</param>
        /// <param name="flagToAdd">The flag to add if <paramref name="condition"/> is true</param>
        /// <param name="condition">The condition to check</param>
        public static void AddFlagIfTrue(ref FieldAttributes flags, FieldAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        /// <summary>
        /// Adds <paramref name="flagToAdd"/> to the <paramref name="flags"/> enumeration
        /// if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="flags">The flags enumeration to update</param>
        /// <param name="flagToAdd">The flag to add if <paramref name="condition"/> is true</param>
        /// <param name="condition">The condition to check</param>
        public static void AddFlagIfTrue(ref PropertyAttributes flags, PropertyAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        /// <summary>
        /// Adds <paramref name="flagToAdd"/> to the <paramref name="flags"/> enumeration
        /// if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="flags">The flags enumeration to update</param>
        /// <param name="flagToAdd">The flag to add if <paramref name="condition"/> is true</param>
        /// <param name="condition">The condition to check</param>
        public static void AddFlagIfTrue(ref ParameterAttributes flags, ParameterAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }
    }
}