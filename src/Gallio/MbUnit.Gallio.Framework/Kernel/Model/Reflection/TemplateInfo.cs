using System;
using System.Collections.Generic;
using MbUnit.Framework.Kernel.DataBinding;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="ITemplate" /> for reflection.
    /// </summary>
    public sealed class TemplateInfo : ModelComponentInfo, ITemplate
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateInfo(ITemplate source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public bool IsGenerator
        {
            get { return Source.IsGenerator; }
        }
        bool ITemplate.IsGenerator
        {
            get { return IsGenerator; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TemplateParameterInfoList Parameters
        {
            get { return new TemplateParameterInfoList(Source.Parameters); }
        }
        IList<ITemplateParameter> ITemplate.Parameters
        {
            get { return Parameters.AsModelList(); }
        }

        /// <inheritdoc />
        public TemplateInfo Parent
        {
            get { return Source.Parent != null ? new TemplateInfo(Source.Parent) : null; }
        }
        ITemplate IModelTreeNode<ITemplate>.Parent
        {
            get { return Parent; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TemplateInfoList Children
        {
            get { return new TemplateInfoList(Source.Children); }
        }
        IList<ITemplate> IModelTreeNode<ITemplate>.Children
        {
            get { return Children.AsModelList(); }
        }

        void IModelTreeNode<ITemplate>.AddChild(ITemplate node)
        {
            throw new NotSupportedException();
        }

        ITemplateBinding ITemplate.Bind(TemplateBindingScope scope, IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        new internal ITemplate Source
        {
            get { return (ITemplate)base.Source; }
        }
    }
}