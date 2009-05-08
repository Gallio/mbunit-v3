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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Common.Collections;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> type wrapper that represents a special type
    /// that is either constructed from other types or derived from them as with a generic parameter.
    /// </summary>
    /// <seealso cref="StaticConstructedTypeWrapper"/>
    /// <seealso cref="StaticGenericParameterWrapper"/>
    public abstract class StaticSpecialTypeWrapper : StaticTypeWrapper
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        protected StaticSpecialTypeWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
        }

        /// <inheritdoc />
        public override StaticTypeSubstitution Substitution
        {
            get { return StaticTypeSubstitution.Empty; }
        }

        /// <inheritdoc />
        protected override ITypeInfo BaseTypeInternal
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override IList<ITypeInfo> Interfaces
        {
            get { return EmptyArray<ITypeInfo>.Instance; }
        }

        /// <inheritdoc />
        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            return EmptyArray<IConstructorInfo>.Instance;
        }

        /// <inheritdoc />
        public override IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
        {
            return null;
        }

        /// <inheritdoc />
        public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
        {
            return EmptyArray<IMethodInfo>.Instance;
        }

        /// <inheritdoc />
        public override IPropertyInfo GetProperty(string propertyName, BindingFlags bindingFlags)
        {
            return null;
        }

        /// <inheritdoc />
        public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
        {
            return EmptyArray<IPropertyInfo>.Instance;
        }

        /// <inheritdoc />
        public override IFieldInfo GetField(string fieldName, BindingFlags bindingFlags)
        {
            return null;
        }

        /// <inheritdoc />
        public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
        {
            return EmptyArray<IFieldInfo>.Instance;
        }

        /// <inheritdoc />
        public override IEventInfo GetEvent(string eventName, BindingFlags bindingFlags)
        {
            return null;
        }

        /// <inheritdoc />
        public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
        {
            return EmptyArray<IEventInfo>.Instance;
        }

        /// <inheritdoc />
        public override ITypeInfo GetNestedType(string nestedTypeName, BindingFlags bindingFlags)
        {
            return null;
        }

        /// <inheritdoc />
        public override IList<ITypeInfo> GetNestedTypes(BindingFlags bindingFlags)
        {
            return EmptyArray<ITypeInfo>.Instance;
        }

        /// <inheritdoc />
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            return EmptyArray<Attribute>.Instance;
        }
    }
}
