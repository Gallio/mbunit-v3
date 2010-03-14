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
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml.Diffing
{
    /// <summary>
    /// Represents an XML node that can be compared to another 
    /// node of the same type, in order to get the differences.
    /// </summary>
    /// <typeparam name="TNode">The type of the diffable node.</typeparam>
    public interface IDiffable<TNode>
    {
        /// <summary>
        /// Diffs the current node with an expected prototype, and returns
        /// a set of differences found.
        /// </summary>
        /// <param name="expected">A prototype representing the expected content of the node.</param>
        /// <param name="path">The path of the parent node.</param>
        /// <param name="options">Comparison options.</param>
        /// <returns>The resulting diff set describing the differences found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expected"/> or <paramref name="path"/> is null.</exception>
        DiffSet Diff(TNode expected, IXmlPathStrict path, Options options);
    }
}
