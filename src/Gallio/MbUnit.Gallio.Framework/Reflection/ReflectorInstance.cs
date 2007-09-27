using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace MbUnit.Framework.Reflection
{
    public enum MemberType { Method, Field, Property }
    public enum AccessModifier
    { 
        Public = BindingFlags.Public, 
        NonPublic = BindingFlags.NonPublic, 
        Static  = BindingFlags.Static,
        Default = Public | NonPublic | Static
    }

    public partial class Reflector
    {
        object _obj;

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
        /// <param name="className"></param>
        public Reflector(string assemblyName, string typeName)
            : this(assemblyName, typeName, null)
        {
        }

        /// <summary>
        /// Use this constructor if you plan to test constructor with arguments of a non-public class.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
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
        /// <param name="fieldName">Property Name.</param>
        /// <param name="fieldValue">Property Value.</param>
        public void SetProperty(string propertyName, object propertyValue)
        {
            SetProperty(AccessModifier.Default, _obj, propertyName, propertyValue);
        }

        /// <summary>
        /// Set Property value.
        /// </summary>
        /// <param name="access">Specify property access modifier.</param>
        /// <param name="fieldName">Property Name.</param>
        /// <param name="fieldValue">Property Value.</param>
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

        #region Obsolete Members

        /// <summary>
        /// Gets value of NonPublic property
        /// </summary>
        /// <param name="propName">Property name</param>
        /// <returns>Property value</returns>
        [Obsolete("Use GetProperty instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetNonPublicProperty(string propName)
        {
            PropertyInfo pi = _obj.GetType().GetProperty(propName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            IsMember(pi, propName, MemberType.Property);
            return pi.GetValue(_obj, null);
        }

        /// <summary>
        /// Gets value of NonPublic field.
        /// </summary>
        /// <param name="fieldName">NonPublic field name</param>
        /// <returns>Field value</returns>
        [Obsolete("Use GetField instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetNonPublicField(string fieldName)
        {
            FieldInfo fi = _obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            IsMember(fi, fieldName, MemberType.Field);
            return fi.GetValue(_obj);
        }

        /// <summary>
        /// Get the value from a NonPublic variable or field.
        /// </summary>
        /// <param name="obj">Object which contains field</param>
        /// <param name="variableName">Field Name</param>
        /// <returns></returns>
        [Obsolete("Use GetField instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetPrivateVariable(string variableName)
        {
            Type objType = _obj.GetType();

            FieldInfo variableInfo = objType.GetField(variableName, BindingFlags.NonPublic |
                                                                        BindingFlags.Instance |
                                                                        BindingFlags.Public |
                                                                        BindingFlags.Static);

            return variableInfo.GetValue(_obj);
        }

        /// <summary>
        /// Execute a NonPublic method without arguments on a object
        /// </summary>
        /// <param name="obj">Object to call method on</param>
        /// <param name="methodName">Method to call</param>
        /// <returns>The object the method should return.</returns>
        [Obsolete("Use InvokeMethod instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object RunPrivateMethod(string methodName)
        {
            return RunPrivateMethod(methodName, null);
        }

        /// <summary>
        /// Execute a NonPublic method with arguments on a object
        /// </summary>
        /// <param name="obj">Object to call method on</param>
        /// <param name="methodName">Method to call</param>
        /// <returns>The object the method should return.</returns>
        [Obsolete("Use InvokeMethod instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object RunPrivateMethod(string methodName, params object[] methodParams)
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

            MethodInfo mi = _obj.GetType().GetMethod(methodName
                    , BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
                    , null, paramTypes, null);

            IsMember(mi, methodName, MemberType.Method);
            return mi.Invoke(_obj, methodParams);
        }

        #endregion

        private void IsMember(object member, string memberName, MemberType type)
        {
            IsMember(_obj, member, memberName, type);
        }
    }
}
