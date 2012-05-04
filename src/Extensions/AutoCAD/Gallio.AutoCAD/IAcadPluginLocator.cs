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

using System.IO;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Provides the path to an AutoCAD plugin.
    /// </summary>
    public interface IAcadPluginLocator
    {
        /// <summary>
        /// Gets the location of the AutoCAD plugin for the specified version.
        /// </summary>
        /// <param name="acadVersion">
        /// The version reported to Gallio from AutoCAD. This is that same value the <c>ACADVER</c>
        /// variable provides in AutoCAD. Specify <c>null</c> to find the plugin with highest version.
        /// </param>
        /// <returns>The location of the AutoCAD plugin.</returns>
        /// <exception cref="FileNotFoundException">If an AutoCAD plugin can't be found.</exception>
        string GetPluginPath(string acadVersion);
    }
}