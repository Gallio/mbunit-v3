// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Reflection;

#pragma warning disable 1591
#pragma warning disable 3001

namespace MbUnit.Framework
{
	/// <summary>
	/// Reflection Assertion class
	/// </summary>
    [Obsolete("Use Assert instead.")]
    public static class OldReflectionAssert
	{
		/// <summary>
		/// Asserts whether an instance of the <paramref name="parent"/> 
		/// can be assigned from an instance of <paramref name="child"/>.
		/// </summary>
		/// <param name="parent">
		/// Parent <see cref="Type"/> instance.
		/// </param>
		/// <param name="child">
		/// Child <see cref="Type"/> instance.
		/// </param>
		public static void IsAssignableFrom(Type parent, Type child)
		{
			OldAssert.IsNotNull(parent);
			OldAssert.IsNotNull(child);
			OldAssert.IsTrue(parent.IsAssignableFrom(child),
			              "{0} is not assignable from {1}",
			              parent,
			              child
			              );
		}

		/// <summary>
		/// Asserts whether <paramref name="child"/> is an instance of the 
		/// <paramref name="type"/>.
		/// </summary>
		/// <param name="type">
        /// <see cref="Type"/> instance.
		/// </param>
		/// <param name="child">
		/// Child instance.
		/// </param>
		public static void IsInstanceOf(Type type, Object child)
		{
			OldAssert.IsNotNull(type);
			OldAssert.IsNotNull(child);
			OldAssert.IsTrue(type.IsInstanceOfType(child),
			              "{0} is not an instance of {1}",
			              type,
			              child
			              );
		}
		
		/// <summary>
		/// Asserts that the type has a default public constructor
		/// </summary>
		public static void HasDefaultConstructor(Type type)
		{
			HasConstructor(type, Type.EmptyTypes);
		}

		/// <summary>
		/// Asserts that the type has a public instance constructor with a signature defined by parameters.
		/// </summary>		
		public static void HasConstructor(Type type, params Type[] parameters)
		{
			HasConstructor(type,BindingFlags.Public | BindingFlags.Instance,parameters);
		}

		/// <summary>
		/// Asserts that the type has a constructor, with the specified bindind flags, with a signature defined by parameters.
		/// </summary>				
		public static void HasConstructor(Type type, BindingFlags flags, params Type[] parameters)
		{
			OldAssert.IsNotNull(type);
			OldAssert.IsNotNull(type.GetConstructor(flags,null,parameters,null),
			                 "{0} does not have matching constructor",
			                 type.FullName
			                 );
		}
		
		/// <summary>
		/// Asserts that the type has a public instance method with a signature defined by parameters.
		/// </summary>		
		public static void HasMethod(Type type, string name, params Type[] parameters)
		{
			HasMethod(type,BindingFlags.Public | BindingFlags.Instance,name,parameters); 
		}

		
		/// <summary>
		/// Asserts that the type has a method, with the specified bindind flags, with a signature defined by parameters.
		/// </summary>				
		public static void HasMethod(Type type, BindingFlags flags, string name, params Type[] parameters)
		{
			OldAssert.IsNotNull(type, "Type is null");
			OldStringAssert.IsNonEmpty(name);
			
			OldAssert.IsNotNull(type.GetMethod(name,parameters),
			                 "Method {0} of type {1} not found with matching arguments",
			                 name,
			                 type
			                 );
		}

		/// <summary>
		/// Asserts that the type has a public field method with a signature defined by parameters.
		/// </summary>		
		public static void HasField(Type type, string name)
		{
			HasField(type, BindingFlags.Public | BindingFlags.Instance,name );
		}
		
		/// <summary>
		/// Asserts that the type has a field, with the specified bindind flags, with a signature defined by parameters.
		/// </summary>								
		public static void HasField(Type type, BindingFlags flags,string name)
		{
			OldAssert.IsNotNull(type, "Type is null");
			OldStringAssert.IsNonEmpty(name);
			
			OldAssert.IsNotNull(type.GetField(name),
			                 "Field {0} of type {1} not found with binding flags {2}",
			                 name,
			                 type,
			                 flags
			                 );
		}

		public static void ReadOnlyProperty(Type t, string propertyName)
		{
			OldAssert.IsNotNull(t);
			OldAssert.IsNotNull(propertyName);
			PropertyInfo pi = t.GetProperty(propertyName);
			OldAssert.IsNotNull(pi,
				"Type {0} does not contain property {1}",
				t.FullName,
				propertyName);
			ReadOnlyProperty(pi);
		}

		public static void ReadOnlyProperty(PropertyInfo pi)
		{
			OldAssert.IsNotNull(pi);
			OldAssert.IsFalse(pi.CanWrite,
				"Property {0}.{1} is not read-only",
				pi.DeclaringType.Name,
				pi.Name
				);
		}

		public static void WriteOnlyProperty(Type t, string propertyName)
		{
			OldAssert.IsNotNull(t);
			OldAssert.IsNotNull(propertyName);
			PropertyInfo pi = t.GetProperty(propertyName);
			OldAssert.IsNotNull(pi,
				"Type {0} does not contain property {1}",
				t.FullName,
				propertyName);
			WriteOnlyProperty(pi);
		}

		public static void WriteOnlyProperty(PropertyInfo pi)
		{
			OldAssert.IsNotNull(pi);
			OldAssert.IsFalse(pi.CanRead,
				"Property {0}.{1} is not read-only",
				pi.DeclaringType.FullName,
				pi.Name
				);
		}

		public static void IsSealed(Type t)
		{
			OldAssert.IsNotNull(t);
			OldAssert.IsTrue(t.IsSealed,
				"Type {0} is not sealed",
				t.FullName);
		}
		
		public static void NotCreatable(Type t)
		{
			OldAssert.IsNotNull(t);
			foreach(ConstructorInfo ci in t.GetConstructors())
			{
				OldAssert.Fail(
					"Non-private constructor found in class {0}  that must not be creatable",
					t.FullName);
			}
		}
	}
}
