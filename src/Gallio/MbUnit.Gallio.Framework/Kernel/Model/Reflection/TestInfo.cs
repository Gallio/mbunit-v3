using System;
using System.Collections.Generic;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="ITest" /> for reflection.
    /// </summary>
    public sealed class TestInfo : ModelComponentInfo, ITest
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestInfo(ITest source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public bool IsTestCase
        {
            get { return Source.IsTestCase; }
        }
        bool ITest.IsTestCase
        {
            get { return IsTestCase; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TemplateBindingInfo TemplateBinding
        {
            get { return new TemplateBindingInfo(Source.TemplateBinding); }
        }
        ITemplateBinding ITest.TemplateBinding
        {
            get { return TemplateBinding; }
        }

        /// <inheritdoc />
        public TestInfoList Dependencies
        {
            get { return new TestInfoList(Source.Dependencies); }
        }
        IList<ITest> ITest.Dependencies
        {
            get { return Dependencies.AsModelList(); }
        }

        /// <inheritdoc />
        public TestBatch Batch
        {
            get { return Source.Batch; }
        }
        TestBatch ITest.Batch
        {
            get { return Batch; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public ITest Parent
        {
            get { return Source.Parent != null ? new TestInfo(Source.Parent) : null; }
        }
        ITest IModelTreeNode<ITest>.Parent
        {
            get { return Parent; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TestInfoList Children
        {
            get { return new TestInfoList(Source.Children); }
        }
        IList<ITest> IModelTreeNode<ITest>.Children
        {
            get { return Children.AsModelList(); }
        }

        void IModelTreeNode<ITest>.AddChild(ITest node)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        new internal ITest Source
        {
            get { return (ITest)base.Source; }
        }
    }
}