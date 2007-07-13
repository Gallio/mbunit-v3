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

using System.Collections;
using System;

namespace TestFu.Operations
{
    public class EmptyDomain : IDomain
    {
        private string name = null;
        public EmptyDomain()
        {}

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name=value;
            }
        }

        public IDomain Boundary
        {
            get
            {
                return this;
            }
        }

        public int Count
        {
            get
            {
                return 0;
            }
        }

        public object this[int i]
        {
            get
            {
                throw new InvalidOperationException("Empty domain");
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new ArrayList().GetEnumerator();
        }
    }
}
