using System;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="IStep" /> for reflection.
    /// </summary>
    public sealed class StepInfo : BaseInfo, IStep
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public StepInfo(IStep source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public string Id
        {
            get { return Source.Id; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return Source.Name; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return Source.FullName; }
        }

        /// <inheritdoc />
        public StepInfo Parent
        {
            get { return Source.Parent != null ? new StepInfo(Source.Parent) : null; }
        }
        IStep IStep.Parent
        {
            get { return Parent; }
        }

        /// <inheritdoc />
        public TestInfo Test
        {
            get { return new TestInfo(Source.Test); }
        }
        ITest IStep.Test
        {
            get { return Test; }
        }

        /// <inheritdoc />
        new internal IStep Source
        {
            get { return (IStep)base.Source; }
        }
    }
}
