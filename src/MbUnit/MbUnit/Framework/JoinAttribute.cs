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

using System;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// An abstract class for attributes that specify the join strategy of a test.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You may define a new join strategy by creating a subclass of this attribute
    /// and overriding <see cref="GetJoinStrategy" />.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public abstract class JoinAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            Validate(scope, codeElement);

            IJoinStrategy strategy = GetJoinStrategy();
            scope.TestBuilder.TestActions.BeforeTestChain.Before(delegate(PatternTestState state)
            {
                state.BindingContext.Strategy = strategy;
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope</param>
        /// <param name="codeElement">The code element</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope scope, ICodeElementInfo codeElement)
        {
            if (!scope.IsTestDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test.");
        }

        /// <summary>
        /// Gets the join strategy to use.
        /// </summary>
        /// <returns>The join strategy</returns>
        protected abstract IJoinStrategy GetJoinStrategy();        
    }
}
