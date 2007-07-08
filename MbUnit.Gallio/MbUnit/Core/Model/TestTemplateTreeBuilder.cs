using System;
using System.Collections.Generic;
using MbUnit.Core.Metadata;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// The builder for a test template tree.  The builder holds the root of
    /// the tree and provides a registry for cooperatively resolving internal
    /// references among templates within the tree.  When internal references
    /// are present, two passes through the tree must be made to resolve
    /// them.  The tree builder supports this lifecycle directly by providing
    /// callbacks to perform fixup activities once all templates have been
    /// registered.
    /// </summary>
    public class TestTemplateTreeBuilder
    {
        private ITestTemplate root;
        private IDictionary<object, ITestTemplate> registry;

        /// <summary>
        /// Creates a test template tree builder initially populated with
        /// a root template.
        /// </summary>
        public TestTemplateTreeBuilder()
        {
            root = CreateRoot();
            registry = new Dictionary<object, ITestTemplate>();
        }

        /// <summary>
        /// Gets the root node of the test template tree.
        /// </summary>
        public ITestTemplate Root
        {
            get { return root; }
        }

        /// <summary>
        /// Registers a given template with a given key (such as a type).
        /// This allows the template to be referenced elsewhere.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="template">The template to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or
        /// <paramref name="template"/> is null</exception>
        public void RegisterTemplate(object key, ITestTemplate template)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (template == null)
                throw new ArgumentNullException("template");

            registry.Add(key, template);
        }

        /// <summary>
        /// Gets the template registered with the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The registered template, or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        public ITestTemplate GetTemplate(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            ITestTemplate template;
            return registry.TryGetValue(key, out template) ? template : null;
        }

        /// <summary>
        /// Links a template into a template tree.
        /// </summary>
        /// <param name="parent">The parent template already within the tree</param>
        /// <param name="child">The child template</param>
        public static void LinkTemplate(ITestTemplate parent, ITestTemplate child)
        {
            if (child.Parent != null)
                throw new ArgumentNullException("The child is already in the tree.");

            child.Parent = parent;
            parent.Children.Add(child);
        }

        //public event EventHandler PostProcess;

        private static ITestTemplate CreateRoot()
        {
            ITestTemplate root = new BaseTestTemplate("Root", CodeReference.Unknown);
            root.Metadata.Entries.Add(MetadataConstants.TemplateKindKey, TemplateKind.Root);
            return root;
        }
    }
}
