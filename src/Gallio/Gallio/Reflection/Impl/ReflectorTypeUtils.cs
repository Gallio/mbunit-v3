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
using Gallio.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for working with types and type codes.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public class ReflectorTypeUtils
    {
        /// <summary>
        /// Determines the type code of a reflected type based on the
        /// type's name.
        /// </summary>
        /// <param name="type">The reflected type, or null if none</param>
        /// <returns>The type code of the type, or <see cref="TypeCode.Empty" />
        /// if <paramref name="type"/> was null</returns>
        public static TypeCode GetTypeCode(ITypeInfo type)
        {
            if (type == null)
                return TypeCode.Empty;

            switch (type.FullName)
            {
                case "System.Boolean":
                    return TypeCode.Boolean;
                case "System.Byte":
                    return TypeCode.Byte;
                case "System.Char":
                    return TypeCode.Char;
                case "System.DateTime":
                    return TypeCode.DateTime;
                case "System.DBNull":
                    return TypeCode.DBNull;
                case "System.Decimal":
                    return TypeCode.Decimal;
                case "System.Double":
                    return TypeCode.Double;
                case "System.Int16":
                    return TypeCode.Int16;
                case "System.Int32":
                    return TypeCode.Int32;
                case "System.Int64":
                    return TypeCode.Int64;
                case "System.SByte":
                    return TypeCode.SByte;
                case "System.Single":
                    return TypeCode.Single;
                case "System.String":
                    return TypeCode.String;
                case "System.UInt16":
                    return TypeCode.UInt16;
                case "System.UInt32":
                    return TypeCode.UInt32;
                case "System.UInt64":
                    return TypeCode.UInt64;
                default:
                    return TypeCode.Object;
            }
        }

        /// <summary>
        /// Gets the default value for a reflected type.
        /// </summary>
        /// <param name="type">The reflected type</param>
        /// <returns>The default value for the type, such as <c>0</c> if
        /// the type represents an integer, or <c>null</c> if the type
        /// is a reference type or if <paramref name="type"/> was null</returns>
        public static object GetDefaultValue(ITypeInfo type)
        {
            if (type == null)
                return null;

            return GetDefaultValue(type.TypeCode);
        }

        /// <summary>
        /// Gets the default value of a type with a given type code.
        /// </summary>
        /// <param name="typeCode">The type code</param>
        /// <returns>The default value of the type associated with the
        /// specified type code</returns>
        public static object GetDefaultValue(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return default(Boolean);
                case TypeCode.Byte:
                    return default(Byte);
                case TypeCode.Char:
                    return default(Char);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.DBNull:
                    return default(DBNull);
                case TypeCode.Decimal:
                    return default(Decimal);
                case TypeCode.Double:
                    return default(Double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return default(Int16);
                case TypeCode.Int32:
                    return default(Int32);
                case TypeCode.Int64:
                    return default(Int64);
                case TypeCode.Object:
                    return default(Object);
                case TypeCode.SByte:
                    return default(SByte);
                case TypeCode.Single:
                    return default(Single);
                case TypeCode.String:
                    return default(String);
                case TypeCode.UInt16:
                    return default(UInt16);
                case TypeCode.UInt32:
                    return default(UInt32);
                case TypeCode.UInt64:
                    return default(UInt64);
                default:
                    throw new NotSupportedException("TypeCode not supported.");
            }
        }
    }
}