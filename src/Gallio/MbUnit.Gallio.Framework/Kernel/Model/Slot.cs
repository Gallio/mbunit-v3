// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Reflection;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A slot represents a field, property or parameter.  It is used to
    /// simplify the handling of data binding.
    /// </summary>
    public sealed class Slot
    {
        private readonly ICustomAttributeProvider attributeProvider;

        /// <summary>
        /// Initializes a slot from a field.
        /// </summary>
        /// <param name="field">The field</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="field"/> is null</exception>
        public Slot(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException(@"field");

            this.attributeProvider = field;
        }

        /// <summary>
        /// Initializes a slot from a property.
        /// </summary>
        /// <param name="property">The property</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="property"/> is null</exception>
        public Slot(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(@"property");

            this.attributeProvider = property;
        }

        /// <summary>
        /// Initializes a slot from a parameter.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null</exception>
        public Slot(ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(@"parameter");

            this.attributeProvider = parameter;
        }

        /// <summary>
        /// Gets the custom attribute provider for the associated field, property or parameter, non-null.
        /// </summary>
        public ICustomAttributeProvider AttributeProvider
        {
            get { return attributeProvider; }
        }

        /// <summary>
        /// Gets the associated field, or null if not applicable.
        /// </summary>
        public FieldInfo Field
        {
            get { return attributeProvider as FieldInfo; }
        }

        /// <summary>
        /// Gets the associated property, or null if not applicable.
        /// </summary>
        public PropertyInfo Property
        {
            get { return attributeProvider as PropertyInfo; }
        }

        /// <summary>
        /// Gets the associated parameter, or null if not applicable.
        /// </summary>
        public ParameterInfo Parameter
        {
            get { return attributeProvider as ParameterInfo; }
        }

        /// <summary>
        /// Gets the name of the slot.
        /// </summary>
        public string Name
        {
            get
            {
                MemberInfo member = attributeProvider as MemberInfo;
                if (member != null)
                    return member.Name;

                return Parameter.Name;
            }
        }

        /// <summary>
        /// Gets the type of value held in the slot.
        /// </summary>
        public Type ValueType
        {
            get
            {
                FieldInfo field = Field;
                if (field != null)
                    return field.FieldType;

                PropertyInfo property = Property;
                if (property != null)
                    return property.PropertyType;

                return Parameter.ParameterType;
            }
        }

        /// <summary>
        /// Gets the positional index of a parameter slot, or 0 in other cases.
        /// </summary>
        public int Position
        {
            get
            {
                ParameterInfo parameter = Parameter;
                return parameter != null ? parameter.Position : 0;
            }
        }

        /// <summary>
        /// Gets the code reference for the slot.
        /// </summary>
        public CodeReference CodeReference
        {
            get
            {
                MemberInfo member = attributeProvider as MemberInfo;
                if (member != null)
                    return CodeReference.CreateFromMember(member);

                return CodeReference.CreateFromParameter(Parameter);
            }
        }
    }
}
