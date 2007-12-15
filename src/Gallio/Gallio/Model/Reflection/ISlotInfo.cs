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

using System;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// A slot represents a field, property or parameter.  It is used to
    /// simplify the handling of data binding since all three of these types
    /// are similar in that they can hold values of some type.
    /// </summary>
    public interface ISlotInfo : ICodeElementInfo, IEquatable<ISlotInfo>
    {
        /// <summary>
        /// Gets the type of value held in the slot.
        /// </summary>
        ITypeInfo ValueType { get; }

        /// <summary>
        /// Gets the positional index of a parameter slot, or 0 in other cases.
        /// </summary>
        int Position { get; }
    }
}
