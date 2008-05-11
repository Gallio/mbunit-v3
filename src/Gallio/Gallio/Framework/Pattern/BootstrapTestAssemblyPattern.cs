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

using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The pattern used to bootstrap test enumeration for assemblies.
    /// </summary>
    public class BootstrapTestAssemblyPattern : BasePattern
    {
        /// <summary>
        /// Gets the singleton instance of the pattern.
        /// </summary>
        public static readonly BootstrapTestAssemblyPattern Instance = new BootstrapTestAssemblyPattern();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override bool IsTest(PatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return evaluator.IsTest(codeElement, DefaultAssemblyPattern);
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            if (assembly != null)
                containingScope.Consume(codeElement, skipChildren, DefaultAssemblyPattern);
        }

        /// <summary>
        /// Gets the default pattern to apply to assemblies that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestAssemblyPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultAssemblyPattern
        {
            get { return TestAssemblyPatternAttribute.DefaultInstance; }
        }
    }
}