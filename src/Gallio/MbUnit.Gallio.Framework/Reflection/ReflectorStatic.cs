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
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace MbUnit.Framework.Reflection
{
    public partial class Reflector
    {
        /// <summary>
        /// Create Instance
        /// </summary>
        /// <param name="assemblyName">Full assembly path.</param>
        /// <param name="className">Type Name such as (System.String)</param>
        /// <returns>Newly created object.</returns>
        public static object CreateInstance(string assemblyName, string typeName)
        {
            return CreateInstance(assemblyName, typeName, null);
        }

        /// <summary>
        /// Create Instance
        /// </summary>
        /// <param name="assemblyName">Full assembly path.</param>
        /// <param name="className">Type Name such as (System.String)</param>
        /// <param name="args">Constructor parameters.</param>
        /// <returns>Newly created object.</returns>
        public static object CreateInstance(string assemblyName, string typeName, params object[] args)
        {
            object obj = null;
            Type[] argTypes = Type.EmptyTypes;
            Assembly a = Assembly.Load(assemblyName);
            Type type = a.GetType(typeName);
            if (args != null)
            {
                argTypes = new Type[args.Length];
                for (int ndx = 0; ndx < args.Length; ndx++)
                    argTypes[ndx] = (args[ndx] == null) ? typeof(object) : args[ndx].GetType();
            }
            ConstructorInfo ci = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                , null, argTypes, null);
            obj = ci.Invoke(args);
            return obj;
        }

        #region Get/Set Fields

        #region Get
        /// <summary>
        /// Get public, non-public, or static field value.
        /// </summary>
        /// <param name="obj">Object where field is defined.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Field value</returns>
        public static object GetField(object obj, string fieldName)
        {
            return GetField(AccessModifier.Default, obj, fieldName, true);
        }

        /// <summary>
        /// Get field value.
        /// </summary>
        /// <param name="access">Specify field access modifier.</param>
        /// <param name="obj">Object where field is defined.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Field value</returns>
        public static object GetField(AccessModifier access, object obj, string fieldName)
        {
            return GetField(access, obj, fieldName, true);
        }

        /// <summary>
        /// Get field value.
        /// </summary>
        /// <param name="access">Specify field access modifier.</param>
        /// <param name="obj">Object where field is defined.</param>
        /// <param name="fieldName">Field name.</param>
        /// <param name="lookInBase">Specify if need to look in Base classes.</param>
        /// <returns>Field value</returns>
        public static object GetField(AccessModifier access, object obj, string fieldName, bool lookInBase)
        {
            CheckObject(obj);
            FieldInfo fi = GetField(access, obj.GetType(), fieldName, lookInBase);
            IsMember(obj, fi, fieldName, MemberType.Field);
            return fi.GetValue(obj);
        }

        private static FieldInfo GetField(AccessModifier access, Type type, string fieldName, bool lookInBase)
        {
            FieldInfo fi = type.GetField(fieldName, BindingFlags.Instance | (BindingFlags)access);
            if (lookInBase && fi == null && !type.Equals(typeof(Object)))
                return GetField(access, type.BaseType, fieldName, lookInBase);
            else
                return fi;
        }
        #endregion

        #region Set
        /// <summary>
        /// Set field value.
        /// </summary>
        /// <param name="obj">Object where field is defined.</param>
        /// <param name="fieldName">Field Name.</param>
        /// <param name="fieldValue">Field Value.</param>
        public static void SetField(object obj, string fieldName, object fieldValue)
        {
            SetField(AccessModifier.Default, obj, fieldName, fieldValue, true);
        }

        /// <summary>
        /// Set field value.
        /// </summary>
        /// <param name="access">Specify field access modifier.</param>
        /// <param name="obj">Object where field is defined.</param>
        /// <param name="fieldName">Field Name.</param>
        /// <param name="fieldValue">Field Value.</param>
        public static void SetField(AccessModifier access, object obj, string fieldName, object fieldValue)
        {
            SetField(access, obj, fieldName, fieldValue, true);
        }

        /// <summary>
        /// Set field value.
        /// </summary>
        /// <param name="access">Specify field access modifier.</param>
        /// <param name="obj">Object where field is defined.</param>
        /// <param name="fieldName">Field Name.</param>
        /// <param name="fieldValue">Field Value.</param>
        /// <param name="lookInBase">Specify if need to look in Base classes.</param>
        public static void SetField(AccessModifier access, object obj, string fieldName, object fieldValue, bool lookInBase)
        {
            CheckObject(obj);
            FieldInfo fi = GetField(access, obj.GetType(), fieldName, lookInBase);
            IsMember(obj, fi, fieldName, MemberType.Field);
            fi.SetValue(obj, fieldValue);
        }
        #endregion

        #endregion

        #region Get/Set Properties

        #region Get
        /// <summary>
        /// Get Property Value
        /// </summary>
        /// <param name="obj">Object where property is defined.</param>
        /// <param name="propertyName">Property Name.</param>
        /// <returns>Property Value.</returns>
        public static object GetProperty(object obj, string propertyName)
        {
            return GetProperty(AccessModifier.Default, obj, propertyName, true);
        }

        /// <summary>
        /// Get Property Value
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="propertyName">Property Name.</param>
        /// <returns>Property Value.</returns>
        public static object GetProperty(AccessModifier access, object obj, string propertyName)
        {
            return GetProperty(access, obj, propertyName, true);
        }

        /// <summary>
        /// Get Property Value
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="propertyName">Property Name.</param>
        /// <param name="lookInBase">Set to true if need look for the property in base classes.</param>
        /// <returns>Property Value.</returns>
        public static object GetProperty(AccessModifier access, object obj, string propertyName, bool lookInBase)
        {
            CheckObject(obj);
            PropertyInfo pi = GetProperty(access, obj.GetType(), propertyName, lookInBase);
            IsMember(obj, pi, propertyName, MemberType.Property);
            return pi.GetValue(obj, null);
        }

        private static PropertyInfo GetProperty(AccessModifier access, Type type, string fieldName, bool lookInBase)
        {
            PropertyInfo pi = type.GetProperty(fieldName, BindingFlags.Instance | (BindingFlags)access);
            if (lookInBase && pi == null && !type.Equals(typeof(Object)))
                return GetProperty(access, type.BaseType, fieldName, lookInBase);
            else
                return pi;
        }
        #endregion

        #region Set
        /// <summary>
        /// Set Property value.
        /// </summary>
        /// <param name="obj">Object where property is defined.</param>
        /// <param name="fieldName">Property Name.</param>
        /// <param name="fieldValue">Property Value.</param>
        public static void SetProperty(object obj, string propertyName, object propertyValue)
        {
            SetProperty(AccessModifier.Default, obj, propertyName, propertyValue);
        }

        /// <summary>
        /// Set Property value.
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="obj">Object where property is defined.</param>
        /// <param name="fieldName">Property Name.</param>
        /// <param name="fieldValue">Property Value.</param>
        public static void SetProperty(AccessModifier access, object obj, string propertyName, object propertyValue)
        {
            SetProperty(access, obj, propertyName, propertyValue, true);
        }

        /// <summary>
        /// Set Property value.
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="obj">Object where property is defined.</param>
        /// <param name="fieldName">Property Name.</param>
        /// <param name="fieldValue">Property Value.</param>
        /// <param name="lookInBase">Set to true if need look for the property in base classes.</param>
        public static void SetProperty(AccessModifier access, object obj, string propertyName, object propertyValue, bool lookInBase)
        {
            CheckObject(obj);
            PropertyInfo pi = GetProperty(access, obj.GetType(), propertyName, lookInBase);
            IsMember(obj, pi, propertyName, MemberType.Property);
            pi.SetValue(obj, propertyValue, null);
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="obj">Object where method is defined.</param>
        /// <param name="methodName">Method to call</param>
        /// <returns>The object the method should return.</returns>
        public static object InvokeMethod(object obj, string methodName)
        {
            return InvokeMethod(AccessModifier.Default, obj, methodName, true, null);
        }

        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="obj">Object where method is defined.</param>
        /// <param name="methodName">Method to call</param>
        /// <param name="methodParams">Method's parameters.</param>
        /// <returns>The object the method should return.</returns>
        public static object InvokeMethod(object obj, string methodName, params object[] methodParams)
        {
            return InvokeMethod(AccessModifier.Default, obj, methodName, methodParams);
        }

        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="access">Specify method access modifier.</param>
        /// <param name="obj">Object where method is defined.</param>
        /// <param name="methodName">Method to call</param>
        /// <param name="methodParams">Method's parameters.</param>
        /// <returns>The object the method should return.</returns>
        public static object InvokeMethod(AccessModifier access, object obj, string methodName, params object[] methodParams)
        {
            return InvokeMethod(access, obj, methodName, true, methodParams);
        }

        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="access">Specify method access modifier.</param>
        /// <param name="obj">Object where method is defined.</param>
        /// <param name="methodName">Method to call</param>
        /// <param name="methodParams">Method's parameters.</param>
        /// <returns>The object the method should return.</returns>
        public static object InvokeMethod(AccessModifier access, object obj, string methodName, bool lookInBase, params object[] methodParams)
        {
            CheckObject(obj);
            MethodInfo mi = GetMethod(access, obj.GetType(), methodName, lookInBase, methodParams);
            IsMember(obj, mi, methodName, MemberType.Method);
            return mi.Invoke(obj, methodParams);
        }

        private static MethodInfo GetMethod(AccessModifier access, Type type, string methodName, bool lookInBase, params object[] methodParams)
        {

            Type[] paramTypes = null;
            if (methodParams != null)
            {
                paramTypes = new Type[methodParams.Length];

                for (int ndx = 0; ndx < methodParams.Length; ndx++)
                    paramTypes[ndx] = methodParams[ndx].GetType();
            }
            else
                paramTypes = new Type[0];

            MethodInfo mi = type.GetMethod(methodName, BindingFlags.Instance | (BindingFlags)access, null, paramTypes, null);
            if (lookInBase && mi == null && !type.Equals(typeof(Object)))
                return GetMethod(access, type.BaseType, methodName, lookInBase, methodParams);
            else
                return mi;
        }

        #endregion

        #region Private Helpers
        private static void IsMember(object obj, object member, string memberName, MemberType type)
        {
            if (member == null)
                throw new ReflectionException(memberName, type, obj);
        }

        private static void CheckObject(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj cannot be null.");
        }
        #endregion

        #region Obsolete Members
        /// <summary>
        /// Execute a NonPublic method on a object
        /// </summary>
        /// <param name="obj">Object to call method on</param>
        /// <param name="methodName">Method to call</param>
        /// <returns></returns>
        [Obsolete("Use InvokeMethod instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object RunNonPublicMethod(object obj, string methodName)
        {
            Type objType = obj.GetType();

            MethodInfo methodRequired = objType.GetMethod(methodName, BindingFlags.NonPublic |
                                                                        BindingFlags.Instance |
                                                                        BindingFlags.Public |
                                                                        BindingFlags.Static);

            return methodRequired.Invoke(obj, null);
        }

        /// <summary>
        /// Get the value from a NonPublic variable or field.
        /// </summary>
        /// <param name="obj">Object which contains field</param>
        /// <param name="variableName">Field Name</param>
        /// <returns></returns>
        [Obsolete("Use GetField instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetNonPublicVariable(object obj, string variableName)
        {
            Type objType = obj.GetType();

            FieldInfo variableInfo = objType.GetField(variableName, BindingFlags.NonPublic |
                                                                        BindingFlags.Instance |
                                                                        BindingFlags.Public |
                                                                        BindingFlags.Static);

            return variableInfo.GetValue(obj);
        }
        #endregion
    }
}
