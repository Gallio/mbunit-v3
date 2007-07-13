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
    public class CollectionDomain : CollectionBase, IDomain
    {
        private string name = null;
        private IDomain boundary=null;

        public CollectionDomain()
        { }

        public CollectionDomain(ICollection collection)
        {
            foreach (Object item in collection)
                this.List.Add(item);
        }
        public CollectionDomain(IEnumerable enumerable)
        {
            foreach(Object item in enumerable)
                this.List.Add(item);
        }

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

        public object this[int i]
        {
            get
            {
                return this.List[i];
            }
        }

        public void AddDomain(IDomain domain)
        {
            if (domain == null)
                throw new ArgumentNullException("domain");
            this.AddRange(domain);
        }

        public void AddRange(IEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");
            this.boundary = null;
            foreach (Object o in enumerable)
                this.List.Add(o);
        }

        public IDomain Boundary
        {
            get
            {
                if (this.boundary == null)
                {
                    if (this.List.Count > 2)
                    {
                        this.boundary = Domains.ToDomain(this.List[0], this.List[this.List.Count - 1]);
                    }
                    else if (this.List.Count == 2)
                        return this;
                    else if (this.List.Count == 1)
                    {
                        this.boundary = Domains.ToDomain(this.List[0]);
                    }
                    else
                    {
                        this.boundary = new EmptyDomain();
                    }
                }

                return this.boundary;
            }
        }
    }
}
