using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Serialization;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestParameterSet" />.
    /// </summary>
    public class BaseTestParameterSet : BaseTestComponent, ITestParameterSet
    {
        private List<ITestParameter> parameters;

        /// <summary>
        /// Initializes a test parameter set.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition of the parameter set</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="codeReference"/> is null</exception>
        public BaseTestParameterSet(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
            parameters = new List<ITestParameter>();
        }

        /// <inheritdoc />
        public IList<ITestParameter> Parameters
        {
            get { return parameters; }
        }

        /// <inheritdoc />
        public override TestComponentInfo ToInfo()
        {
            TestParameterSetInfo info = new TestParameterSetInfo();
            PopulateInfo(info);
            return info;
        }

        /// <summary>
        /// Populates the component info structure with information about this component.
        /// </summary>
        /// <param name="info">The component info</param>
        protected void PopulateInfo(TestParameterSetInfo info)
        {
            base.PopulateInfo(info);

            info.Parameters = ListUtils.ConvertAllToArray<ITestParameter, TestParameterInfo>(parameters,
                delegate(ITestParameter parameter)
                {
                    return (TestParameterInfo)parameter.ToInfo();
                });
        }
    }
}
