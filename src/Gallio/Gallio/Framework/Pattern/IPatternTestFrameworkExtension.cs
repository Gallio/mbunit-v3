// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using System.Collections.Generic;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// A pattern test framework extension provides the opportunity to extend
    /// the standard pattern test framework with additional behaviors.
    /// </para>
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public interface IPatternTestFrameworkExtension
    {
        /// <summary>
        /// <para>
        /// Gets information about the tools that are used by the specified test assembly.
        /// The tool information will be included in the report as part of the framework
        /// node that contains the assembly.
        /// </para>
        /// <para>
        /// This method is used by authors of tools that are derived from the
        /// <see cref="PatternTestFramework" /> to provide brand and version information
        /// about the tool so that it will be visible to end users.
        /// </para>
        /// <para>
        /// If no tools are referenced by the assembly, it will be ignored by the
        /// <see cref="PatternTestExplorer" />.
        /// </para>
        /// </summary>
        /// <param name="assembly">The test assembly</param>
        /// <returns>The tool information</returns>
        IEnumerable<ToolInfo> GetReferencedTools(IAssemblyInfo assembly);
    }
}
