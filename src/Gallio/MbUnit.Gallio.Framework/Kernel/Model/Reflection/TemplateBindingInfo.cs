using System;
using System.Collections.Generic;
using MbUnit.Framework.Kernel.DataBinding;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="ITemplateBinding" /> for reflection.
    /// </summary>
    public sealed class TemplateBindingInfo : BaseInfo, ITemplateBinding
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateBindingInfo(ITemplateBinding source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public TemplateInfo Template
        {
            get { return new TemplateInfo(Source.Template); }
        }
        ITemplate ITemplateBinding.Template
        {
            get { throw new NotSupportedException(); }
        }

        TemplateBindingScope ITemplateBinding.Scope
        {
            get { throw new NotSupportedException(); }
        }

        IDictionary<ITemplateParameter, IDataFactory> ITemplateBinding.Arguments
        {
            get { throw new NotSupportedException(); }
        }

        void ITemplateBinding.BuildTests(TestTreeBuilder builder, ITest parent)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        new internal ITemplateBinding Source
        {
            get { return (ITemplateBinding)base.Source; }
        }
    }
}