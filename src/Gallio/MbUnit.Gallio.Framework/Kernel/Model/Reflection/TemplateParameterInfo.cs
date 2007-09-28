using System;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="ITemplateParameter" /> for reflection.
    /// </summary>
    public sealed class TemplateParameterInfo : ModelComponentInfo, ITemplateParameter
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateParameterInfo(ITemplateParameter source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public Type Type
        {
            get { return Source.Type; }
        }
        Type ITemplateParameter.Type
        {
            get { return Type; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public int Index
        {
            get { return Source.Index; }
        }
        int ITemplateParameter.Index
        {
            get { return Index; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        new internal ITemplateParameter Source
        {
            get { return (ITemplateParameter)base.Source; }
        }
    }
}