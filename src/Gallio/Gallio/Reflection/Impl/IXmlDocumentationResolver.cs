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

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Resolves members to XML documentation contents.
    /// </summary>
    public interface IXmlDocumentationResolver
    {
        /// <summary>
        /// Gets the XML documentation for a member in an assembly.
        /// </summary>
        /// <param name="assemblyPath">The assembly path</param>
        /// <param name="memberId">The XML documentation id of the member</param>
        /// <returns>The XML documentation for the member, or null if none available</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyPath"/> or <paramref name="memberId"/> is null</exception>
        string GetXmlDocumentation(string assemblyPath, string memberId);
    }
}