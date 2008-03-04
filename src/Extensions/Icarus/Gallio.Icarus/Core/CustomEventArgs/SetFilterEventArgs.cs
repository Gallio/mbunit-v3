// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;

using Gallio.Model;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Core.CustomEventArgs 
{
    public class SetFilterEventArgs : EventArgs
    {
        private readonly string filterName;
        private readonly TreeNodeCollection nodes;
        private readonly Filter<ITest> filter;

        public SetFilterEventArgs(string filterName, TreeNodeCollection nodes)
        {
            this.filterName = filterName;
            this.nodes = nodes;
        }

        public SetFilterEventArgs(string filterName, Filter<ITest> filter)
        {
            this.filterName = filterName;
            this.filter = filter;
        }

        public string FilterName
        {
            get { return filterName; }
        }

        public TreeNodeCollection Nodes
        {
            get { return nodes; }
        }

        public Filter<ITest> Filter
        {
            get { return filter; }
        }
    }
}
