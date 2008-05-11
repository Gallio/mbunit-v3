// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace MbUnit.TestResources
{
    /// <summary>
    /// A documented class.
    /// </summary>
    /// <remarks>
    /// The XML documentation of this test is significant.
    ///   Including the leading whitespace on this line.
    ///     And the extra 8 trailing spaces on this line!        
    /// </remarks>
    public class DocumentedClass
    {
        /// <summary>
        /// A documented constructor.
        /// </summary>
        public DocumentedClass()
        {
        }

        /// <summary>
        /// A documented constructor with parameters.
        /// </summary>
        public DocumentedClass(int x)
        {
        }

        /// <summary>
        /// A documented finalizer.
        /// </summary>
        ~DocumentedClass()
        {
        }

        /// <summary>
        /// A documented field.
        /// </summary>
        public int DocumentedField;

        /// <summary>
        /// A documented property.
        /// </summary>
        public int DocumentedProperty { get { return 0; } }

        /// <summary>
        /// A documented event.
        /// </summary>
#pragma warning disable 0067
        public event EventHandler DocumentedEvent;
#pragma warning restore 0067

        /// <summary>
        /// A documented indexer.
        /// </summary>
        public int this[int index] { get { return 0; } }

        /// <summary>
        /// A documented method.
        /// </summary>
        public void DocumentedMethod()
        {
        }

        /// <summary>
        /// A documented overloaded method with parameters.
        /// </summary>
        public void DocumentedMethod(int x, DocumentedClass c, GenericNestedClass<int> nc)
        {
        }

        /// <summary>
        /// A documented operator.
        /// </summary>
        public static DocumentedClass operator +(DocumentedClass a, DocumentedClass b)
        {
            return null;
        }

        /// <summary>
        /// A documented implicit conversion operator.
        /// </summary>
        public static implicit operator int(DocumentedClass a)
        {
            return 0;
        }

        /// <summary>
        /// A documented explicit conversion operator.
        /// </summary>
        public static explicit operator double(DocumentedClass a)
        {
            return 0;
        }

        public class UndocumentedNestedClass
        {
            public int UndocumentedField;

            public int UndocumentedProperty { get { return 0; } }

#pragma warning disable 0067
            public event EventHandler UndocumentedEvent;
#pragma warning restore 0067

            public void UndocumentedMethod()
            {
            }
        }

        /// <summary>
        /// A documented generic nested class.
        /// </summary>
        public class GenericNestedClass<T>
        {
            /// <summary>
            /// A documented generic method with parameters.
            /// </summary>
            public void DocumentedGenericMethodWithParameters<S>(S s, T t, int y)
            {
            }

            /// <summary>
            /// A documented generic indexer.
            /// </summary>
            public T this[T t]
            {
                get { return default(T); }
            }
        }
    }
}
