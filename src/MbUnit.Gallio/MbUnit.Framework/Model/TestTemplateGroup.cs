using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Model.Metadata;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// A test template group is a test template that aggregates a collection
    /// of related templates under some common parent.  It supports the
    /// addition of arbitrary templates as children.
    /// </summary>
    public class TestTemplateGroup : BaseTestTemplate
    {
        private List<ITestTemplate> children;

        /// <summary>
        /// Initializes an empty test template group.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public TestTemplateGroup(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
            children = new List<ITestTemplate>();
            Kind = TemplateKind.Group;
        }

        /// <inheritdoc />
        public override IEnumerable<ITestTemplate> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets the children of this test template as a list.
        /// </summary>
        public IList<ITestTemplate> ChildrenList
        {
            get { return children; }
        }

        /// <inheritdoc />
        public override void AddChild(ITestTemplate template)
        {
            ModelUtils.LinkTemplate(this, children, template);
        }
    }
}
