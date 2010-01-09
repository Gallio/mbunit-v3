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
    /// Path to an element or an attribute in an XML fragment. 
    /// </summary>
    public interface IXmlPath
    {
    }

    /// <summary>
    /// Closed path to an element or an attribute in an XML fragment. 
    /// </summary>
    public interface IXmlPathClosed : IXmlPath
    {
    }

    /// <summary>
    /// Open path to an element or an attribute in an XML fragment. 
    /// </summary>
    public interface IXmlPathOpen : IXmlPath
    {
        /// <summary>
        /// Extends the path to the specified element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>The extended path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementName"/> is null.</exception>
        IXmlPathOpen Element(string elementName);

        /// <summary>
        /// Extends the path to the specified element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="declarative">A value indicating whether the element is declarative.</param>
        /// <returns>The extended path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementName"/> is null.</exception>
        IXmlPathOpen Element(string elementName, bool declarative);

        /// <summary>
        /// Extends the path to the specified attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The extended path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributeName"/> is null.</exception>
        IXmlPathClosed Attribute(string attributeName);
    }
}
