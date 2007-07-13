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
using System.Collections;
using System.Reflection;

namespace MbUnit.Core.Collections
{
    /// <summary>
    /// Summary description for AttributedMethodCollection.
    /// </summary>
    public sealed class AttributedMethodCollection : ICollection
    {
        private Type testedType;
        private Type customAttributeType;

        public AttributedMethodCollection(Type testedType, Type customAttributeType)
        {
            if (testedType == null)
                throw new ArgumentNullException("testedType");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");
            this.testedType = testedType;
            this.customAttributeType = customAttributeType;
        }

        public Object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public void CopyTo(Array array, int index)
        {
            int i = index;
            foreach (MethodInfo mi in this)
            {
                array.SetValue(mi, i++);
            }
        }

        public int Count
        {
            get
            {
                AttributedMethodEnumerator en = GetEnumerator();
                int n = 0;
                while (en.MoveNext())
                    ++n;
                return n;
            }
        }

        public AttributedMethodEnumerator GetEnumerator()
        {
            return new AttributedMethodEnumerator(
                this.testedType,
                this.customAttributeType
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
