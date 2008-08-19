// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Utilities;

namespace Gallio.Framework.Comparisons
{
    /// <summary>
    /// Describes the difference between a range of the left document and a range of the right document.
    /// Each diff is accompanied by a <see cref="DiffKind" /> that indicates whether it represents
    /// an identical region or one with changes.
    /// </summary>
    [Serializable]
    public struct Diff
    {
        private readonly Range leftRange;
        private readonly Range rightRange;
        private readonly DiffKind kind;

        /// <summary>
        /// Create a diff.
        /// </summary>
        /// <param name="kind">The diff kind which indicates whether there is a change over the range covered by the diff</param>
        /// <param name="leftRange">The range of the left document that is affected</param>
        /// <param name="rightRange">The range of the right document that is affected</param>
        public Diff(DiffKind kind, Range leftRange, Range rightRange)
        {
            this.leftRange = leftRange;
            this.rightRange = rightRange;
            this.kind = kind;
        }

        /// <summary>
        /// Gets the diff kind which indicateds whether there is a change over the range covered by the diff.
        /// </summary>
        public DiffKind Kind
        {
            get { return kind; }
        }

        /// <summary>
        /// Gets the range of the left document that is affected.
        /// </summary>
        public Range LeftRange
        {
            get { return leftRange; }
        }

        /// <summary>
        /// Gets the range of the right document that is affected.
        /// </summary>
        public Range RightRange
        {
            get { return rightRange; }
        }
    }
}
