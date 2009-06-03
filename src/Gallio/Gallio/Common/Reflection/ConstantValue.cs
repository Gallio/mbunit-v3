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
using Gallio.Common.Reflection.Impl;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Specifies a typed constant value.
    /// </summary>
    public struct ConstantValue
    {
        private readonly ITypeInfo type;
        private readonly object value;

        /// <summary>
        /// Creates a constant value.
        /// </summary>
        /// <param name="type">The constant value type.</param>
        /// <param name="value">The constant value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public ConstantValue(ITypeInfo type, object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.type = type;
            this.value = value;
        }

        /// <summary>
        /// Creates a constant value that wraps an existing native value.
        /// </summary>
        /// <param name="value">The native value.</param>
        /// <returns>The constant.</returns>
        public static ConstantValue FromNative(object value)
        {
            Type valueType = value != null ? value.GetType() : typeof(object);
            return new ConstantValue(Reflector.Wrap(valueType), value);
        }

        /// <summary>
        /// Gets the type of the constant.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The type may be an array type, an enum type, a primitive type, or the <see cref="System.Type" /> type.
        /// </para>
        /// </remarks>
        public ITypeInfo Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <see cref="IsNative" /> is true, then this property contains the native value.
        /// Otherwise it is represented differently depending upon its type.
        /// <list type="bullet">
        /// <item>If the constant is a primitive value, then the value will be a primitive value such as an
        /// integer, string, float or null.</item>
        /// <item>If the constant is an enum, then the value will be a primitive value of the underlying type of the enum.</item>
        /// <item>If the constant is an array, then the value will be an array of <see cref="ConstantValue" />
        /// objects representing the elements of the array.</item>
        /// <item>If the constant is a type, then the value will be a <see cref="ITypeInfo" />.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public object Value
        {
            get { return value; }
        }

        /// <summary>
        /// Returns true if the contents of <see cref="Value" /> can be interpreted as a native value
        /// or false if <see cref="Resolve" /> must be used to resolve the constant to a native value.
        /// </summary>
        public bool IsNative
        {
            get
            {
                if (value == null)
                    return true;
                if (IsType)
                    return value is Type;
                if (IsEnum)
                    return value is Enum;
                if (IsArray)
                    return !(value is ConstantValue[]);
                return System.Type.GetTypeCode(value.GetType()) == type.TypeCode;
            }
        }

        /// <summary>
        /// Returns true if the value is an array.
        /// </summary>
        public bool IsArray
        {
            get { return type.IsArray; }
        }

        /// <summary>
        /// Returns true if the value is an enum.
        /// </summary>
        public bool IsEnum
        {
            get { return type.IsEnum; }
        }

        /// <summary>
        /// Returns true if the value is a type.
        /// </summary>
        public bool IsType
        {
            get { return type.FullName == @"System.Type"; }
        }

        /// <summary>
        /// Resolves the constant to a native value by replacing type references with actual <see cref="System.Type" />
        /// values, arrays of constants with <see cref="System.Array" /> values of the appropriate type,
        /// and enums with <see cref="System.Enum" /> values.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise the result may include unresolved types, enums or arrays.</param>
        /// <returns>The native constant value.</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the target cannot be resolved.</exception>
        public object Resolve(bool throwOnError)
        {
            if (IsNative)
                return value;

            if (IsType)
            {
                ITypeInfo typeInfo = (ITypeInfo) value;
                return typeInfo.Resolve(throwOnError);
            }

            if (IsArray)
            {
                ConstantValue[] arrayValues = (ConstantValue[]) value;
                Type arrayType = type.Resolve(throwOnError);
                Type elementType = arrayType.GetElementType();
                if (Reflector.IsUnresolved(elementType))
                    elementType = typeof(object);

                int length = arrayValues.Length;
                Array array = Array.CreateInstance(elementType, length);
                for (int i = 0; i < length; i++)
                    array.SetValue(arrayValues[i].Resolve(throwOnError), i);

                return array;
            }

            object underlyingValue = GetTypedPrimitiveValue();
            if (IsEnum)
            {
                Type enumType = type.Resolve(throwOnError);
                if (! Reflector.IsUnresolved(enumType))
                    return Enum.ToObject(enumType, underlyingValue);
            }

            return underlyingValue;
        }

        private object GetTypedPrimitiveValue()
        {
            switch (type.TypeCode)
            {
                case TypeCode.Boolean:
                    return (Boolean)value;
                case TypeCode.Byte:
                    return (Byte)value;
                case TypeCode.Char:
                    return (Char)value;
                case TypeCode.DateTime:
                    return (DateTime)value;
                case TypeCode.DBNull:
                    return DBNull.Value;
                case TypeCode.Decimal:
                    return (Decimal)value;
                case TypeCode.Double:
                    return (Double)value;
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return (Int16)value;
                case TypeCode.Int32:
                    return (Int32)value;
                case TypeCode.Int64:
                    return (Int64)value;
                case TypeCode.SByte:
                    return (SByte)value;
                case TypeCode.Single:
                    return (Single)value;
                case TypeCode.String:
                    return (String)value;
                case TypeCode.UInt16:
                    return (UInt16)value;
                case TypeCode.UInt32:
                    return (UInt32)value;
                case TypeCode.UInt64:
                    return (UInt64)value;
                default:
                    return value;
            }
        }
    }
}
