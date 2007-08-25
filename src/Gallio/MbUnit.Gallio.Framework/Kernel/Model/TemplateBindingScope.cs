using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
