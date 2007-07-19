using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Represents the root template in the template tree.
    /// </summary>
    public class RootTemplate : BaseTemplate
    {
        /// <summary>
        /// Creates the root template.
        /// </summary>
        public RootTemplate()
            : base("Root", CodeReference.Unknown)
        {
            Kind = ComponentKind.Root;
        }

        /// <inheritdoc />
        public override ITemplateBinding Bind(TestScope scope, IDictionary<ITemplateParameter, object> arguments)
        {
            return new RootTemplateBinding(this, scope, arguments);
        }
    }
}
