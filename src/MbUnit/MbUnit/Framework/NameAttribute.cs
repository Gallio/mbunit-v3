// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Overrides the name of a test or test parameter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this attribute to specify a more descriptive or readable name than
    /// the one chosen by default by MbUnit which is usually derived from the method,
    /// class, field, property, or parameters name.  The test name specified by this
    /// attribute can contain 
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : PatternAttribute
    {
        private readonly string name;

        /// <summary>
        /// Overrides the name of a test or test parameter.
        /// </summary>
        /// <param name="name">The overridden name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public NameAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the overridden name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            Validate(scope, codeElement);

            scope.TestComponentBuilder.Name = name;
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="codeElement">The code element</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope scope, ICodeElementInfo codeElement)
        {
            if (!scope.IsTestDeclaration && !scope.IsTestParameterDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test or test parameter.");
        }
    }
}
