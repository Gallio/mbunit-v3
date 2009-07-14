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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using Gallio.Common.Collections;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    internal class ImmutableTypeCollection : ICollection<Type>
    {
        private readonly HashSet<Type> types;

        public ImmutableTypeCollection()
        {
            types = new HashSet<Type>
            {
                typeof(Boolean),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(IntPtr),
                typeof(UInt16),
                typeof(UInt32),
                typeof(UInt64),
                typeof(UIntPtr),
                typeof(Single),
                typeof(Double),
                typeof(Decimal),
                typeof(Byte),
                typeof(Char),
                typeof(String),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(ReadOnlyCollectionBase),
                typeof(ReadOnlyCollection<>),
                typeof(ReadOnlyDictionary<,>),
            };
        }

        public void Add(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsGenericType)
                type = type.GetGenericTypeDefinition();
                
            types.Add(type);
        }

        public int Count
        {
            get
            {
                return types.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var type in types)
            {
                yield return type;
            }
        }

        public bool Contains(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsEnum || typeof(Delegate).IsAssignableFrom(type))
                return true;

            if (type.IsGenericType)
                type = type.GetGenericTypeDefinition();

            return types.Contains(type);
        }

        #region Not supported operations

        public bool Remove(Type type)
        {
            throw new NotSupportedException();
        }
        public void Clear()
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Type[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
