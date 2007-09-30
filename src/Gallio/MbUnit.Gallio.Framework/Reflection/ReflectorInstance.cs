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

using System.Reflection;

namespace MbUnit.Framework.Reflection
{
    ///<summary>
    /// Member types of a class.
    ///</summary>
    public enum MemberType
    {
        ///<summary>
        /// Method
        ///</summary>
        Method, 
        /// <summary>
        /// Field or variable
        /// </summary>
        Field, 
        ///<summary>
        /// Property
        ///</summary>
        Property
    }

    ///<summary>
    /// Access modifier of a class or class member.
    ///</summary>
    public enum AccessModifier
    { 
        ///<summary>
        /// public
        ///</summary>
        Public = BindingFlags.Public, 
        ///<summary>
        /// protected, internal, private
        ///</summary>
        NonPublic = BindingFlags.NonPublic, 
        /// <summary>
        /// static
        /// </summary>
        Static  = BindingFlags.Static,
        /// <summary>
        /// default that includes public, protected, internal, private, and static
        /// </summary>
        Default = Public | NonPublic | Static
    }

    public partial class Reflector
    {
        readonly object _obj;

        #region Constructors
        /// <summary>
        /// Constructor for object
        /// </summary>
        /// <param name="obj">Object to be referred to in methods</param>
        public Reflector(object obj)
        {
            CheckObject(obj);
            _obj = obj;
        }

        /// <summary>
        /// Use this constructor if you plan to test default constructor of a non-public class.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        public Reflector(string assemblyName, string typeName)
            : this(assemblyName, typeName, null)
        {
        }

        /// <summary>
        /// Use this constructor if you plan to test constructor with arguments of a non-public class.
        /// </summary>
        /// <param name="assemblyName">Assembly name</param>
        /// <param name="typeName">Type name</param>
        /// <param name="args">Parameters for a constructor.</param>
        public Reflector(string assemblyName, string typeName, params object[] args)
        {
            _obj = CreateInstance(assemblyName, typeName, args);
        }
        #endregion

        #region Get/Set Fields
        /// <summary>
        /// Get public, non-public, or static field value.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Field value</returns>
        public object GetField(string fieldName)
        {
            return GetField(AccessModifier.Default, _obj, fieldName);
        }

        /// <summary>
        /// Get field value.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="access">Specify field access modifier.</param>
        /// <returns>Field value</returns>
        public object GetField(string fieldName, AccessModifier access)
        {
            return GetField(access, _obj, fieldName);
        }

        /// <summary>
        /// Get field value.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="access">Specify field access modifier.</param>
        /// <param name="lookInBase">Specify if need to look in Base classes.</param>
        /// <returns>Field value</returns>
        public object GetField(string fieldName, AccessModifier access, bool lookInBase)
        {
            return GetField(access, _obj, fieldName, lookInBase);
        }

        /// <summary>
        /// Set field value.
        /// </summary>
        /// <param name="fieldName">Field Name.</param>
        /// <param name="fieldValue">Field Value.</param>
        public void SetField(string fieldName, object fieldValue)
        {
            SetField(AccessModifier.Default, _obj, fieldName, fieldValue);
        }

        /// <summary>
        /// Set field value.
        /// </summary>
        /// <param name="fieldName">Field Name.</param>
        /// <param name="fieldValue">Field Value.</param>
        /// <param name="access">Specify field access modifier.</param>
        public void SetField(AccessModifier access, string fieldName, object fieldValue)
        {
            SetField(access, _obj, fieldName, fieldValue);
        }
        #endregion

        /// <summary>
        /// Get Property Value
        /// </summary>
        /// <param name="propertyName">Property Name.</param>
        /// <returns>Property Value.</returns>
        public object GetProperty(string propertyName)
        {
            return GetProperty(AccessModifier.Default, _obj, propertyName);
        }

        /// <summary>
        /// Get Property Value
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="propertyName">Property Name.</param>
        /// <returns>Property Value.</returns>
        public object GetProperty(AccessModifier access, string propertyName)
        {
            return GetProperty(access, _obj, propertyName);
        }

        /// <summary>
        /// Get Property Value
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="propertyName">Property Name.</param>
        /// <param name="lookInBase">Specify if need to look in Base classes.</param>
        /// <returns>Property Value.</returns>
        public object GetProperty(AccessModifier access, string propertyName, bool lookInBase)
        {
            return GetProperty(access, _obj, propertyName, lookInBase);
        }

        /// <summary>
        /// Set Property value.
        /// </summary>
        /// <param name="propertyName">Property Name.</param>
        /// <param name="propertyValue">Property Value.</param>
        public void SetProperty(string propertyName, object propertyValue)
        {
            SetProperty(AccessModifier.Default, _obj, propertyName, propertyValue);
        }

        /// <summary>
        /// Set Property value.
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="propertyName">Property Name.</param>
        /// <param name="propertyValue">Property Value.</param>
        public void SetProperty(AccessModifier access, string propertyName, object propertyValue)
        {
            SetProperty(access, _obj, propertyName, propertyValue);
        }

        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="methodName">Method to call</param>
        /// <returns>The object the method should return.</returns>
        public object InvokeMethod(string methodName)
        {
            return InvokeMethod(AccessModifier.Default, _obj, methodName, null);
        }

        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="methodName">Method to call</param>
        /// <param name="methodParams">Method's parameters.</param>
        /// <returns>The object the method should return.</returns>
        public object InvokeMethod(string methodName, params object[] methodParams)
        {
            return InvokeMethod(AccessModifier.Default, _obj, methodName, methodParams);
        }

        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="methodName">Method to call</param>
        /// <param name="access">Specify method access modifier.</param>
        /// <param name="methodParams">Method's parameters.</param>
        /// <returns>The object the method should return.</returns>
        public object InvokeMethod(AccessModifier access, string methodName, params object[] methodParams)
        {
            return InvokeMethod(access, _obj, methodName, methodParams);
        }
    }
}
