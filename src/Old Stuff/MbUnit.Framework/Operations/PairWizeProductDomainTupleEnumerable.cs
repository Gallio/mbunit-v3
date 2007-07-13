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
    internal class PairWizeProductDomainTupleEnumerable : ITupleEnumerable
    {
        private IDomainCollection domains;
        public PairWizeProductDomainTupleEnumerable(IDomainCollection domains)
        {
            if (domains==null)
                throw new ArgumentNullException("domains");
            this.domains = domains;
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
            return new PairWizeProductDomainTupleEnumerator(this.Domains);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal sealed class PairWizeProductDomainTupleEnumerator : DomainTupleEnumeratorBase
        {
            public PairWizeProductDomainTupleEnumerator(IDomainCollection domains)
		    :base(domains)
            {
                this.Reset();
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
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
