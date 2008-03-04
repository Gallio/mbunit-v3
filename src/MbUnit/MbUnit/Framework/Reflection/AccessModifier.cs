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

using System.Reflection;

namespace MbUnit.Framework.Reflection
{
    ///<summary>
    /// Access modifier of a class or class member.
    ///</summary>
    public enum AccessModifier
    { 
        ///<summary>
        /// public
        ///</summary>
        Public = BindingFlags.Public, 
        ///<summary>
        /// protected, internal, private
        ///</summary>
        NonPublic = BindingFlags.NonPublic, 
        /// <summary>
        /// static
        /// </summary>
        Static  = BindingFlags.Static,
        /// <summary>
        /// default that includes public, protected, internal, private, and static
        /// </summary>
        Default = Public | NonPublic | Static
    }
}