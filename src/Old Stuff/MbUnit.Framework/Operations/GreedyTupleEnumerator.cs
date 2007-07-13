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
    internal class GreedyTupleEnumerable : ITupleEnumerable
    {
        private ITupleEnumerable enumerable;
        public GreedyTupleEnumerable(ITupleEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            this.enumerable = enumerable;
        }

        public ITupleEnumerator GetEnumerator()
        {
            return new GreedyTupleEnumerator(this.enumerable.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal class GreedyTupleEnumerator : ITupleEnumerator
        {
            private Hashtable tuples = new Hashtable();
            private ITupleEnumerator enumerator;

            public GreedyTupleEnumerator(ITupleEnumerator enumerator)
            {
                this.enumerator = enumerator;
            }

            public void Reset()
            {
                this.tuples.Clear();
                this.enumerator.Reset();
            }

            public bool MoveNext()
            {
                while (this.enumerator.MoveNext())
                {
                    if (this.tuples.Contains(this.Current))
                        continue;
                    this.tuples.Add(this.Current, null);
                    return true;
                }
                return false;
            }

            public ITuple Current
            {
                get
                {
                    return this.enumerator.Current;
                }
            }

            Object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }
        }
    }
}
