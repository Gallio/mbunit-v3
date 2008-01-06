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
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection.Impl
{
    internal sealed class PsiAttributeWrapper : PsiWrapper<IAttributeInstance>, IAttributeInfo
    {
        public PsiAttributeWrapper(PsiReflector reflector, IAttributeInstance target)
            : base(reflector, target)
        {
        }

        public ITypeInfo Type
        {
            get { return Reflector.Wrap(Target.AttributeType, true); }
        }

        public IConstructorInfo Constructor
        {
            get { return Reflector.Wrap(Target.Constructor); }
        }

        public object[] InitializedArgumentValues
        {
            get
            {
                IList<IParameter> parameters = Target.Constructor.Parameters;
                if (parameters.Count == 0)
                    return EmptyArray<object>.Instance;

                List<object> values = new List<object>();
                for (int i = 0; ; i++)
                {
                    ConstantValue2 rawValue = Target.PositionParameter(i);
                    if (rawValue.IsBadValue())
                        break;

                    values.Add(ResolveObject(rawValue.Value));
                }

                int lastParameterIndex = parameters.Count - 1;
                if (! parameters[lastParameterIndex].IsParameterArray)
                    return values.ToArray();

                // Note: When presented with a constructor that accepts a variable number of
                //       arguments, ReSharper treats them as a sequence of normal parameter
                //       values.  So we we need to map them back into a params array appropriately.                
                object[] args = new object[parameters.Count];
                values.CopyTo(0, args, 0, lastParameterIndex);

                int varArgsCount = values.Count - lastParameterIndex;
                object[] varArgs = new object[varArgsCount];
                values.CopyTo(lastParameterIndex, varArgs, 0, varArgsCount);

                args[lastParameterIndex] = varArgs;
                return args;
            }
        }

        public object GetFieldValue(string name)
        {
            foreach (PsiFieldWrapper field in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.Name == name && ReflectorAttributeUtils.IsAttributeField(field))
                {
                    ConstantValue2 value = Target.NamedParameter(field.Target);
                    return GetValueOrDefault(value, field);
                }
            }

            throw new ArgumentException(String.Format("The attribute does not have a writable instance field named '{0}'.", name));
        }

        public object GetPropertyValue(string name)
        {
            foreach (PsiPropertyWrapper property in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.Name == name && ReflectorAttributeUtils.IsAttributeProperty(property))
                {
                    ConstantValue2 value = Target.NamedParameter(property.Target);
                    return GetValueOrDefault(value, property);
                }
            }

            throw new ArgumentException(String.Format("The attribute does not have a writable instance property named '{0}'.", name));
        }


        public IDictionary<IFieldInfo, object> InitializedFieldValues
        {
            get
            {
                Dictionary<IFieldInfo, object> values = new Dictionary<IFieldInfo, object>();

                foreach (PsiFieldWrapper field in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ReflectorAttributeUtils.IsAttributeField(field))
                    {
                        ConstantValue2 value = Target.NamedParameter(field.Target);
                        if (!value.IsBadValue())
                            values.Add(field, ResolveObject(value.Value));
                    }
                }

                return values;
            }
        }

        public IDictionary<IPropertyInfo, object> InitializedPropertyValues
        {
            get
            {
                Dictionary<IPropertyInfo, object> values = new Dictionary<IPropertyInfo, object>();

                foreach (PsiPropertyWrapper property in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ReflectorAttributeUtils.IsAttributeProperty(property))
                    {
                        ConstantValue2 value = Target.NamedParameter(property.Target);
                        if (!value.IsBadValue())
                            values.Add(property, ResolveObject(value.Value));
                    }
                }

                return values;
            }
        }

        public object Resolve()
        {
            return ReflectorAttributeUtils.CreateAttribute(this);
        }

        private object GetValueOrDefault(ConstantValue2 value, ISlotInfo slot)
        {
            if (value.IsBadValue())
                return ReflectorTypeUtils.GetDefaultValue(slot.ValueType);
            return ResolveObject(value.Value);
        }

        private object ResolveObject(object value)
        {
            if (value != null)
            {
                IType type = value as IType;
                if (type != null)
                    return ResolveType(type);

                // TODO: It's not clear to me that the PSI internal implementation is complete!
                //       I found a special case for mapping types but nothing for arrays.
                //       So I've omitted the array code from here for now.  -- Jeff.
            }

            return value;
        }

        private Type ResolveType(IType type)
        {
            return Reflector.Wrap(type, false).Resolve();
        }
    }
}