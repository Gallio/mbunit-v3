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
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using JetBrains.Metadata.Reader.API;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class MetadataAttributeWrapper : MetadataWrapper<IMetadataCustomAttribute>, IAttributeInfo
    {
        public MetadataAttributeWrapper(MetadataReflector reflector, IMetadataCustomAttribute target)
            : base(reflector, target)
        {
        }

        public ITypeInfo Type
        {
            get { return Reflector.WrapOpenType(Target.UsedConstructor.DeclaringType); }
        }

        public IConstructorInfo Constructor
        {
            get { return Reflector.WrapConstructor(Target.UsedConstructor); }
        }

        public object[] InitializedArgumentValues
        {
            get { return Target.ConstructorArguments; }
        }

        public object GetFieldValue(string name)
        {
            foreach (IMetadataCustomAttributeFieldInitialization initialization in Target.InitializedFields)
                if (initialization.Field.Name == name)
                    return ResolveObject(initialization.Value);

            foreach (IFieldInfo field in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.Name == name && ReflectorAttributeUtils.IsAttributeField(field))
                    return ReflectorTypeUtils.GetDefaultValue(field.ValueType);
            }

            throw new ArgumentException(String.Format("The attribute does not have a writable instance field named '{0}'.", name));
        }

        public object GetPropertyValue(string name)
        {
            foreach (IMetadataCustomAttributePropertyInitialization initialization in Target.InitializedProperties)
                if (initialization.Property.Name == name)
                    return ResolveObject(initialization.Value);

            foreach (IPropertyInfo property in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.Name == name && ReflectorAttributeUtils.IsAttributeProperty(property))
                    return ReflectorTypeUtils.GetDefaultValue(property.ValueType);
            }

            throw new ArgumentException(String.Format("The attribute does not have a writable instance property named '{0}'.", name));
        }

        public IDictionary<IFieldInfo, object> InitializedFieldValues
        {
            get
            {
                IMetadataCustomAttributeFieldInitialization[] initializations = Target.InitializedFields;
                Dictionary<IFieldInfo, object> values = new Dictionary<IFieldInfo, object>(initializations.Length);

                foreach (IMetadataCustomAttributeFieldInitialization initialization in initializations)
                    values.Add(Reflector.Wrap(initialization.Field), ResolveObject(initialization.Value));

                return values;
            }
        }

        public IDictionary<IPropertyInfo, object> InitializedPropertyValues
        {
            get
            {
                IMetadataCustomAttributePropertyInitialization[] initializations = Target.InitializedProperties;
                Dictionary<IPropertyInfo, object> values = new Dictionary<IPropertyInfo, object>(initializations.Length);

                foreach (IMetadataCustomAttributePropertyInitialization initialization in initializations)
                    values.Add(Reflector.Wrap(initialization.Property), ResolveObject(initialization.Value));

                return values;
            }
        }

        public object Resolve()
        {
            return ReflectorAttributeUtils.CreateAttribute(this);
        }

        private object ResolveObject(object value)
        {
            if (value != null)
            {
                IMetadataType type = value as IMetadataType;
                if (type != null)
                    return ResolveType(type);

                Type valueType = value.GetType();
                if (valueType.IsArray)
                {
                    Type elementType = valueType.GetElementType();

                    if (elementType == typeof(IMetadataType))
                        return GenericUtils.ConvertAllToArray<IMetadataType, Type>((IMetadataType[])value, ResolveType);

                    if (elementType == typeof(Object))
                        return GenericUtils.ConvertAllToArray<object, object>((object[])value, ResolveObject);
                }
            }
            
            return value;
        }

        private Type ResolveType(IMetadataType type)
        {
            return Reflector.Wrap(type).Resolve(false);
        }
    }
}