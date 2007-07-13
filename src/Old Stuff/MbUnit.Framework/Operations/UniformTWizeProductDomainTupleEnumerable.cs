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
    internal class UniformTWizeProductDomainTupleEnumerable : ITupleEnumerable
    {
        private IDomainCollection domains;
        private int tupleSize;
        public UniformTWizeProductDomainTupleEnumerable(IDomainCollection domains, int tupleSize)
        {
            if (domains == null)
                throw new ArgumentNullException("domains");
            if (tupleSize <= 0)
                throw new ArgumentOutOfRangeException("tupleSize is negative or zero");

            this.domains = domains;
            this.tupleSize = tupleSize;

            int count = -1;
            for (int i = 0; i < domains.Count; ++i)
            {
                if (i == 0)
                    count = domains[i].Count;
                else
                {
                    if (count != domains[i].Count)
                        throw new ArgumentException("Domains have not uniform size");
                }
            }
        }

        public IDomainCollection Domains
        {
            get
            {
                return this.domains;
            }
        }

        public ITupleEnumerator GetEnumerator()
        {
            return new UniformTWizeProductDomainTupleEnumerator(this.Domains, this.tupleSize);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal sealed class UniformTWizeProductDomainTupleEnumerator :
            DomainTupleEnumeratorBase
        {
            private int tupleSize;
            public UniformTWizeProductDomainTupleEnumerator(IDomainCollection domains, int tupleSize)
                :base(domains)
            {
                this.tupleSize = tupleSize;
                this.CreateColoring();
            }

            public override void Reset()
            {
                throw new NotImplementedException();
            }
            public override bool MoveNext()
            {
                throw new NotImplementedException();
            }
            public override ITuple Current
            {
                get { throw new NotImplementedException(); }
            }

            private void CreateColoring()
            {
            }
        }
    }
}
