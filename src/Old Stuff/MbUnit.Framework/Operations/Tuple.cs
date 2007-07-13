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

namespace TestFu.Operations
{
    public class Tuple : CollectionBase, ITuple, IComparable
    {
        public Tuple()
        {}

        public object this[int index]
        {
            get
            {
                return this.List[index];
            }
        }

        public void Add(Object o)
        {
            this.InnerList.Add(o);
        }

        public void Concat(ITuple tuple)
        {
            foreach (Object o in tuple)
                this.Add(o);
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            foreach (object item in this)
                sw.Write("{0}, ", item);
            return sw.ToString().TrimEnd(',',' ');
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            ITuple tuple = obj as ITuple;
            if (tuple == null)
                return false;
            return this.CompareTo(tuple) == 0;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 0;
            foreach (object o in this)
            {
                if (o != null)
                    hash += o.GetHashCode();
            }
            return hash;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as ITuple);
        }

        public int CompareTo(ITuple tuple)
        {
            if (((object)tuple) == null)
                return -1;
            if (this.Count < tuple.Count)
                return -1;
            else if (this.Count > tuple.Count)
                return 1;
            for (int i = 0; i < this.Count; ++i)
            {
                int c = Comparer.Default.Compare(this[i], tuple[i]);
                if (c != 0)
                    return c;
            }
            return 0;
        }

        public Object[] ToObjectArray()
        {
            object[] objs = new object[this.Count];
            this.List.CopyTo(objs, 0);
            return objs;
        }
    }
}

