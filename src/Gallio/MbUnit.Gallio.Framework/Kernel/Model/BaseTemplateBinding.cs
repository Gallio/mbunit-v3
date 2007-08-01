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
using System.Text;
using MbUnit.Framework.Kernel.Collections;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITemplateBinding" />
    /// </summary>
    /// <remarks>
    /// The base implementation of a template binding simply mirrors the bound
    /// template as a node in the test tree with identical metadata then recurses
    /// over its children to add them as sub-tests of this node.
    /// </remarks>
    public class BaseTemplateBinding : ITemplateBinding
    {
        private ITemplate template;
        private TestScope scope;
        private IDictionary<ITemplateParameter, object> arguments;

        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public BaseTemplateBinding(ITemplate template, TestScope scope,
            IDictionary<ITemplateParameter, object> arguments)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            this.template = template;
            this.scope = scope;
            this.arguments = arguments;
        }

        /// <inheritdoc />
        public ITemplate Template
        {
            get { return template; }
        }

        /// <inheritdoc />
        public TestScope Scope
        {
            get { return scope; }
        }

        /// <inheritdoc />
        public IDictionary<ITemplateParameter, object> Arguments
        {
            get { return arguments; }
        }

        /// <inheritdoc />
        public virtual void BuildTests(TestTreeBuilder builder)
        {
            // The base implementation builds a test node for the template and recurses
            // into the children.
            BaseTest test = new BaseTest(template.Name, template.CodeReference, scope);
            test.Kind = null;
            test.Metadata.Entries.AddAll(template.Metadata.Entries);

            scope.ContainingTest.AddChild(test);

            BuildTestsForChildren(test.Scope, builder);
        }

        /// <summary>
        /// Builds tests for children templates.
        /// </summary>
        /// <param name="parentScope">The containing scope for the tests created by the children</param>
        /// <param name="builder">The test tree builder</param>
        protected virtual void BuildTestsForChildren(TestScope parentScope, TestTreeBuilder builder)
        {
            foreach (ITemplate child in template.Children)
            {
                // Todo: get args from scope and use data providers
                ITemplateBinding binding = child.Bind(parentScope, EmptyDictionary<ITemplateParameter, object>.Instance);
                binding.BuildTests(builder);
            }
        }
    }
}
