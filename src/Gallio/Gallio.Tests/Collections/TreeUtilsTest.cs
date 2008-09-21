// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Collections;
using MbUnit.Framework;

namespace Gallio.Tests.Collections
{
    [TestFixture]
    [TestsOn(typeof(TreeUtils))]
    public class TreeUtilsTest
    {
        [Test]
        public void GetPreOrderTraversal()
        {
            Node root = new Node(1,
                new Node(2, new Node(3), new Node(4), new Node(5)),
                new Node(6, new Node(7, new Node(8))),
                new Node(9));

            List<Node> nodes = new List<Node>(TreeUtils.GetPreOrderTraversal(root, delegate(Node node)
            {
                return node.Children;
            }));

            IList<int> ids = GenericUtils.ConvertAllToArray<Node, int>(nodes, delegate(Node node)
            {
                return node.Id;
            });

            Assert.AreElementsEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, ids);
        }

        private sealed class Node
        {
            private readonly int id;
            private readonly IEnumerable<Node> children;

            public Node(int id, params Node[] children)
            {
                this.id = id;
                this.children = children;
            }

            public int Id
            {
                get { return id; }
            }

            public IEnumerable<Node> Children
            {
                get { return children; }
            }
        }
    }
}
