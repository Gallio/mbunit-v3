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
using MbUnit.Framework.Kernel.DataBinding;

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
        private readonly ITemplate template;
        private readonly TemplateBindingScope scope;
        private readonly IDictionary<ITemplateParameter, IDataFactory> arguments;

        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="arguments"/> does not
        /// bind all template parameters</exception>
        public BaseTemplateBinding(ITemplate template, TemplateBindingScope scope,
            IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            if (template == null)
                throw new ArgumentNullException(@"template");
            if (scope == null)
                throw new ArgumentNullException(@"scope");
            if (arguments == null)
                throw new ArgumentNullException(@"arguments");

            ValidateArguments(template, arguments);

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
        public TemplateBindingScope Scope
        {
            get { return scope; }
        }

        /// <inheritdoc />
        public IDictionary<ITemplateParameter, IDataFactory> Arguments
        {
            get { return arguments; }
        }

        /// <inheritdoc />
        public virtual void BuildTests(TestTreeBuilder builder, ITest parent)
        {
            // The base implementation builds a test node for the template and recurses
            // into the children.
            BaseTest test = new BaseTest(template.Name, template.CodeReference, this);
            test.Kind = null;
            test.Metadata.Entries.AddAll(template.Metadata.Entries);

            parent.AddChild(test);

            BuildTestsForGenerativeChildren(builder, test);
        }

        /// <summary>
        /// Builds tests for children templates for all children with <see cref="ITemplate.IsGenerator" />
        /// set to true.
        /// </summary>
        /// <param name="builder">The test tree builder</param>
        /// <param name="parent">The parent test for the children</param>
        protected virtual void BuildTestsForGenerativeChildren(TestTreeBuilder builder, ITest parent)
        {
            foreach (ITemplate childTemplate in template.Children)
            {
                if (childTemplate.IsGenerator)
                {
                    foreach (ITemplateBinding childBinding in scope.Bind(childTemplate))
                    {
                        childBinding.BuildTests(builder, parent);
                    }
                }
            }
        }

        private static void ValidateArguments(ITemplate template, IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            if (template.Parameters.Count != arguments.Count)
                throw new ArgumentException(String.Format(
                    Resources.BaseTemplateBinding_InvalidNumberOfTemplateArguments, template.Name,
                    template.Parameters.Count, arguments.Count), @"arguments");

            foreach (ITemplateParameter parameter in template.Parameters)
                if (!arguments.ContainsKey(parameter))
                    throw new ArgumentException(String.Format(
                    Resources.BaseTemplateBinding_MissingArgumentForTemplateParameter,
                    parameter.Name, template.Name), @"arguments");
        }
    }
}
