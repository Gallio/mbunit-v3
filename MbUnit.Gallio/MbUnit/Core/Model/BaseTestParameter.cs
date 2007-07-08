using System;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestParameter" />.
    /// </summary>
    public class BaseTestParameter : BaseTestComponent, ITestParameter
    {
        private ITestParameterSet parameterSet;
        private Type type;
        private int index;

        /// <summary>
        /// Initializes a test parameter.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="parameterSet">The parameter set to which the parameter belongs</param>
        /// <param name="type">The type of the parameter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, <paramref name="codeReference"/>,
        /// <paramref name="parameterSet"/> or <paramref name="type"/> is null</exception>
        public BaseTestParameter(string name, CodeReference codeReference, ITestParameterSet parameterSet, Type type)
            : base(name, codeReference)
        {
            if (parameterSet == null)
                throw new ArgumentNullException("parameterSet");
            if (type == null)
                throw new ArgumentNullException("type");

            this.parameterSet = parameterSet;
            this.type = type;
        }

        /// <inheritdoc />
        public ITestParameterSet ParameterSet
        {
            get { return parameterSet; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                parameterSet = value;
            }
        }

        /// <inheritdoc />
        public Type Type
        {
            get { return type; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                type = value;
            }
        }

        /// <inheritdoc />
        public int Index
        {
            get { return index; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("index");

                index = value;
            }
        }

        /// <inheritdoc />
        public override TestComponentInfo ToInfo()
        {
            TestParameterInfo info = new TestParameterInfo();
            PopulateInfo(info);
            return info;
        }

        /// <summary>
        /// Populates the component info structure with information about this component.
        /// </summary>
        /// <param name="info">The component info</param>
        protected void PopulateInfo(TestParameterInfo info)
        {
            base.PopulateInfo(info);

            info.TypeName = type.FullName;
            info.Index = index;
        }
    }
}
