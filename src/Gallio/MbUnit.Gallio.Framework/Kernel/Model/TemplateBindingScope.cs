using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A template binding scope provides a mechanism for template bindings to
    /// access state provided by containing template bindings.
    /// </summary>
    public class TemplateBindingScope
    {
        private readonly TemplateBindingScope parent;

        /// <summary>
        /// Creates a template binding scope as a child of another scope.
        /// </summary>
        /// <param name="parent">The parent scope, or null if none</param>
        public TemplateBindingScope(TemplateBindingScope parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the parent scope, or null if none.
        /// </summary>
        public TemplateBindingScope Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Produces bindings of a template by producing arguments for template parameters
        /// based on the data sources available in the scope.
        /// </summary>
        /// <param name="template">The template to bind</param>
        /// <returns>The bound templates</returns>
        public IEnumerable<ITemplateBinding> Bind(ITemplate template)
        {
            TemplateBindingScope nestedScope = new TemplateBindingScope(this);

            //DataBinding.DataBinding[] bindings = ListUtils.ConvertAllToArray<ITemplateParameter, DataBinding.DataBinding>(template.Parameters, CreateDataBindingFromParameter);

            if (template.Parameters.Count == 0)
                yield return template.Bind(nestedScope, EmptyDictionary<ITemplateParameter, IDataFactory>.Instance);
        }

        /*
        private DataBinding.DataBinding CreateDataBindingFromParameter(ITemplateParameter parameter)
        {
        }
         */
    }
}
