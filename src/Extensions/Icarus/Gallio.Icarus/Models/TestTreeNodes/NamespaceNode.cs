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

using System.Collections.Generic;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Models.TestTreeNodes
{
    public sealed class NamespaceNode : TestTreeNode, ITestDescriptor
    {
        public NamespaceNode(string @namespace, string text)
            : base(@namespace, text)
        {
            CheckState = System.Windows.Forms.CheckState.Checked;
            CodeElement = Reflector.WrapNamespace(@namespace);
            Metadata = new PropertyBag();
        }

        public IEnumerable<TestTreeNode> GetChildren()
        {
            var nodes = new List<TestTreeNode>();
            foreach (var node in Nodes)
            {
                var testTreeNode = node as TestTreeNode;
                if (testTreeNode != null)
                    nodes.Add(testTreeNode);
            }
            return nodes;
        }

        public override string TestKind
        {
            get
            {
                return TestKinds.Namespace;
            }
        }

        public string Name
        {
            get { return CodeElement.Name; }
        }

        public ICodeElementInfo CodeElement { get; private set; }

        public PropertyBag Metadata { get; private set; }
    }
}
