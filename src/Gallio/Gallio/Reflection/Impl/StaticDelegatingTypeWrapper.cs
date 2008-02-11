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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> type wrapper that delegates most operations to another type.
    /// </summary>
    /// <seealso cref="StaticConstructedTypeWrapper"/>
    /// <seealso cref="StaticGenericParameterWrapper"/>
    public abstract class StaticDelegatingTypeWrapper : StaticTypeWrapper
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/> or <paramref name="handle"/> is null</exception>
        public StaticDelegatingTypeWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
        }

        /// <inheritdoc />
        public override StaticTypeSubstitution Substitution
        {
            get { return StaticTypeSubstitution.Empty; }
        }

        /// <inheritdoc />
        public override IAssemblyInfo Assembly
        {
            get { return EffectiveType.Assembly; }
        }

        /// <inheritdoc />
        public override INamespaceInfo Namespace
        {
            get { return EffectiveType.Namespace; }
        }

        /// <inheritdoc />
        public override ITypeInfo BaseType
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override TypeAttributes TypeAttributes
        {
            get { return EffectiveType.TypeAttributes; }
        }

        /// <inheritdoc />
        public override IList<ITypeInfo> Interfaces
        {
            get { return EffectiveType.Interfaces; }
        }

        /// <inheritdoc />
        public override IList<IConstructorInfo> GetConstructors(BindingFlags bindingFlags)
        {
            return EffectiveType.GetConstructors(bindingFlags);
        }

        /// <inheritdoc />
        public override IMethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
        {
            return EffectiveType.GetMethod(methodName, bindingFlags);
        }

        /// <inheritdoc />
        public override IList<IMethodInfo> GetMethods(BindingFlags bindingFlags)
        {
            return EffectiveType.GetMethods(bindingFlags);
        }

        /// <inheritdoc />
        public override IPropertyInfo GetProperty(string propertyName, BindingFlags bindingFlags)
        {
            return EffectiveType.GetProperty(propertyName, bindingFlags);
        }

        /// <inheritdoc />
        public override IList<IPropertyInfo> GetProperties(BindingFlags bindingFlags)
        {
            return EffectiveType.GetProperties(bindingFlags);
        }

        /// <inheritdoc />
        public override IFieldInfo GetField(string fieldName, BindingFlags bindingFlags)
        {
            return EffectiveType.GetField(fieldName, bindingFlags);
        }

        /// <inheritdoc />
        public override IList<IFieldInfo> GetFields(BindingFlags bindingFlags)
        {
            return EffectiveType.GetFields(bindingFlags);
        }

        /// <inheritdoc />
        public override IEventInfo GetEvent(string eventName, BindingFlags bindingFlags)
        {
            return EffectiveType.GetEvent(eventName, bindingFlags);
        }

        /// <inheritdoc />
        public override IList<IEventInfo> GetEvents(BindingFlags bindingFlags)
        {
            return EffectiveType.GetEvents(bindingFlags);
        }

        /// <summary>
        /// Gets the effective type that this type "looks like" when accessing members
        /// and structural properties of the type.  For example, the .Net <see cref="Type" /> class
        /// treats all array types as <see cref="Array" /> for most purposes.
        /// </summary>
        protected abstract ITypeInfo EffectiveType { get; }
    }
}
