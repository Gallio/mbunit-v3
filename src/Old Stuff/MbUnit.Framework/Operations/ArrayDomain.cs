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

namespace TestFu.Operations
{
    public class ArrayDomain : DomainBase
    {
        private Array array;

        public ArrayDomain(params Object[] array)
        {
            this.array = array;
        }
        public ArrayDomain(Array array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            this.array = array;
        }
        public override int Count
        {
            get { return array.Length; }
        }

        public override IEnumerator GetEnumerator()
        {
            return array.GetEnumerator();
        }

        public override Object this[int index]
        {
            get { return this.array.GetValue(index); }
        }
    }
}
