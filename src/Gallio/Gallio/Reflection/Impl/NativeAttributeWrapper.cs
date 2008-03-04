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

        public object[] InitializedArgumentValues
        {
            get { throw new NotSupportedException("Cannot get original constructor arguments of an Attribute object."); }
        }

        public object GetFieldValue(string name)
        {
            FieldInfo field = attrib.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (field != null && ReflectorAttributeUtils.IsAttributeField(Reflector.Wrap(field)))
                return field.GetValue(attrib);

            throw new ArgumentException(String.Format("The attribute does not have a writable instance field named '{0}'.", name));
        }

        public object GetPropertyValue(string name)
        {
            PropertyInfo property = attrib.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (property != null && ReflectorAttributeUtils.IsAttributeProperty(Reflector.Wrap(property)))
                return property.GetValue(attrib, null);

            throw new ArgumentException(String.Format("The attribute does not have a writable instance property named '{0}'.", name));
        }

        public IDictionary<IFieldInfo, object> InitializedFieldValues
        {
            get
            {
                Dictionary<IFieldInfo, object> values = new Dictionary<IFieldInfo, object>();

                foreach (FieldInfo field in attrib.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    IFieldInfo fieldInfo = Reflector.Wrap(field);
                    if (ReflectorAttributeUtils.IsAttributeField(fieldInfo))
                        values.Add(fieldInfo, field.GetValue(attrib));
                }

                return values;
            }
        }

        public IDictionary<IPropertyInfo, object> InitializedPropertyValues
        {
            get
            {
                Dictionary<IPropertyInfo, object> values = new Dictionary<IPropertyInfo, object>();

                foreach (PropertyInfo property in attrib.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    IPropertyInfo propertyInfo = Reflector.Wrap(property);
                    if (ReflectorAttributeUtils.IsAttributeProperty(propertyInfo))
                        values.Add(propertyInfo, property.GetValue(attrib, null));
                }

                return values;
            }
        }

        public object Resolve()
        {
            return attrib;
        }
    }
}