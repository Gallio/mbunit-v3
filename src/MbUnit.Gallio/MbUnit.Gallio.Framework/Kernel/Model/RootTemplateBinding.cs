using System;
using System.Collections.Generic;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// The root template binding.
    /// </summary>
    public class RootTemplateBinding : BaseTemplateBinding
    {
        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public RootTemplateBinding(ITemplate template, TestScope scope,
                                   IDictionary<ITemplateParameter, object> arguments)
            : base(template, scope, arguments)
        {
        }

        /// <inheritdoc />
        public override void BuildTests(TestTreeBuilder builder)
        {
            Scope.ContainingTest.Metadata.Entries.AddAll(Template.Metadata.Entries);

            BuildTestsForChildren(Scope, builder);
        }
    }
}