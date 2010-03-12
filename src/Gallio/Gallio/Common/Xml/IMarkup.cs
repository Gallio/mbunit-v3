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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// The base markup for the XML composite tree representing an XML fragment.
    /// </summary>
    public interface IMarkup : IDiffable<IMarkup>, INamed
    {
        /// <summary>
        /// Gets the index of the markup.
        /// </summary>
        int Index
        { 
            get;
        }

        /// <summary>
        /// Gets the children markups.
        /// </summary>
        MarkupCollection Children
        {
            get;
        }

        /// <summary>
        /// Aggregates the representation of the current markup.
        /// </summary>
        /// <param name="aggregator">The aggregator.</param>
        void Aggregate(XmlPathFormatAggregator aggregator);

        /// <summary>
        /// Returns the number of times the searched markup was found at the specified path.
        /// </summary>
        /// <param name="searchedPath">The path of the searched markup.</param>
        /// <param name="expectedValue">The expected value found in the searched markup, or null if the value must be ignored.</param>
        /// <param name="options">Options for the search.</param>
        /// <returns>The number of matching items found; zero if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="searchedPath"/> is null.</exception>
        int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options);
    }
}
