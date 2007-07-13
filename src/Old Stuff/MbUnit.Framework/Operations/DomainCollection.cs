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
    public class DomainCollection : CollectionBase, IDomainCollection
    {
        public DomainCollection(IDomain[] domains)
        {
            foreach (IDomain domain in domains)
                this.Add(domain);
        }
        public DomainCollection()
        {}

        public IDomain this[int index]
        {
            get
            {
                return (IDomain)this.List[index];
            }
        }

        public void Add(IDomain domain)
        {
            if (domain == null)
                throw new ArgumentNullException("domain");
            this.List.Add(domain);
        }

        public new IDomainEnumerator GetEnumerator()
        {
            return new DomainEnumerator(this.List.GetEnumerator());
        }

        internal class DomainEnumerator : IDomainEnumerator
        {
            private IEnumerator en;
            public DomainEnumerator(IEnumerator en)
            {
                this.en = en;
            }

            public IDomain Current
            {
                get
                {
                    return (IDomain)this.en.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Reset()
            {
                this.en.Reset();
            }

            public bool MoveNext()
            {
                return this.en.MoveNext();
            }
        }
    }
}
