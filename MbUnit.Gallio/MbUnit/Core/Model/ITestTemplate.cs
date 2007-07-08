using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// <para>
    /// A template is an abstract description of a test component.
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
    /// A template may have zero or more <see cref="ITestParameter" />s.
    /// Furthermore, each <see cref="ITestParameter" /> belongs to a
    /// <see cref="ITestParameterSet" />.  Values must be bound to each
    /// parameter when templates are specialized during test enumeration.
    /// The result of template specialization is a <see cref="ITestTemplateBinding" />
    /// that contains the actual values that were bound to each parameter.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// During test enumeration, the template adds each <see cref="ITest" />s to a
    /// test graph.  The template can inject its own contributions in and around
    /// those of the templates by modifying the <see cref="ITestScope" /> it passes
    /// to the inner templates, or by instrumenting the <see cref="ITestGraphBuilder" />.
    /// Thus a template can effect great control over its constituent parts.
    /// </para>
    /// <para>
    /// It can happen that a template is never enumerated, possibly because no values
    /// were specified for its parameters in the enclosing context or because its
    /// enumeration was skipped due to filtering or other means.  In this case,
    /// the template does not contribute anything to the test graph but it will
    /// still be visible to runtime reflection.
    /// </para>
    /// <para>
    /// Refer to <see cref="ITestScope" /> for information on how a <see cref="IDataProvider" />
    /// can be used to provide values for template parameters.
    /// </para>
    /// </remarks>
    public interface ITestTemplate : ITestComponent
    {
        /// <summary>
        /// Gets the parent of this test template, or null if this template
        /// is at the root of the template tree.
        /// </summary>
        ITestTemplate Parent { get; set; }

        /// <summary>
        /// Gets the children of this test template.
        /// </summary>
        IList<ITestTemplate> Children { get; }

        /// <summary>
        /// Gets the parameter sets that belong to this test template.
        /// Each parameter set must have a unique name.  The order in which
        /// the parameter sets appear is not significant.
        /// </summary>
        IList<ITestParameterSet> ParameterSets { get; }

        /*
        ITestScope Scope { get; }
        
        void BuildTests(ITestGraphBuilder builder, ITestScope scope, IDictionary<ITestParameter, object> parameterValues);
        */
    }
}
