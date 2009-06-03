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

namespace Gallio.Common.Text
{
    /// <summary>
    /// Describes whether a difference represents a changed or unchanged region.
    /// </summary>
    public enum DiffKind
    {
        /// <summary>
        /// Indicates there is no change between the left and right documents over the ranges described by the diff.
        /// </summary>
        NoChange,

        /// <summary>
        /// Indicates there is a change between the left and right documents over the ranges described by the diff.
        /// Text may have been added, removed, or edited.
        /// </summary>
        Change
    }
}
