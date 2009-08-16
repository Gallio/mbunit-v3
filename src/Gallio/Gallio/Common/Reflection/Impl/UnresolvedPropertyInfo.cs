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
using Gallio.Common.Collections;

#if DOTNET40
namespace Gallio.Common.Reflection.Impl.DotNet40
#else
namespace Gallio.Common.Reflection.Impl.DotNet20
#endif
{
    internal sealed partial class UnresolvedPropertyInfo : PropertyInfo, IUnresolvedCodeElement
    {
        private readonly IPropertyInfo adapter;

        internal UnresolvedPropertyInfo(IPropertyInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        public IPropertyInfo Adapter
        {
            get { return adapter; }
        }

        ICodeElementInfo IUnresolvedCodeElement.Adapter
        {
            get { return adapter; }
        }

        public override PropertyAttributes Attributes
        {
            get { return adapter.PropertyAttributes; }
        }

        public override bool CanRead
        {
            get { return adapter.GetMethod != null; }
        }

        public override bool CanWrite
        {
            get { return adapter.SetMethod != null; }
        }

        public override MemberTypes MemberType
        {
            get { return MemberTypes.Property; }
        }

        public override Type PropertyType
        {
            get { return adapter.ValueType.Resolve(false); }
        }

        private static void AddIfNotNull(ICollection<MethodInfo> collection, MethodInfo method)
        {
            if (method != null)
                collection.Add(method);
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            List<MethodInfo> methods = new List<MethodInfo>(2);
            AddIfNotNull(methods, GetGetMethod(nonPublic));
            AddIfNotNull(methods, GetSetMethod(nonPublic));
            return methods.ToArray();
        }

        public override object GetConstantValue()
        {
            throw new NotSupportedException("Cannot get constant value of unresolved property.");
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.GetMethod, nonPublic);
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return UnresolvedMethodBase.ResolveParameters(adapter.IndexParameters);
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        public override object GetRawConstantValue()
        {
            throw new NotSupportedException("Cannot get constant value of unresolved property.");
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return EmptyArray<Type>.Instance;
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return UnresolvedMemberInfo.ResolveMethod(adapter.SetMethod, nonPublic);
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index,
            CultureInfo culture)
        {
            throw new NotSupportedException("Cannot get value of unresolved property.");
        }

        public override object GetValue(object obj, object[] index)
        {
            throw new NotSupportedException("Cannot get value of unresolved property.");
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index,
            CultureInfo culture)
        {
            throw new NotSupportedException("Cannot set value of unresolved property.");
        }

        public override void SetValue(object obj, object value, object[] index)
        {
            throw new NotSupportedException("Cannot set value of unresolved property.");
        }

        #region .Net 4.0 Only
#if DOTNET40
#endif
        #endregion
    }
}