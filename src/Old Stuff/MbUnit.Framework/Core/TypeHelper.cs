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

// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace MbUnit.Core
{
    using MbUnit.Core.Collections;
    using MbUnit.Core.Exceptions;
    using MbUnit.Framework;
    using MbUnit.Core.Reflection;

    /// <summary>
    /// Helper static class for Type related tasks
    /// </summary>
    /// <include file="MbUnit.Core.Doc.xml" path="doc/remarkss/remarks[@name='TypeHelper']"/>
    public sealed class TypeHelper
    {
        internal TypeHelper()
        { }

        /// <summary>
        /// Output the methods and their custom attributes to the console.
        /// (Debugging method)
        /// </summary>
        /// <param name="t">type to visit</param>
        /// <remarks>
        /// You can use this method to display the methods of a class or struct
        /// type. Mainly for debugging purpose.
        /// </remarks>
        /// <include file="MbUnit.Core.Doc.xml" path="doc/examples/example[@name='TypeHelper.ShowMethodAttributes']"/>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="t"/>
        /// is a null reference
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="t"/>
        /// is anot a class type.
        /// </exception>
        public static void ShowMethodAttributes(Type t)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (!t.IsClass)
                throw new ArgumentException("t must be a class or struct");

            Console.WriteLine("Method and attributes of " + t.Name);
            foreach (MethodInfo mi in t.GetMethods())
            {
                Console.WriteLine("\tMethod: {0}", mi.Name);
                foreach (Attribute a in mi.GetCustomAttributes(true))
                {
                    Console.WriteLine("\t\t{0}, {1}", a.GetType().Name, a.ToString());
                }
            }
        }

        public static void ShowPropertyValues(Object o)
        {
            ShowPropertyValues(o, Console.Out);
        }

        public static void ShowPropertyValues(Object o, TextWriter output)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (output == null)
                throw new ArgumentNullException("output");

            foreach (PropertyInfo pi in o.GetType().GetProperties())
            {
                if (!pi.CanRead)
                    continue;
                output.WriteLine("{0}: {1}", pi.Name, pi.GetValue(o, null));
            }
        }

        /// <summary>
        /// Gets a value indicating the class type <paramref name="t"/> has
        /// a method that is tagged
        /// by a <paramref name="customAttributeType"/> instance.
        /// </summary>
        /// <param name="t">type to test</param>
        /// <param name="customAttributeType">custom attribute type to search</param>
        /// <returns>
        /// true if class type <param name="t"/> has a method tagged by a <paramref name="customAttributeType"/>
        /// attribute, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="t"/> or <paramref name="customAttributeType"/>
        /// is a null reference
        /// </exception>
        /// <remarks>
        /// You can use this method to check that a type is tagged by an attribute.
        /// </remarks>		
        public static bool HasMethodCustomAttribute(Type t, Type customAttributeType)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            foreach (MethodInfo mi in t.GetMethods())
            {
                if (HasCustomAttribute(mi, customAttributeType))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating if the <paramref name="t"/> is tagged
        /// by a <paramref name="customAttributeType"/> instance.
        /// </summary>
        /// <param name="t">method to test</param>
        /// <param name="customAttributeType">custom attribute type to search</param>
        /// <returns>
        /// true if <param name="t"/> is tagged by a <paramref name="customAttributeType"/>
        /// attribute, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="t"/> or <paramref name="customAttributeType"/>
        /// is a null reference
        /// </exception>
        /// <remarks>
        /// You can use this method to check that a method is tagged by a
        /// specified attribute.
        /// </remarks>
        public static bool HasCustomAttribute(Type t, Type customAttributeType)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            return t.IsDefined(customAttributeType, true);
        }

        /// <summary>
        /// Gets a value indicating if the method info <paramref name="t"/> is tagged
        /// by a <paramref name="customAttributeType"/> instance.
        /// </summary>
        /// <param name="t">method to test</param>
        /// <param name="customAttributeType">custom attribute type to search</param>
        /// <returns>
        /// true if <param name="t"/> is tagged by a <paramref name="customAttributeType"/>
        /// attribute, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="t"/> or <paramref name="customAttributeType"/>
        /// is a null reference
        /// </exception>
        /// <remarks>
        /// You can use this method to check that a method is tagged by a
        /// specified attribute.
        /// </remarks>
        public static bool HasCustomAttribute(ICustomAttributeProvider t, Type customAttributeType)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            return t.IsDefined(customAttributeType, true);
        }

        /// <summary>
        /// Gets the first instance of <paramref name="customAttributeType"/> 
        /// from the method <paramref name="mi"/> custom attributes.
        /// </summary>
        /// <param name="mi">Method to test</param>
        /// <param name="customAttributeType">custom attribute type to search</param>
        /// <returns>
        /// First instance of <paramref name="customAttributeTyp"/>
        /// from the method <paramref name="mi"/> custom attributes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mi"/> or <paramref name="customAttributeType"/>
        /// is a null reference
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mi"/> is not tagged by an attribute of type
        /// <paramref name="customAttributeType"/>
        /// </exception>
        /// <remarks>
        /// You can use this method to retreive a specified attribute
        /// instance of a method.
        /// </remarks>
        public static Object GetFirstCustomAttribute(ICustomAttributeProvider mi, Type customAttributeType)
        {
            if (mi == null)
                throw new ArgumentNullException("mi");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            Object[] attrs = mi.GetCustomAttributes(customAttributeType, true);
            if (attrs.Length == 0)
                throw new ArgumentException("type does not have custom attribute");
            return attrs[0];
        }

        /// <summary>
        /// Gets the first instance of <paramref name="customAttributeType"/> 
        /// from the method <paramref name="mi"/> custom attributes.
        /// </summary>
        /// <param name="mi">Method to test</param>
        /// <param name="customAttributeType">custom attribute type to search</param>
        /// <returns>
        /// First instance of <paramref name="customAttributeTyp"/>
        /// from the method <paramref name="mi"/> custom attributes; otherwize
        /// a null reference
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mi"/> or <paramref name="customAttributeType"/>
        /// is a null reference
        /// </exception>
        /// <remarks>
        /// You can use this method to retreive a specified attribute
        /// instance of a method.
        /// </remarks>
        public static Object TryGetFirstCustomAttribute(ICustomAttributeProvider mi, Type customAttributeType)
        {
            if (mi == null)
                throw new ArgumentNullException("mi");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            Object[] attrs = mi.GetCustomAttributes(customAttributeType, true);
            if (attrs.Length == 0)
                return null;
            return attrs[0];
        }

        /// <summary>
        /// Gets the first method of the type <paramref name="t"/>
        /// that is tagged by a <paramref name="customAttributeType"/>
        /// instance.
        /// </summary>
        /// <param name="t">type to test</param>
        /// <param name="customAttributeType">custom attribute type to search</param>
        /// <returns>
        /// First method of <paramref name="t"/> that 
        /// that is tagged by a <paramref name="customAttributeType"/>
        /// instance, null if no method is tagged by the specified attribute
        /// type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="t"/> or <paramref name="customAttributeType"/>
        /// is a null reference
        /// </exception>
        /// <remarks>
        /// You can use this method to retreive a tagged method
        /// </remarks>
        public static MethodInfo GetAttributedMethod(Type t, Type customAttributeType)
        {
            return GetAttributedMethod(t, customAttributeType, BindingFlags.Public | BindingFlags.Instance);
        }

        public static MethodInfo GetAttributedMethod(Type t, Type customAttributeType, BindingFlags bindingFlags)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            foreach (MethodInfo m in t.GetMethods(bindingFlags))
            {
                if (HasCustomAttribute(m, customAttributeType))
                    return m;
            }

            return null;
        }

        /// <summary>
        /// Gets all methods of the type <paramref name="t"/>
        /// that are tagged by a <paramref name="customAttributeType"/>
        /// instance.
        /// </summary>
        /// <param name="t">type to test</param>
        /// <param name="customAttributeType">custom attribute type to search</param>
        /// <returns>
        /// <see cref="MethodInfo"/> collection  of type <paramref name="t"/> that 
        /// that are tagged by a <paramref name="customAttributeType"/>
        /// instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="t"/> or <paramref name="customAttributeType"/>
        /// is a null reference
        /// </exception>
        /// <remarks>
        /// You can use this method to retreive all the methods of a type
        /// tagged by a <paramref name="customAttributeType"/>.
        /// </remarks>
        public static AttributedMethodCollection GetAttributedMethods(Type t, Type customAttributeType)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            return new AttributedMethodCollection(t, customAttributeType);
        }

        public static AttributedPropertyCollection GetAttributedProperties(Type t, Type customAttributeType)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            return new AttributedPropertyCollection(t, customAttributeType);
        }

        /// <summary>
        /// Gets a value indicating if the type <paramref name="t"/> contains
        /// a Method with the signature defined by <paramref name="types"/>.
        /// </summary>
        /// <remarks>
        /// Checks if a type has a desired Method.
        /// </remarks>
        /// <param name="t">type to test</param>
        /// <param name="types">arguments of the Method</param>
        /// <returns>true if <paramref name="t"/> contains a Method matching
        /// types</returns>
        /// <exception cref="ArgumentNullException">t is a null reference</exception>
        public static bool HasConstructor(Type t, params Type[] types)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            return t.GetConstructor(types) != null;
        }

        /// <summary>
        /// Retreives the <see cref="MethodInfo"/> that matches the signature.
        /// </summary>		
        /// <param name="t">type to test</param>
        /// <param name="args">Method signature</param>
        /// <returns>
        /// The <see cref="MethodInfo"/> instance of <paramref name="t"/> matching
        /// the signature.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="t"/> is a null reference</exception>
        /// <exception cref="MethodNotFoundException">
        /// No Method of type <paramref name="t"/> match the signature defined
        /// by <paramref name="types"/>.
        /// </exception>
        /// <remarks>
        /// This method tries to retreive a Method matching the signature
        /// and throws if it failed.
        /// </remarks>
        public static ConstructorInfo GetConstructor(Type t, params Type[] types)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            ConstructorInfo ci = t.GetConstructor(types);
            if (ci == null)
                throw new ConstructorNotFoundException(t,
                                                       types
                                                       );
            return ci;
        }

        /// <summary>
        /// Retreives the <see cref="MethodInfo"/> that matches the signature,
        /// given the list of arguments.
        /// </summary>		
        /// <param name="t">type to test</param>
        /// <param name="types">Method arguments from which the signature
        /// is deduced</param>
        /// <returns>
        /// The <see cref="MethodInfo"/> instance of <paramref name="t"/> matching
        /// the signature defined by the list of arguments.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="t"/> is a null reference</exception>
        /// <exception cref="ArguementNullException">
        /// One of the args item is a null reference
        /// </exception>
        /// <exception cref="MethodNotFoundException">
        /// No Method of type <paramref name="t"/> match the signature defined
        /// by <paramref name="args"/>.
        /// </exception>
        /// <remarks>
        /// This methods retreives the types of <paramref name="args"/> and
        /// looks for a Method matching that signature.
        /// </remarks>
        public static ConstructorInfo GetConstructor(Type t, params Object[] args)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            Type[] types = GetParameterTypes(args);
            return GetConstructor(t, types);
        }

        public static Type[] GetParameterTypes(object[] args)
        {
            Type[] types = null;

            if (args.Length == 0)
                types = Type.EmptyTypes;
            else
            {
                types = new Type[args.Length];
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i] == null)
                        throw new ArgumentNullException("args[" + i + "]");

                    types[i] = args[i].GetType();
                }
            }

            return types;
        }

        /// <summary>
        /// Creates an instance of the type <paramref name="t"/> using
        /// the default Method.
        /// </summary>
        /// <param name="t">type to instanciate</param>
        /// <returns>type <paramref name="t"/> instance</returns>
        public static Object CreateInstance(Type t)
        {
            return CreateInstance(t, Type.EmptyTypes);
        }

        /// <summary>
        /// Creates an instance of the type <paramref name="t"/> using
        /// the Method that matches the signature defined by 
        /// <paramref name="args"/>
        /// </summary>
        /// <param name="t">type to instanciate</param>
        /// <param name="args">argument of the Method</param>
        /// <returns>type <paramref name="t"/> instance initialized using <paramref name="args"/></returns>
        public static Object CreateInstance(Type t, params Object[] args)
        {
            ConstructorInfo ci = GetConstructor(t, args);
            return ci.Invoke(args);
        }


        /// <summary>
        /// Gets a value indicating if the type <paramref name="t"/>
        /// has an indexer that takes <paramref name="args"/> arguments.
        /// </summary>
        /// <remarks>
        /// Checks that an indexer with a given signature exists in the class.
        /// </remarks>
        /// <param name="t">type that holds the indexer</param>
        /// <param name="args">indexer arguments</param>
        /// <returns>true if an indexer that matched the signature was found,
        /// false otherwise
        /// </returns>
        public static bool HasIndexer(Type t, params Type[] args)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (ParametersMatch(pi.GetIndexParameters(), args))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Retreives the indexer that matches the signature
        /// </summary>
        /// <remarks>
        /// Safe retreival of an indexer, given it's signature
        /// </remarks>
        /// <param name="t">type that holds the indexer</param>
        /// <param name="args">indexer arguments</param>
        public static PropertyInfo GetIndexer(Type t, params Type[] args)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (ParametersMatch(pi.GetIndexParameters(), args))
                    return pi;
            }

            throw new IndexerNotFoundException(t, args);
        }

        /// <summary>
        /// Gets the value of the property <paramref name="pi"/>.
        /// </summary>
        /// <param name="pi">property</param>
        /// <param name="o">object instnace</param>
        /// <param name="args">property arguments (in case of an indexer</param>
        /// <returns>property value</returns>
        public static Object GetValue(PropertyInfo pi, Object o, params Object[] args)
        {
            if (pi == null)
                throw new ArgumentNullException("pi");
            if (o == null)
                throw new ArgumentNullException("o");
            return pi.GetValue(o, args);
        }

        /// <summary>
        /// Gets a value indicating if the <paramref name="parameters"/> match
        /// the <paramref name="types"/>
        /// </summary>
        /// <param name="parameters">property or method paramter info</param>
        /// <param name="types">tested signature</param>
        public static bool ParametersMatch(ParameterInfo[] parameters, Type[] types)
        {
            if (parameters.Length != types.Length)
                return false;

            for (int i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i].ParameterType != types[i])
                    return false;
            }
            return true;
        }

        public static MethodInfo TryGetMethod(Type t, string name, params Type[] parameters)
        {
            if (t == null)
                throw new ArgumentNullException("t");
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentNullException("name is empty");

            MethodInfo mi = t.GetMethod(name, parameters);
            if (mi == null)
                return null;
            return mi;
        }

        public static MethodInfo GetMethod(Type t, string name, params Type[] parameters)
        {
            MethodInfo mi = TryGetMethod(t, name, parameters);
            if (mi == null)
                throw new MethodNotFoundException(t, name, parameters);
            return mi;
        }

        public static Object Invoke(MethodInfo method, Object o, IList args)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (o == null)
                throw new ArgumentNullException("o");
            if (args == null)
                throw new ArgumentNullException("args");

            Object[] argsArray = new Object[args.Count];
            args.CopyTo(argsArray, 0);
            return method.Invoke(o, argsArray);
        }

        public static void CheckSignature(MethodInfo mi, Type returnType)
        {
            if (mi == null)
                throw new ArgumentNullException("mi");
            if (returnType == null)
                throw new ArgumentNullException("returnType");

            ReflectionAssert.IsAssignableFrom(returnType, mi.ReturnType);
            ParameterInfo[] pis = mi.GetParameters();
            Assert.AreEqual(pis.Length, 0);
        }

        public static void CheckSignature(MethodInfo mi, Type returnType, params Type[] parameters)
        {
            if (mi == null)
                throw new ArgumentNullException("mi");
            if (returnType == null)
                throw new ArgumentNullException("returnType");

            SignatureChecker checker = new SignatureChecker(returnType, parameters);
            checker.Check(mi);
        }

        public static void InvokeFutureMethod(
            Object target,
            string methodName,
            params object[] parameters
            )
        {
            if (target == null)
                throw new ArgumentNullException("target");
            MethodInfo method = TypeHelper.TryGetMethod(target.GetType(), methodName, GetParameterTypes(parameters));
            if (method == null)
                return;
            method.Invoke(target, parameters);
        }

        public static void InvokeFutureStaticMethod(
            Type targetType,
            string methodName,
            params object[] parameters
            )
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            MethodInfo method = TypeHelper.TryGetMethod(targetType, methodName, GetParameterTypes(parameters));
            if (method == null)
                return;
            method.Invoke(null, parameters);
        }
    }
}
