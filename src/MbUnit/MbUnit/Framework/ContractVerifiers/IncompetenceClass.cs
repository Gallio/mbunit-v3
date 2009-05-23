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
    /// A collection of distinct object instances associated with a particular exception type. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every element represents an instance which is different from all the other 
    /// elements in the collection (object equality), and which is expected to be
    /// the primary cause of an exception thrown when it is set or passed to 
    /// the tested method or property.
    /// </para>
    /// <para>
    /// Distinct invalid instances are used by some contract verifiers such as 
    /// <see cref="AccessorContract{TTarget, TValue}"/> to check for the correct 
    /// detection of invalid or unexpected value assignment.
    /// </para>
    /// <para>
    /// Use the single-parameter constructor to create an empty collection, which can be then
    /// populated by calling explicitely the the <see cref="DistinctInstanceCollection{T}.Add(T)"/>
    /// method inherited from <see cref="DistinctInstanceCollection{T}"/> or by using the list initializer syntax.
    /// <example>
    /// <code><![CDATA[
    /// var collection = new IncompetenceClass<Foo>(typeof(ArgumentException));
    /// collection.Add(new Foo(1));
    /// collection.Add(new Foo(2));
    /// collection.Add(new Foo(3));
    /// ]]></code>
    /// </example>
    /// <example>
    /// <code><![CDATA[
    /// var collection = new IncompetenceClass<Foo>(typeof(ArgumentException))
    /// {
    ///     new Foo(1)
    ///     new Foo(2),
    ///     new Foo(3),
    /// };
    /// ]]></code>
    /// </example>
    /// </para>
    /// <para>
    /// Or, use the two-parameters constructor to create a collection from an pre-existing enumeration.
    /// <example>
    /// <code><![CDATA[
    /// var collection = new InvalidClass<Foo>(typeof(ArgumentException), Foo.GetThemAll());
    /// ]]></code>
    /// </example>
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the object instances in the collection.</typeparam>
    public class IncompetenceClass<T> : DistinctInstanceCollection<T>
    {
        private readonly Type expectedExceptionType;

        /// <summary>
        /// The type of the exception associated with the object instances in the collection.
        /// </summary>
        public Type ExpectedExceptionType
        {
            get
            {
                return expectedExceptionType;
            }
        }

        /// <summary>
        /// <para>
        /// Constructs an empty collection of distinct object instances associated with an exception type.
        /// </para>
        /// <para>
        /// Use this constructor in conjunction with the <see cref="DistinctInstanceCollection{T}.Add(T)"/>
        /// method inherited from <see cref="DistinctInstanceCollection{T}"/> to initialize the collection; 
        /// either explicitely, or by using the list initializer syntax.
        /// <example>
        /// <code><![CDATA[
        /// var collection = new IncompetenceClass<Foo>(typeof(ArgumentException));
        /// collection.Add(new Foo(1));
        /// collection.Add(new Foo(2));
        /// collection.Add(new Foo(3));
        /// ]]></code>
        /// </example>
        /// <example>
        /// <code><![CDATA[
        /// var collection = new IncompetenceClass<Foo>(typeof(ArgumentException))
        /// {
        ///     new Foo(1)
        ///     new Foo(2),
        ///     new Foo(3),
        /// };
        /// ]]></code>
        /// </example>
        /// </para>
        /// </summary>
        /// <param name="expectedExceptionType"></param>
        public IncompetenceClass(Type expectedExceptionType)
        {
            this.expectedExceptionType = CheckValidExceptionType(expectedExceptionType);
        }

        /// <summary>
        /// <para>
        /// Constructs a collection of distinct object instances associated with an exception type.
        /// </para>
        /// <para>
        /// Every element represents an instance which is different from all the other 
        /// elements in the collection (object equality), and which is expected to be
        /// the primary cause of an exception thrown when it is set or passed to 
        /// the tested method or property.
        /// </para>
        /// </summary>
        /// <param name="expectedExceptionType">The type of the exception associated with the object instances in the collection.</param>
        /// <param name="distinctInvalidInstances">An array of distinct object instances.</param>
        public IncompetenceClass(Type expectedExceptionType, IEnumerable<T> distinctInvalidInstances)
            : base(distinctInvalidInstances)
        {
            this.expectedExceptionType = CheckValidExceptionType(expectedExceptionType);
        }

        private static Type CheckValidExceptionType(Type expectedExceptionType)
        {
            if (expectedExceptionType == null)
            {
                throw new ArgumentNullException("expectedExceptionType");
            }

            if (!typeof(Exception).IsAssignableFrom(expectedExceptionType))
            {
                throw new ArgumentException("The specified exception type must derive from 'System.Exception'.", "expectedExceptionType");
            }

            return expectedExceptionType;
        }
    }
}
