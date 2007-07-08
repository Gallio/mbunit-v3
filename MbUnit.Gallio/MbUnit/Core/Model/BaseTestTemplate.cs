using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Serialization;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestTemplate" />.
    /// </summary>
    public class BaseTestTemplate : BaseTestComponent, ITestTemplate
    {
        private ITestTemplate parent;
        private List<ITestTemplate> children;
        private List<ITestParameterSet> parameterSets;

        /// <summary>
        /// Initializes a test template initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public BaseTestTemplate(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
            this.children = new List<ITestTemplate>();
            this.parameterSets = new List<ITestParameterSet>();
        }

        /// <inheritdoc />
        public ITestTemplate Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <inheritdoc />
        public IList<ITestTemplate> Children
        {
            get { return children; }
        }

        /// <inheritdoc />
        public IList<ITestParameterSet> ParameterSets
        {
            get { return parameterSets; }
        }

        /// <inheritdoc />
        public override TestComponentInfo ToInfo()
        {
            TestTemplateInfo info = new TestTemplateInfo();
            PopulateInfo(info);
            return info;
        }

        /// <summary>
        /// Gets the parameter set with the specified name, or null if none.
        /// Always returns null if the parameter set name is empty (anonymous).
        /// </summary>
        /// <param name="parameterSetName">The parameter set name</param>
        /// <returns>The parameter set</returns>
        public ITestParameterSet GetParameterSetByName(string parameterSetName)
        {
            if (parameterSetName.Length != 0)
            {
                foreach (ITestParameterSet parameterSet in parameterSets)
                    if (parameterSet.Name == parameterSetName)
                        return parameterSet;
            }

            return null;
        }

        /// <summary>
        /// Populates the component info structure with information about this component.
        /// </summary>
        /// <param name="info">The component info</param>
        protected void PopulateInfo(TestTemplateInfo info)
        {
            base.PopulateInfo(info);

            info.Children = ListUtils.ConvertAllToArray<ITestTemplate, TestTemplateInfo>(children,
                delegate(ITestTemplate child)
                {
                    return (TestTemplateInfo) child.ToInfo();
                });

            info.ParameterSets = ListUtils.ConvertAllToArray<ITestParameterSet, TestParameterSetInfo>(parameterSets,
                delegate(ITestParameterSet parameterSet)
                {
                    return (TestParameterSetInfo) parameterSet.ToInfo();
                });
        }
    }
}
