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

using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The pattern used to bootstrap test enumeration for assemblies.
    /// </summary>
    public class BootstrapAssemblyPattern : BasePattern
    {
        /// <summary>
        /// Gets the singleton instance of the pattern.
        /// </summary>
        public static readonly BootstrapAssemblyPattern Instance = new BootstrapAssemblyPattern();

        /// <summary>
        /// Processes an assembly.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>True if the assembly was consumed</returns>
        protected virtual bool ProcessAssembly(IPatternTestBuilder containingTestBuilder, IAssemblyInfo assembly)
        {
            return PatternUtils.ConsumeWithFallback(containingTestBuilder, assembly, ProcessAssemblyFallback);
        }

        /// <summary>
        /// Processes an assembly using a default rule because no associated pattern has consumed it.
        /// </summary>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>True if the assembly was consumed</returns>
        protected virtual bool ProcessAssemblyFallback(IPatternTestBuilder containingTestBuilder, IAssemblyInfo assembly)
        {
            return AssemblyPatternAttribute.DefaultInstance.Consume(containingTestBuilder, assembly);
        }

        /// <inheritdoc />
        public override bool Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            if (assembly == null)
                return false;

            return ProcessAssembly(containingTestBuilder, assembly);
        }
    }
}