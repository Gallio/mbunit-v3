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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// A collection of classes of distinct object instances gathered by the type
    /// of the exception which is expected to be raised when the subject instances are passed
    /// to a tested method or property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Distinct invalid instances are used by some contract verifiers such as 
    /// <see cref="AccessorContract{TTarget, TValue}"/> to check for the correct 
    /// detection of invalid or unexpected value assignment.
    /// </para>
    /// <para>
    /// Use the default constructor to create an empty collection, then use the 
    /// <see cref="Add(Type, T[])"/> method, either explicitely, or by using the list initializer
    /// syntax, to feed the collection with classes of objects:
    /// <example>
    /// <code><![CDATA[
    /// var collection = new InvalidValuesClassCollection<Foo>();
    /// collection.Add(typeof(ArgumentException), new Foo(1), new Foo(2));
    /// collection.Add(typeof(ArgumentOutOfRangeException), new Foo(3));
    /// ]]></code>
    /// </example>
    /// <example>
    /// <code><![CDATA[
    /// var collection = new InvalidValuesClassCollection<Foo>
    /// {
    ///     { typeof(ArgumentException), new Foo(1), new Foo(2) },
    ///     { typeof(ArgumentOutOfRangeException), new Foo(3) }
    /// };
    /// ]]></code>
    /// </example>
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the object instances in the inner class collections.</typeparam>
    public class InvalidValuesClassCollection<T> : IEnumerable<InvalidValuesClass<T>>
    {
        private readonly List<InvalidValuesClass<T>> invalidClasses;

        /// <summary>
        /// Gets the number of invalid classes in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return invalidClasses.Count;
            }
        }

        /// <summary>
        /// Constructs an empty collection of classes of distinct object instances. Each class
        /// identifies a collection of distinct object instances and the type
        /// of the exception which is expected to be raised when the subject instances are passed
        /// to the tested method or property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Once the collection is created, populate it by using the <see cref="Add(Type, T[])"/>
        /// method; Either explicitely, or by using the list initializer syntax:
        /// <example>
        /// <code><![CDATA[
        /// var collection = new InvalidValuesClassCollection<Foo>();
        /// collection.Add(typeof(ArgumentException), new Foo(1), new Foo(2));
        /// collection.Add(typeof(ArgumentOutOfRangeException), new Foo(3));
        /// ]]></code>
        /// </example>
        /// <example>
        /// <code><![CDATA[
        /// var collection = new InvalidValuesClassCollection<Foo>
        /// {
        ///     { typeof(ArgumentException), new Foo(1), new Foo(2) },
        ///     { typeof(ArgumentOutOfRangeException), new Foo(3) }
        /// };
        /// ]]></code>
        /// </example>
        /// </para>
        /// </remarks>
        public InvalidValuesClassCollection()
        {
            invalidClasses = new List<InvalidValuesClass<T>>();
        }

        /// <summary>
        /// Adds the specified class of distinct object instance to the collection.
        /// </summary>
        /// <param name="invalidClass">The class to add to the collection.</param>
        public void Add(InvalidValuesClass<T> invalidClass)
        {
            if (invalidClass == null)
                throw new ArgumentNullException("invalidClass");

            CheckExceptionTypeAlreadyIn(invalidClass.ExpectedExceptionType);
            invalidClasses.Add(invalidClass);
        }

        /// <summary>
        /// Adds a new class of distinct object instances to the collection.
        /// The new class is created with the specified arguments.
        /// </summary>
        /// <param name="expectedException">The exception type to associate with the new class.</param>
        /// <param name="invalidValues">an array of distinct object instances to populate the new class.</param>
        public void Add(Type expectedException, params T[] invalidValues)
        {
            if (expectedException == null)
                throw new ArgumentNullException("expectedException");

            CheckExceptionTypeAlreadyIn(expectedException);
            invalidClasses.Add(new InvalidValuesClass<T>(expectedException, invalidValues));
        }

        private void CheckExceptionTypeAlreadyIn(Type expectedException)
        {
            if (invalidClasses.Exists(x => x.ExpectedExceptionType == expectedException))
                throw new ArgumentException(String.Format("The collection already contains a class associated " +
                    "with the exception type '{0}'.", expectedException), "expectedException");
        }

        /// <summary>
        /// Returns a strongly-typed enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A strongly-typed enumerator.</returns>
        public IEnumerator<InvalidValuesClass<T>> GetEnumerator()
        {
            return invalidClasses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var invalidClass in invalidClasses)
            {
                yield return invalidClass;
            }
        }
    }
}
