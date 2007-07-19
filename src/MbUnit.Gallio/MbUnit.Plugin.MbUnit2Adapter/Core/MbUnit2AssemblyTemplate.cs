using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Plugin.MbUnit2Adapter.Core
{
    /// <summary>
    /// The MbUnit v2 assembly template.  Since MbUnit v2 does not support test
    /// parameters, we do not fully decompose the tests into a template tree.
    /// Instead, this template takes care of building all of the tests as needed.
    /// </summary>
    public class MbUnit2AssemblyTemplate : BaseTemplate
    {
        /// <summary>
        /// Initializes a template initially without a parent.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        public MbUnit2AssemblyTemplate(Assembly assembly)
            : base(assembly.FullName, CodeReference.CreateFromAssembly(assembly))
        {
            Kind = ComponentKind.Assembly;
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        public Assembly Assembly
        {
            get { return CodeReference.Assembly; }
        }

        /// <inheritdoc />
        public override ITemplateBinding Bind(TestScope scope, IDictionary<ITemplateParameter, object> arguments)
        {
            return new MbUnit2AssemblyTemplateBinding(this, scope, arguments);
        }
    }
}
