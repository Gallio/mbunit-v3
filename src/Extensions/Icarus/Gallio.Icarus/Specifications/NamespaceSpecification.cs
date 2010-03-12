// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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

using Gallio.Common.Reflection;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;

namespace Gallio.Icarus.Specifications
{
    public class NamespaceSpecification : Specification<TestTreeNode>
    {
        private readonly string @namespace;

        public NamespaceSpecification(string @namespace)
        {
            this.@namespace = @namespace;
        }

        public string MatchText
        {
            get { return @namespace; }
        }

        public override bool Matches(TestTreeNode item)
        {
            var node = item as TestDataNode;

            if (node == null)
                return false;

            return Matches(node);
        }

        private bool Matches(TestDataNode node)
        {
            if (node.CodeReference == CodeReference.Unknown)
                return false;

            var namespaceName = node.CodeReference.NamespaceName;

            if (namespaceName == null)
                return false;

            return CaseInsensitiveContains(namespaceName, 
                @namespace);
        }
    }
}
