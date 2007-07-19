using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// An MbUnit template binding.  MbUnit attributes contribute to the
    /// test construction process by attaching appropriate behavior to the
    /// template binding.
    /// </summary>
    public class MbUnitTemplateBinding : BaseTemplateBinding
    {
        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public MbUnitTemplateBinding(ITemplate template, TestScope scope,
            IDictionary<ITemplateParameter, object> arguments)
            : base(template, scope, arguments)
        {
        }
    }
}
