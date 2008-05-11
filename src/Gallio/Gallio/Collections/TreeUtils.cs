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

namespace Gallio.Collections
{
    /// <summary>
    /// Provides functions for manipulating ad-hoc trees.
    /// </summary>
    public static class TreeUtils
    {
        /// <summary>
        /// Enumerates child nodes in a tree.
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>The node's children</returns>
        public delegate IEnumerable<T> ChildEnumerator<T>(T node);

        /// <summary>
        /// Gets an enumeration of a tree data structure in pre-order traversal.
        /// </summary>
        /// <typeparam name="T">The type of node in the tree</typeparam>
        /// <param name="rootNode">The root node of the tree</param>
        /// <param name="enumerator">A function that yields an enumeration of the children
        /// of a node in the tree</param>
        /// <returns>The pre-order traversal enumeration</returns>
        public static IEnumerable<T> GetPreOrderTraversal<T>(T rootNode, ChildEnumerator<T> enumerator)
        {
            yield return rootNode;

            Stack<IEnumerator<T>> stack = new Stack<IEnumerator<T>>();
            stack.Push(enumerator(rootNode).GetEnumerator());

            do
            {
                IEnumerator<T> children = stack.Peek();

                if (children.MoveNext())
                {
                    T child = children.Current;
                    yield return child;

                    stack.Push(enumerator(child).GetEnumerator());
                }
                else
                {
                    stack.Pop();
                }
            }
            while (stack.Count != 0);
        }
    }
}