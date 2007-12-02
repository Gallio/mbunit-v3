// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Helper functions for the reflector.
    /// </summary>
    internal static class ReflectorUtils
    {
        public static void AddFlagIfTrue(ref TypeAttributes flags, TypeAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref MethodAttributes flags, MethodAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref FieldAttributes flags, FieldAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref PropertyAttributes flags, PropertyAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref ParameterAttributes flags, ParameterAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }
    }
}
