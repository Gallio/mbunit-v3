using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Interface implemented by a node in a tree of model objects.
    /// </summary>
    /// <typeparam name="T">The node type</typeparam>
    public interface IModelTreeNode<T> where T : class, IModelTreeNode<T>
    {
        /// <summary>
        /// Gets or sets the parent of this node, or null if this node is the root of the tree.
        /// </summary>
        T Parent { get; set; }

        /// <summary>
        /// Gets the list of the children of this node.
        /// </summary>
        IList<T> Children { get; }

        /// <summary>
        /// Adds a child node and sets its parent property to refer to this node.
        /// </summary>
        /// <param name="node">The node to add</param>
        void AddChild(T node);
    }
}
