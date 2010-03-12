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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Represents an immutable and loose path to a markup in an XML fragment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A loose path is built based on the name of the elements that lead to it. 
    /// Thus several markups might be represented by the same loose path.
    /// </para>
    /// </remarks>
    public interface IXmlPathLooseOpen : IXmlPathLooseClosed
    {
        /// <summary>
        /// Extends the path to a child element.
        /// </summary>
        /// <param name="name">The name of the child element.</param>
        /// <returns>A new immutable open path that encapsulates the current path.</returns>
        IXmlPathLooseOpen Element(string name);

        /// <summary>
        /// Extends the path to a child attribute.
        /// </summary>
        /// <param name="name">The name of the child attribute.</param>
        /// <returns>A new immutable closed path that encapsulates the current path.</returns>
        IXmlPathLooseClosed Attribute(string name);
    }
}