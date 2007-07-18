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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// <para>
    /// A template is an abstract parameterized description of some part of a test.
    /// </para>
    /// <para>
    /// During test enumeration, a tree of templates is constructed to
    /// describe the test assemblies, fixtures, methods, and other entities
    /// that make up the test.  The tree reflects the logical structure
    /// in which the tests were defined.  Typically the tree will be
    /// traversed when building the final test graph during test enumeration
    /// but the order in which tests are executed is not required to
    /// correspond to the hierarchical arrangement of templates in the tree.
    /// </para>
    /// <para>
    /// A template may have zero or more <see cref="ITemplateParameter" />s.
    /// Furthermore, each <see cref="ITemplateParameter" /> belongs to a
    /// <see cref="ITemplateParameterSet" />.  Values must be bound to each
    /// parameter when templates are specialized during test enumeration.
    /// The result of template specialization is a <see cref="ITemplateBinding" />
    /// that contains the actual values that were bound to each parameter.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// During test enumeration, the template contributes <see cref="ITest" />s to a
    /// <see cref="TestTreeBuilder" />.  The template can inject its own contributions
    /// in and around those of the templates by modifying the <see cref="TestScope" /> it passes
    /// to the inner templates, or by instrumenting the <see cref="TestTreeBuilder" />
    /// appropriates.  Thus a template can effect great control over test construction.
    /// </para>
    /// <para>
    /// It can happen that a template is never asked to produce tests, possibly because
    /// it has some other purpose, or because no values were specified for its parameters
    /// in the enclosing context.  In this case, the template does not contribute anything
    /// to the test tree but it will still be visible in the template tree.
    /// </para>
    /// <para>
    /// Refer to <see cref="TestScope" /> for information on how a <see cref="IDataProvider" />
    /// can be used to provide values for template parameters.
    /// </para>
    /// </remarks>
    public interface ITemplate : ITemplateComponent
    {
        /// <summary>
        /// Gets or sets the parent of this template, or null if this template
        /// is at the root of the template tree.
        /// </summary>
        ITemplate Parent { get; set; }

        /// <summary>
        /// Gets the children of this template.
        /// </summary>
        IEnumerable<ITemplate> Children { get; }

        /// <summary>
        /// Gets the parameter sets that belong to this template.
        /// Each parameter set must have a unique name.  The order in which
        /// the parameter sets appear is not significant.
        /// </summary>
        IList<ITemplateParameterSet> ParameterSets { get; }

        //ITemplateBinding Bind();

        /*
        ITestScope Scope { get; }
        
        void BuildTests(ITestGraphBuilder builder, ITestScope scope, IDictionary<ITestParameter, object> parameterValues);
        */

        /// <summary>
        /// Adds a child template.
        /// Sets the child's parent to this template as part of the addition process.
        /// </summary>
        /// <param name="template">The template to add</param>
        /// <exception cref="NotSupportedException">Thrown if the template does not support
        /// the addition of arbitrary children (because it has some more specific internal structure)</exception>
        void AddChild(ITemplate template);
    }
}
