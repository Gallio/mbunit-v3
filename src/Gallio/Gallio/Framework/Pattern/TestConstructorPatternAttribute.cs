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
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Declares that a constructor is used to provide test fixture parameters.
    /// Subclasses of this attribute can control what happens with the constructor.
    /// </para>
    /// <para>
    /// At most one attribute of this type may appear on any given constructor.
    /// </para>
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestConstructor, AllowMultiple=false, Inherited=true)]
    public abstract class TestConstructorPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Gets a default instance of the constructor pattern attribute to use
        /// when no other pattern consumes a contructor.
        /// </summary>
        public static readonly TestConstructorPatternAttribute DefaultInstance = new DefaultImpl();

        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            IConstructorInfo constructor = codeElement as IConstructorInfo;
            Validate(containingScope, constructor);

            IPatternScope dataContextScope = containingScope.CreateChildTestDataContextScope(codeElement);

            InitializeDataContext(dataContextScope, constructor);
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="containingScope">The containing scope</param>
        /// <param name="constructor">The constructor</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly</exception>
        protected virtual void Validate(IPatternScope containingScope, IConstructorInfo constructor)
        {
            if (!containingScope.CanAddTestParameter || constructor == null)
                ThrowUsageErrorException("This attribute can only be used on a test type constructor.");
            if (constructor.IsStatic)
                ThrowUsageErrorException("This attribute cannot be used on a static constructor.");
        }

        /// <summary>
        /// <para>
        /// Initializes the <see cref="PatternTestDataContext" />.
        /// </para>
        /// </summary>
        /// <param name="dataContextScope">The data context scope</param>
        /// <param name="constructor">The constructor</param>
        protected virtual void InitializeDataContext(IPatternScope dataContextScope, IConstructorInfo constructor)
        {
            ITypeInfo declaringType = constructor.DeclaringType;
            if (declaringType.IsGenericTypeDefinition)
                dataContextScope.TestDataContextBuilder.ImplicitDataBindingIndexOffset = declaringType.GenericArguments.Count;

            foreach (IParameterInfo parameter in constructor.Parameters)
                dataContextScope.Consume(parameter, false, DefaultConstructorParameterPattern);

            dataContextScope.Process(constructor);
        }

        /// <summary>
        /// Gets the default pattern to apply to constructor parameters that do not have a primary pattern, or null if none.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <see cref="TestParameterPatternAttribute.DefaultInstance" />.
        /// </remarks>
        protected virtual IPattern DefaultConstructorParameterPattern
        {
            get { return TestParameterPatternAttribute.DefaultInstance; }
        }

        private sealed class DefaultImpl : TestConstructorPatternAttribute
        {
        }
    }
}
