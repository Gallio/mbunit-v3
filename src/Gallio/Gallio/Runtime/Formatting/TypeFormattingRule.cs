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
using System.Text;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="Type" />.
    /// </para>
    /// <para>
    /// Formats values like: string, MyType.Nested, int[], byte*, similar to C#.
    /// </para>
    /// </summary>
    public sealed class TypeFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (typeof(Type).IsAssignableFrom(type))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            Type value = (Type) obj;
            StringBuilder result = new StringBuilder();
            AppendType(result, value);
            return result.ToString();
        }

        private static void AppendType(StringBuilder result, Type type)
        {
            if (type.HasElementType)
            {
                AppendType(result, type.GetElementType());

                if (type.IsArray)
                {
                    int rank = type.GetArrayRank();
                    result.Append('[');
                    result.Append(',', rank - 1);
                    result.Append(']');
                }
                else if (type.IsPointer)
                {
                    result.Append('*');
                }
                else
                {
                    result.Append('&');
                }

                return;
            }

            string typeName = GetBuiltInTypeName(type);
            if (typeName == null)
            {
                if (type.IsNested)
                {
                    AppendType(result, type.DeclaringType);
                    result.Append('.');
                }
                else
                {
                    string typeNamespace = type.Namespace;
                    if (!string.IsNullOrEmpty(typeNamespace))
                    {
                        result.Append(typeNamespace);
                        result.Append('.');
                    }
                }

                typeName = type.Name;
            }

            result.Append(typeName);

            if (type.IsGenericType)
            {
                // strip off the `# part of a generic type name like Dictionary`2
                int backTickPos = typeName.IndexOf('`');
                if (backTickPos >= 0)
                    result.Length -= typeName.Length - backTickPos;

                result.Append('<');

                Type[] typeParams = type.GetGenericArguments();
                for (int i = 0; i < typeParams.Length; i++)
                {
                    if (i != 0)
                        result.Append(',');
                    if (! typeParams[i].IsGenericParameter)
                        AppendType(result, typeParams[i]);
                }

                result.Append('>');
            }
        }

        private static string GetBuiltInTypeName(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return "bool";
                case TypeCode.Char:
                    return "char";
                case TypeCode.SByte:
                    return "sbyte";
                case TypeCode.Byte:
                    return "byte";
                case TypeCode.Int16:
                    return "short";
                case TypeCode.UInt16:
                    return "ushort";
                case TypeCode.Int32:
                    return "int";
                case TypeCode.UInt32:
                    return "uint";
                case TypeCode.Int64:
                    return "long";
                case TypeCode.UInt64:
                    return "ulong";
                case TypeCode.Single:
                    return "float";
                case TypeCode.Double:
                    return "double";
                case TypeCode.Decimal:
                    return "decimal";
                case TypeCode.String:
                    return "string";
                default:
                    if (type == typeof(Object))
                        return "object";
                    return null;
            }
        }
    }
}