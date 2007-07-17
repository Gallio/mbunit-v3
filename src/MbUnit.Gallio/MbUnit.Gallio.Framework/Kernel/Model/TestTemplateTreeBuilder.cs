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
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
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
        private TestProject project;
        private TestTemplateGroup root;
        private IDictionary<object, ITestTemplate> registry;

        /// <summary>
        /// Creates a test template tree builder initially populated with
        /// a root template.
        /// </summary>
        /// <param name="project">The test project</param>
        public TestTemplateTreeBuilder(TestProject project)
        {
            this.project = project;

            root = CreateRoot();
            registry = new Dictionary<object, ITestTemplate>();
        }

        /// <summary>
        /// Gets the test project.
        /// </summary>
        public TestProject Project
        {
            get { return project; }
        }

        /// <summary>
        /// Gets the root node of the test template tree.
        /// </summary>
        public TestTemplateGroup Root
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

        //public event EventHandler PostProcess;

        private static TestTemplateGroup CreateRoot()
        {
            TestTemplateGroup root = new TestTemplateGroup("Root", CodeReference.Unknown);
            root.Kind = TemplateKind.Root;
            return root;
        }
    }
}
