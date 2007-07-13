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
    internal class StringDomain : DomainBase
    {
        private string value;
        public StringDomain(string value)
        {
            this.value = value;
        }
        public override int Count
        {
            get { return 1; }
        }
        public override Object this[int i]
        {
            get 
            {
                if (i != 0)
                    throw new ArgumentOutOfRangeException("index out of range");
                return this.value;
            }
        }
        public override IEnumerator GetEnumerator()
        {
            ArrayList list = new ArrayList();
            list.Add(this.value);
            return list.GetEnumerator();
        }
    }
}
