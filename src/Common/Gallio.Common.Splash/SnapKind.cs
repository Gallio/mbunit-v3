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

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the kind of snap.
    /// </summary>
    public enum SnapKind
    {
        /// <summary>
        /// The character snap did not succeed.  The position was outside the bounds of the document.
        /// </summary>
        None,

        /// <summary>
        /// The character snap was exact.
        /// </summary>
        Exact,

        /// <summary>
        /// The character snap was before the actual character or at a position above
        /// the start of the document.  (eg. To the left of the character if the reading order is left-to-right.)
        /// </summary>
        Leading,

        /// <summary>
        /// The character snap was after the actual character or at a position above
        /// the start of the document.  (eg. To the right of the character if the reading order is left-to-right.)
        /// </summary>
        Trailing
    }
}
