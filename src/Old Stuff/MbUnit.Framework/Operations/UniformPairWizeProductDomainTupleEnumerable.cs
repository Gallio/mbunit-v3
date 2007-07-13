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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TestFu.Operations
{
    public class UniformPairWizeProductDomainTupleEnumerable : ITupleEnumerable
    {
        private IDomainCollection domains;
        public UniformPairWizeProductDomainTupleEnumerable(IDomainCollection domains)
        {
            if (domains == null)
                throw new ArgumentNullException("domains");
            this.domains = domains;
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
            return new UniformPairWizeProductDomainTupleEnumerator(this.Domains);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal sealed class UniformPairWizeProductDomainTupleEnumerator : 
            DomainTupleEnumeratorBase
        {
            private int m = -1;
            private int domainCount=-1;
            private ArrayList bipartiteGraphs = null;
            private int bipartiteGraphIndex = -1;
            private int leftIndex = 0;
            private int rightIndex = -1;
            private Tuple tuple = null;

            public UniformPairWizeProductDomainTupleEnumerator(IDomainCollection domains)
		    :base(domains)
            {
                this.domainCount = this.Domains[0].Count;
                this.Reset();
            }

            public override void Reset()
            {
                // get number of bipartite graphs
                m = (int)Math.Ceiling(Math.Log(this.Domains.Count, 2));

                // create bipartite graphs
                this.bipartiteGraphs = new ArrayList(m);
                for (int i = 0; i < m; ++i)
                {
                    // create bipartie graph
                    BipartiteGraph bg = new BipartiteGraph(this.Domains.Count);
                    this.bipartiteGraphs.Add(bg);

                    // do some swapping
                    if (i>0)
                        bg.Swap(i-1, bg.Left.Count+i-1);
                }


                this.bipartiteGraphIndex = -1;
                this.leftIndex = 0;
                this.rightIndex = 0;
                this.tuple = null;
            }

            public override bool MoveNext()
            {
                do
                {
                    if (this.leftIndex == this.rightIndex && this.bipartiteGraphIndex < this.bipartiteGraphs.Count)
                    {
                        this.bipartiteGraphIndex++;
                        this.CreateTuple();
                        this.bipartiteGraphIndex = this.bipartiteGraphs.Count;
                        return true;
                    }
                    else
                    {
                        this.bipartiteGraphIndex++;
                        if (this.bipartiteGraphIndex < this.bipartiteGraphs.Count)
                        {
                            this.CreateTuple();
                            return true;
                        }
                    }

                    // increasing index
                    this.rightIndex++;
                    if (this.rightIndex >= this.domainCount)
                    {
                        this.leftIndex++;
                        this.rightIndex = 0;
                    }
                    this.bipartiteGraphIndex = -1;
                } while (this.leftIndex < this.domainCount && this.rightIndex < this.domainCount);

                return false;
            }

            private void CreateTuple()
            {
                // get bipartite graph
                BipartiteGraph bg = (BipartiteGraph)this.bipartiteGraphs[this.bipartiteGraphIndex];

                this.tuple = new Tuple();
                for (int i = 0; i < this.Domains.Count; ++i)
                {
                    if (bg.Left.Contains(i))
                        this.tuple.Add(this.Domains[i][leftIndex]);
                    else
                        this.tuple.Add(this.Domains[i][rightIndex]);
                }
            }

            public override ITuple Current
            {
                get
                {
                    return this.tuple;
                }
            }

            private class BipartiteGraph
            {
                private SortedList left = new SortedList();
                private SortedList right = new SortedList();

                public BipartiteGraph(int count)
                {
                    int middle = count / 2 + count%2;
                    int i = 0;
                    for (i = 0; i < middle; ++i)
                    {
                        left.Add(i, i);
                    }
                    for (; i < count; ++i)
                        right.Add(i, i);
                }

                public void Swap(int i, int j)
                {
                    left.Remove(i);
                    right.Remove(j);
                    left.Add(j, j);
                    right.Add(i, i);
                }

                public SortedList Left
                {
                    get
                    {
                        return this.left;
                    }
                }
                public SortedList Right
                {
                    get
                    {
                        return this.right;
                    }
                }

                public override string ToString()
                {
                    StringWriter sw = new StringWriter();
                    sw.Write("[");
                    foreach (object o in this.Left.Keys)
                        sw.Write("{0} ",o);
                    sw.Write("] [");
                    foreach (object o in this.Right.Keys)
                        sw.Write("{0} ", o);
                    sw.Write("]"); 
                    return sw.ToString();
                }
            }
        }
    }
}
