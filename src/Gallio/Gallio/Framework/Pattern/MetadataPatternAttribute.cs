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
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Applies declarative metadata to a test component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A metadata attribute is similar to a decorator but more restrictive.  Metadata does
    /// not modify the structure of a test directly.  Instead it introduces additional metadata
    /// key / value pairs for classification, filtering, reporting, documentation or other purposes.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            Validate(scope, codeElement);

            foreach (KeyValuePair<string, string> pair in GetMetadata())
                scope.TestComponentBuilder.AddMetadata(pair.Key, pair.Value);
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="codeElement">The code element.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly.</exception>
        protected virtual void Validate(IPatternScope scope, ICodeElementInfo codeElement)
        {
            if (!scope.IsTestDeclaration && !scope.IsTestParameterDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test or test parameter.");
        }

        /// <summary>
        /// Gets the metadata key / value pairs to be added to the test component.
        /// </summary>
        /// <returns>The metadata entries.</returns>
        protected abstract IEnumerable<KeyValuePair<string, string>> GetMetadata();
    }
}