using System;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="IModelComponent" /> for reflection.
    /// </summary>
    public abstract class ModelComponentInfo : BaseInfo, IModelComponent
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        internal ModelComponentInfo(IModelComponent source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public string Id
        {
            get { return Source.Id; }
        }
        string IModelComponent.Id
        {
            get { return Id; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return Source.Name; }
        }
        string IModelComponent.Name
        {
            get { return Name; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get { return Source.Metadata.Copy(); }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return Source.CodeReference.Copy(); }
        }
        CodeReference IModelComponent.CodeReference
        {
            get { return CodeReference; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        new internal IModelComponent Source
        {
            get { return (IModelComponent)base.Source; }
        }
    }
}