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

namespace Gallio.Framework.Text
{
    /// <summary>
    /// Specifies the presentation style for a <see cref="DiffSet" />.
    /// </summary>
    public enum DiffStyle
    {
        /// <summary>
        /// Display the left and right document contents and diffs fully interleaved.
        /// </summary>
        Interleaved = 0,

        /// <summary>
        /// Display only the left document contents.
        /// </summary>
        LeftOnly,

        /// <summary>
        /// Display only the right document contents.
        /// </summary>
        RightOnly
    }
}
