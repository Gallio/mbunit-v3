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
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test assembly decorator pattern attribute applies decorations to an
    /// existing test declared at the assembly-level.
    /// </summary>
    /// <seealso cref="TestAssemblyPatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.TestAssembly, AllowMultiple = true, Inherited = true)]
    public abstract class TestAssemblyDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            Validate(scope, assembly);

            scope.TestBuilder.AddDeferredAction(codeElement, Order, delegate
            {
                DecorateAssemblyTest(scope, assembly);
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="assembly">The assembly.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly.</exception>
        protected virtual void Validate(IPatternScope scope, IAssemblyInfo assembly)
        {
            if (!scope.IsTestDeclaration || assembly == null)
                ThrowUsageErrorException("This attribute can only be used on a test assembly.");
        }

        /// <summary>
        /// Applies decorations to an assembly-level test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A typical use of this method is to augment the test with additional metadata
        /// or to add additional behaviors to the test.
        /// </para>
        /// </remarks>
        /// <param name="assemblyScope">The assembly scope.</param>
        /// <param name="assembly">The assembly.</param>
        protected virtual void DecorateAssemblyTest(IPatternScope assemblyScope, IAssemblyInfo assembly)
        {
        }
    }
}