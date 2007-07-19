using System;
using System.Collections.Generic;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITest" />.
    /// </summary>
    /// <remarks>
    /// The base test implementation acts as a simple container for tests.
    /// Accordingly its kind is set to <see cref="ComponentKind.Group" /> by default.
    /// </remarks>
    public class BaseTest : BaseTestComponent, ITest
    {
        private ITest parent;
        private List<ITest> children;
        private List<ITest> dependencies;
        private ITemplateBinding templateBinding;
        private TestScope scope;

        /// <summary>
        /// Initializes a template initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="parentScope">The parent scope, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public BaseTest(string name, CodeReference codeReference, TestScope parentScope)
            : base(name, codeReference)
        {
            scope = new TestScope(parentScope, this);
            dependencies = new List<ITest>();
            children = new List<ITest>();

            Kind = ComponentKind.Group;
        }

        /// <inheritdoc />
        public ITest Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <inheritdoc />
        public ITemplateBinding TemplateBinding
        {
            get { return templateBinding; }
            set { templateBinding = value; }
        }

        /// <inheritdoc />
        public virtual IList<ITest> Children
        {
            get { return children; }
        }

        /// <inheritdoc />
        public virtual IList<ITest> Dependencies
        {
            get { return dependencies; }
        }

        /// <inheritdoc />
        public virtual TestScope Scope
        {
            get { return scope; }
        }

        /// <inheritdoc />
        public virtual void AddChild(ITest test)
        {
            ModelUtils.Link<ITest>(this, test);
        }
    }
}