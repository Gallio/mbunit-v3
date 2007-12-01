// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// Represents a structural element of some code base. 
    /// </summary>
    public interface ICodeElementInfo
    {
        /// <summary>
        /// Gets the name of the code element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a <see cref="CodeReference" /> for this code element.
        /// </summary>
        /// <returns>The code reference</returns>
        CodeReference CodeReference { get; }

        /// <summary>
        /// Gets information about all attributes of the code element.
        /// </summary>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>The code element's attributes</returns>
        IEnumerable<IAttributeInfo> GetAttributeInfos(bool inherit);

        /// <summary>
        /// Returns true if the code element has an attribute of the specified type.
        /// </summary>
        /// <param name="attributeType">The attribute type</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>True if the code element has at least one attribute of the specified type</returns>
        bool HasAttribute(Type attributeType, bool inherit);

        /// <summary>
        /// Gets the code element's attributes of the specified type.
        /// </summary>
        /// <param name="attributeType">The attribute type</param>
        /// <param name="inherit">If true, includes inherited attributes</param>
        /// <returns>The attributes</returns>
        IEnumerable<object> GetAttributes(Type attributeType, bool inherit);

        /// <summary>
        /// Gets the XML documentation associated with the code element.
        /// </summary>
        /// <returns>The XML documentation or null if none available</returns>
        string GetXmlDocumentation();
    }
}
