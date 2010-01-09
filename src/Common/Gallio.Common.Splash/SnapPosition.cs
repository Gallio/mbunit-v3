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

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Provides the result of mapping a screen position to a character index.
    /// </summary>
    public struct SnapPosition
    {
        private readonly SnapKind kind;
        private readonly int charIndex;

        /// <summary>
        /// Initializes a snap result.
        /// </summary>
        /// <param name="kind">The snap kind.</param>
        /// <param name="charIndex">The character index of the snap, or -1 if no snap.</param>
        public SnapPosition(SnapKind kind, int charIndex)
        {
            this.kind = kind;
            this.charIndex = charIndex;
        }

        /// <summary>
        /// Gets the snap kind.
        /// </summary>
        public SnapKind Kind
        {
            get { return kind; }
        }

        /// <summary>
        /// Gets the character index of the snap, or -1 if no snap.
        /// </summary>
        public int CharIndex
        {
            get { return charIndex; }
        }
    }
}
