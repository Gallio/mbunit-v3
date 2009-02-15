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
using System.Globalization;
using System.Reflection;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// Represents a <see cref="PropertyInfo" /> whose native definition could not be resolved
    /// so we fall back on the <see cref="IPropertyInfo"/> wrapper.
    /// </summary>
    public sealed partial class UnresolvedPropertyInfo : PropertyInfo, IUnresolvedCodeElement
    {
        private readonly IPropertyInfo adapter;

        /// <summary>
        /// Creates a reflection object backed by the specified adapter.
        /// </summary>
        /// <param name="adapter">The adapter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="adapter"/> is null</exception>
        public UnresolvedPropertyInfo(IPropertyInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        /// <summary>
        /// Gets the underlying reflection adapter.
        /// </summary>
        public IPropertyInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        /// <inheritdoc />
        public override PropertyAttributes Attributes
        {
            get { return adapter.PropertyAttributes; }
        }

        /// <inheritdoc />
        public override bool CanRead
        {
            get { return adapter.GetMethod != null; }
        }

        /// <inheritdoc />
        public override bool CanWrite
        {
            get { return adapter.SetMethod != null; }
        }

        /// <inheritdoc />
        public override MemberTypes MemberType
        {
            get { return MemberTypes.Property; }
        }

        /// <inheritdoc />
        public override Type PropertyType
        {
            get { return adapter.ValueType.Resolve(false); }
        }

        private static void AddIfNotNull(ICollection<MethodInfo> collection, MethodInfo method)
        {
            if (method != null)
                collection.Add(method);
        }

        /// <inheritdoc />
        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            List<MethodInfo> methods = new List<MethodInfo>(2);
            AddIfNotNull(methods, GetGetMethod(nonPublic));
            AddIfNotNull(methods, GetSetMethod(nonPublic));
            return methods.ToArray();
        }

        /// <inheritdoc />
        public override object GetConstantValue()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.GetMethod, nonPublic);
        }

        /// <inheritdoc />
        public override ParameterInfo[] GetIndexParameters()
        {
            return UnresolvedMethodBase.ResolveParameters(adapter.IndexParameters);
        }

        /// <inheritdoc />
        public override Type[] GetOptionalCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override object GetRawConstantValue()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Type[] GetRequiredCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        /// <inheritdoc />
        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.SetMethod, nonPublic);
        }

        /// <inheritdoc />
        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override object GetValue(object obj, object[] index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetValue(object obj, object value, object[] index)
        {
            throw new NotSupportedException();
        }
    }
}