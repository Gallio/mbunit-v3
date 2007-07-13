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
    public abstract class DomainBase : IDomain
    {
        private string name = null;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public abstract int Count { get;}
        public virtual IDomain Boundary 
        { 
            get
            {
                if (this.Count <= 2)
                    return this;
                return new ArrayDomain(this[0], this[this.Count - 1]);
            }
        }
        public abstract IEnumerator GetEnumerator();
        public abstract Object this[int index] { get;}
    }
}
