// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Targeted member types for the fixture invoker.
    /// </summary>
    [Flags]
    public enum FixtureMemberInvokerTargets
    {
        /// <summary>
        /// Default value combining <see cref="Field"/>, <see cref="Property"/>, and <see cref="Method"/>.
        /// </summary>
        Default = Field | Property | Method,

        /// <summary>
        /// Indicates to find the searched member among the fields.
        /// </summary>
        Field = 1,

        /// <summary>
        /// Indicates to find the searched member among the properties.
        /// </summary>
        Property = 2,

        /// <summary>
        /// Indicates to find the searched member among the methods.
        /// </summary>
        Method = 3,
    }
}
