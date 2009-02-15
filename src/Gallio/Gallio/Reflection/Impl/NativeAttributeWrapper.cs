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

namespace Gallio.Reflection.Impl
{
    // TODO: We could wrap CustomAttributeData to get the positional and
    //       named parameter information.
    internal sealed class NativeAttributeWrapper : IAttributeInfo
    {
        private readonly Attribute attrib;

        public NativeAttributeWrapper(Attribute attrib)
        {
            if (attrib == null)
                throw new ArgumentNullException("attrib");

            this.attrib = attrib;
        }

        public ITypeInfo Type
        {
            get { return Reflector.Wrap(attrib.GetType()); }
        }

        public IConstructorInfo Constructor
        {
            get { throw new NotSupportedException("Cannot get original constructor of an Attribute object."); }
        }

        public ConstantValue[] InitializedArgumentValues
        {
            get { throw new NotSupportedException("Cannot get original constructor arguments of an Attribute object."); }
        }

        public ConstantValue GetFieldValue(string name)
        {
            FieldInfo field = attrib.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (field != null && ReflectorAttributeUtils.IsAttributeField(Reflector.Wrap(field)))
                return ConstantValue.FromNative(field.GetValue(attrib));

            throw new ArgumentException(String.Format("The attribute does not have a writable instance field named '{0}'.", name));
        }

        public ConstantValue GetPropertyValue(string name)
        {
            PropertyInfo property = attrib.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (property != null && ReflectorAttributeUtils.IsAttributeProperty(Reflector.Wrap(property)))
                return ConstantValue.FromNative(property.GetValue(attrib, null));

            throw new ArgumentException(String.Format("The attribute does not have a writable instance property named '{0}'.", name));
        }

        public IEnumerable<KeyValuePair<IFieldInfo, ConstantValue>> InitializedFieldValues
        {
            get
            {
                foreach (FieldInfo field in attrib.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    IFieldInfo fieldInfo = Reflector.Wrap(field);
                    if (ReflectorAttributeUtils.IsAttributeField(fieldInfo))
                        yield return new KeyValuePair<IFieldInfo, ConstantValue>(fieldInfo, ConstantValue.FromNative(field.GetValue(attrib)));
                }
            }
        }

        public IEnumerable<KeyValuePair<IPropertyInfo, ConstantValue>> InitializedPropertyValues
        {
            get
            {
                foreach (PropertyInfo property in attrib.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    IPropertyInfo propertyInfo = Reflector.Wrap(property);
                    if (ReflectorAttributeUtils.IsAttributeProperty(propertyInfo))
                        yield return new KeyValuePair<IPropertyInfo, ConstantValue>(propertyInfo, ConstantValue.FromNative(property.GetValue(attrib, null)));
                }
            }
        }

        public object Resolve(bool throwOnError)
        {
            return attrib;
        }
    }
}