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
using System.Globalization;
using System.Reflection;
using Gallio.Collections;
using Gallio.Properties;

namespace Gallio.Reflection
{
    /// <summary>
    /// Provides functions for working with reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Gets the assembly that declares the code element, or the
        /// code element itself if it is an <see cref="IAssemblyInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The assembly, or null if not found</returns>
        public static IAssemblyInfo GetAssembly(ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = codeElement as IAssemblyInfo;
            if (assembly != null)
                return assembly;

            ITypeInfo type = GetType(codeElement);
            if (type != null)
                return type.Assembly;

            return null;
        }

        /// <summary>
        /// Gets the namespace that declares the code element, or the
        /// code element itself if it is an <see cref="INamespaceInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The namespace, or null if not found</returns>
        public static INamespaceInfo GetNamespace(ICodeElementInfo codeElement)
        {
            INamespaceInfo @namespace = codeElement as INamespaceInfo;
            if (@namespace != null)
                return @namespace;

            ITypeInfo type = GetType(codeElement);
            if (type != null)
                return type.Namespace;

            return null;
        }

        /// <summary>
        /// Gets the type that declares the code element, or the
        /// code element itself if it is an <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The type, or null if not found</returns>
        public static ITypeInfo GetType(ICodeElementInfo codeElement)
        {
            ITypeInfo type = codeElement as ITypeInfo;
            if (type != null)
                return type;

            IMemberInfo member = GetMember(codeElement);
            if (member != null)
                return member.DeclaringType;

            return null;
        }

        /// <summary>
        /// Gets the non-type member that declares the code element, or the
        /// code element itself if it is an <see cref="IMemberInfo"/>
        /// other than a <see cref="ITypeInfo" />.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The member, or null if not found</returns>
        public static IMemberInfo GetMember(ICodeElementInfo codeElement)
        {
            IMemberInfo member = codeElement as IMemberInfo;
            if (member != null)
                return member is ITypeInfo ? null : member;

            IParameterInfo parameter = GetParameter(codeElement);
            if (parameter != null)
                return parameter.Member;

            return null;
        }

        /// <summary>
        /// Gets the parameter that declares the code element, or the
        /// code element itself if it is an <see cref="IParameterInfo"/>.
        /// </summary>
        /// <param name="codeElement">The code element, possibly null</param>
        /// <returns>The parameter, or null if not found</returns>
        public static IParameterInfo GetParameter(ICodeElementInfo codeElement)
        {
            IParameterInfo type = codeElement as IParameterInfo;
            if (type != null)
                return type;

            return null;
        }

        /// <summary>
        /// Checks that the method has the specified signature otherwise throws a <see cref="InvalidOperationException" />.
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="signature">The list of parameter types (all input parameters)</param>
        /// <exception cref="InvalidOperationException">Thrown if the method has a different signature</exception>
        public static void CheckMethodSignature(IMethodInfo method, params ITypeInfo[] signature)
        {
            IList<IParameterInfo> parameters = method.Parameters;
            if (parameters.Count == signature.Length)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    IParameterInfo parameter = parameters[i];
                    if (parameter.ValueType != signature[i]
                        || (parameter.ParameterAttributes & (ParameterAttributes.In | ParameterAttributes.Out)) != ParameterAttributes.In)
                        goto Fail;
                }

                return;
            }

        Fail:
            string[] expectedTypeNames = Array.ConvertAll<ITypeInfo, string>(signature, delegate(ITypeInfo parameterType)
            {
                return parameterType.FullName;
            });
            string[] actualTypeNames = GenericUtils.ConvertAllToArray<IParameterInfo, string>(parameters, delegate(IParameterInfo parameter)
            {
                if ((parameter.ParameterAttributes & ParameterAttributes.Out) != 0)
                {
                    string prefix = (parameter.ParameterAttributes & ParameterAttributes.In) != 0 ? @"ref " : @"out ";
                    return prefix + parameter.ValueType.FullName;
                }

                return parameter.ValueType.FullName;
            });

            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                Resources.ModelUtils_InvalidSignature,
                string.Join(@", ", expectedTypeNames),
                string.Join(@", ", actualTypeNames)));
        }

        /// <summary>
        /// Finds the assembly name of the directly referenced assembly with the specified display name.
        /// </summary>
        /// <param name="assembly">The assembly to search</param>
        /// <param name="displayName">The display name of the referenced assembly to find</param>
        /// <returns>The referenced assembly name or null if none</returns>
        public static AssemblyName FindAssemblyReference(IAssemblyInfo assembly, string displayName)
        {
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (reference.Name == displayName)
                    return reference;
            }

            return null;
        }

        /// <summary>
        /// Determines if the type can be instantiated using a public constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type can be instantiated</returns>
        public static bool CanInstantiate(ITypeInfo type)
        {
            return type != null
                && (type.TypeAttributes & (TypeAttributes.Abstract | TypeAttributes.Class | TypeAttributes.Public)) == (TypeAttributes.Class | TypeAttributes.Public)
                && type.ElementType == null
                && type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Count != 0;
        }

        /// <summary>
        /// Determines if the method is public, non-static and is non-abstract so it can be invoked.
        /// </summary>
        /// <param name="method">The method</param>
        /// <returns>True if the method can be invoked</returns>
        public static bool CanInvokeNonStatic(IMethodInfo method)
        {
            return method != null
                && (method.MethodAttributes & (MethodAttributes.Abstract | MethodAttributes.Public | MethodAttributes.Static)) == MethodAttributes.Public;
        }

        /// <summary>
        /// Determines if the property has public, non-static and non-abstract getter
        /// and setter functions.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>True if the property can be get and set</returns>
        public static bool CanGetAndSetNonStatic(IPropertyInfo property)
        {
            return property != null
                && CanInvokeNonStatic(property.GetMethod)
                && CanInvokeNonStatic(property.SetMethod);
        }

        /// <summary>
        /// Returns true if a type is derived from another type with the specified qualified name.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="qualifiedTypeName">The qualified type name</param>
        /// <returns>True if <paramref name="type"/> is derived from <paramref name="qualifiedTypeName"/></returns>
        public static bool IsDerivedFrom(ITypeInfo type, string qualifiedTypeName)
        {
            for (ITypeInfo superType = type; superType != null; superType = superType.BaseType)
            {
                if (superType.FullName == qualifiedTypeName)
                    return true;
            }

            foreach (ITypeInfo interfaceType in type.Interfaces)
            {
                if (interfaceType.FullName == qualifiedTypeName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The default value for the type, such as <c>0</c> if
        /// the type represents an integer, or <c>null</c> if the type
        /// is a reference type or if <paramref name="type"/> was null</returns>
        public static object GetDefaultValue(Type type)
        {
            return GetDefaultValue(Type.GetTypeCode(type));
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
