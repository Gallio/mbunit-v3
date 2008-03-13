// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(IPatternResolver patternResolver, ICodeElementInfo codeElement)
        {
            IPattern pattern = PatternUtils.GetPrimaryPattern(patternResolver, codeElement);
            if (pattern == null)
            {
                IAssemblyInfo assembly = codeElement as IAssemblyInfo;
                if (assembly != null)
                    pattern = DefaultAssemblyPattern;

                if (pattern == null)
                    return false;
            }

            // TODO: Need to find a way to handle default patterns for elements besides assemblies.
            return pattern.IsTest(patternResolver, codeElement);
        }

        /// <inheritdoc />
        public override void Consume(IPatternTestBuilder containingTestBuilder, ICodeElementInfo codeElement, bool skipChildren)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            if (assembly != null)
                ProcessAssembly(containingTestBuilder, assembly, skipChildren);
        }

        /// <summary>
        /// Gets the default pattern to apply to assemblies that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="AssemblyPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultAssemblyPattern
        {
            get { return AssemblyPatternAttribute.DefaultInstance; }
        }

        /// <summary>
        /// Gets the primary pattern of an assembly, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="assembly">The assembly</param>
        /// <returns>The primary pattern, or null if none</returns>
        protected IPattern GetPrimaryAssemblyPattern(IPatternResolver patternResolver, IAssemblyInfo assembly)
        {
            return PatternUtils.GetPrimaryPattern(patternResolver, assembly) ?? DefaultAssemblyPattern;
        }

        /// <summary>
        /// Processes an assembly.
        /// </summary>
        /// <param name="frameworkTestBuilder">The test builder for the framework</param>
        /// <param name="assembly">The assembly</param>
        /// <param name="skipChildren">If true, skips processing types within the assembly (if supported)</param>
        protected virtual void ProcessAssembly(IPatternTestBuilder frameworkTestBuilder, IAssemblyInfo assembly, bool skipChildren)
        {
            IPattern pattern = GetPrimaryAssemblyPattern(frameworkTestBuilder.TestModelBuilder.PatternResolver, assembly);
            if (pattern != null)
                pattern.Consume(frameworkTestBuilder, assembly, skipChildren);
        }
    }
}