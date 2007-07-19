using System;
using System.Collections.Generic;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// The builder for a tree of model objects.  The builder holds the root of
    /// the tree and provides a registry for cooperatively resolving internal
    /// references among objects within the tree.  When internal references
    /// are present, two passes through the tree must be made to resolve
    /// them.  The tree builder supports this lifecycle directly by providing
    /// callbacks to perform fixup activities once all objects have been
    /// registered.
    /// </summary>
    public class ModelTreeBuilder<T> where T : class, IModelTreeNode<T>
    {
        private ITestHarness harness;
        private T root;
        private Dictionary<object, T> registry;

        /// <summary>
        /// Creates a tree builder initially populated with the specified root node.
        /// </summary>
        /// <param name="harness">The test harness</param>
        /// <param name="root">The root of the tree</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="harness"/> or
        /// <paramref name="root"/> is null</exception>
        public ModelTreeBuilder(ITestHarness harness, T root)
        {
            if (harness == null)
                throw new ArgumentNullException("harness");
            if (root == null)
                throw new ArgumentNullException("root");

            this.harness = harness;
            this.root = root;

            registry = new Dictionary<object, T>();
        }

        /// <summary>
        /// Gets the test harness.
        /// </summary>
        public ITestHarness Harness
        {
            get { return harness; }
        }

        /// <summary>
        /// Gets the root node of the tree.
        /// </summary>
        public T Root
        {
            get { return root; }
        }

        /// <summary>
        /// This event is fired once all nodes have been added to the tree to
        /// allow internal cross-references among nodes to be resolved.  New nodes
        /// should not be added to the tree at this time.
        /// </summary>
        public event EventHandler<EventArgs> ResolveReferences;

        /// <summary>
        /// This event is fired after <see cref="ResolveReferences" /> to finalize
        /// any remaining node construction issues.  New nodes should not
        /// be added to the tree at this time.
        /// </summary>
        public event EventHandler<EventArgs> PostProcess;

        /// <summary>
        /// Registers a given node with a given key (such as a type).
        /// This allows the node to be referenced elsewhere.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="node">The node to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or
        /// <paramref name="node"/> is null</exception>
        public void RegisterNode(object key, T node)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (node == null)
                throw new ArgumentNullException("node");

            registry.Add(key, node);
        }

        /// <summary>
        /// Gets the node registered with the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The registered node, or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        public T GetNode(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            T node;
            return registry.TryGetValue(key, out node) ? node : null;
        }

        /// <summary>
        /// Once all declarative node information has been added to the tree, this method
        /// should be called to run the <see cref="ResolveReferences" />
        /// and <see cref="PostProcess" /> event handlers.
        /// </summary>
        /// <remarks>
        /// The event handler references are cleared by this method since they are 
        /// no longer of any use.
        /// </remarks>
        public void FinishBuilding()
        {
            if (ResolveReferences != null)
            {
                ResolveReferences(this, EventArgs.Empty);
                ResolveReferences = null;
            }

            if (PostProcess != null)
            {
                PostProcess(this, EventArgs.Empty);
                PostProcess = null;
            }
        }
    }
}