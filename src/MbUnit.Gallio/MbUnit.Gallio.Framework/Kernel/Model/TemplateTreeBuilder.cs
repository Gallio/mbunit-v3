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
using System.Collections.Generic;
using MbUnit.Framework.Kernel.Harness;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// The builder for a template tree.  The builder holds the root of
    /// the tree and provides a registry for cooperatively resolving internal
    /// references among templates within the tree.  When internal references
    /// are present, two passes through the tree must be made to resolve
    /// them.  The tree builder supports this lifecycle directly by providing
    /// callbacks to perform fixup activities once all templates have been
    /// registered.
    /// </summary>
    public class TemplateTreeBuilder
    {
        private ITestHarness harness;
        private TemplateGroup root;
        private IDictionary<object, ITemplate> registry;

        /// <summary>
        /// Creates a template tree builder initially populated with
        /// a root template.
        /// </summary>
        /// <param name="harness">The test harness</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="harness"/> is null</exception>
        public TemplateTreeBuilder(ITestHarness harness)
        {
            if (harness == null)
                throw new ArgumentNullException("harness");
            this.harness = harness;

            root = CreateRoot();
            registry = new Dictionary<object, ITemplate>();
        }

        /// <summary>
        /// Gets the test harness.
        /// </summary>
        public ITestHarness Harness
        {
            get { return harness; }
        }

        /// <summary>
        /// Gets the root node of the template tree.
        /// </summary>
        public TemplateGroup Root
        {
            get { return root; }
        }

        /// <summary>
        /// This event is fired once all templates have been added to the tree to
        /// allow cross-references among templates to be resolved.  New templates
        /// should not be added to the tree at this time.
        /// </summary>
        public event EventHandler<EventArgs> ResolveReferences;

        /// <summary>
        /// This event is fired after <see cref="ResolveReferences" /> to finalize
        /// any remaining template construction issues.  New templates should not
        /// be added to the tree at this time.
        /// </summary>
        public event EventHandler<EventArgs> PostProcess;

        /// <summary>
        /// Registers a given template with a given key (such as a type).
        /// This allows the template to be referenced elsewhere.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="template">The template to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or
        /// <paramref name="template"/> is null</exception>
        public void RegisterTemplate(object key, ITemplate template)
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
        public ITemplate GetTemplate(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            ITemplate template;
            return registry.TryGetValue(key, out template) ? template : null;
        }

        /// <summary>
        /// Once all declarative template information has been added to the template
        /// tree, this method should be called to run the <see cref="ResolveReferences" />
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

        private static TemplateGroup CreateRoot()
        {
            TemplateGroup root = new TemplateGroup("Root", CodeReference.Unknown);
            root.Kind = TemplateKind.Root;
            return root;
        }
    }
}
