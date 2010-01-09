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

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Locates resources owned by plugins and other components.
    /// </summary>
    public interface IResourceLocator
    {
        /// <summary>
        /// Resolves a resource Uri to a full path.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>file: Maps to the file's local path.</item>
        /// <item>plugin: Maps to a resource within the plugin's base directory, specified in the form: "plugin://Plugin.Id/RelativePath/To/Resource.txt".</item>
        /// </list>
        /// </remarks>
        /// <param name="resourceUri">The resource Uri.</param>
        /// <returns>The resolved full path of the resource.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceUri"/> is null.</exception>
        /// <exception cref="RuntimeException">Thrown if the uri cannot be resolved.</exception>
        string ResolveResourcePath(Uri resourceUri);
    }
}
