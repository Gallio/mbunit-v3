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

using System;
using System.Reflection;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The recursive type pattern may be used when a type is not consumed by any
    /// other pattern.  It simply recurses back into the pattern engine to consume
    /// public and non-public nested types, if any.
    /// </summary>
    public class RecursiveTypePattern : BasePattern
    {
        /// <summary>
        /// Gets a singleton instance of this pattern.
        /// </summary>
        public static readonly RecursiveTypePattern Instance = new RecursiveTypePattern();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void Consume(PatternEvaluationScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            ITypeInfo type = (ITypeInfo)codeElement;
            foreach (ITypeInfo nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
                containingScope.Consume(nestedType, false, DefaultNestedTypePattern);
        }

        /// <summary>
        /// Gets the default pattern to apply to nested types that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="RecursiveTypePattern.Instance"/>.
        /// </remarks>
        protected virtual IPattern DefaultNestedTypePattern
        {
            get { return Instance; }
        }
    }
}
