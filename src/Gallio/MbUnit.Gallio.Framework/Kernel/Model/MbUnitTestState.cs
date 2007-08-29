using System;
using System.Collections.Generic;
using MbUnit.Framework.Kernel.DataBinding;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Captures the run-time state of a test.
    /// </summary>
    public class MbUnitTestState
    {
        private MbUnitTest test;
        private object fixtureInstance;
        private IDataBindingContext dataBindingContext;
        private Dictionary<ITemplateParameter, object> parameterValues;

        /// <summary>
        /// Creates the state for a test.
        /// </summary>
        /// <param name="test">The test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        public MbUnitTestState(MbUnitTest test)
        {
            if (test == null)
                throw new ArgumentNullException(@"test");

            this.test = test;
        }

        /// <summary>
        /// Gets the test.
        /// </summary>
        public MbUnitTest Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets or sets the test fixture instance or null if none.
        /// </summary>
        public object FixtureInstance
        {
            get { return fixtureInstance; }
            set { fixtureInstance = value; }
        }

        /// <summary>
        /// Gets the value of the specified template parameter.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>The value of the parameter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="parameter"/> does not reference
        /// a valid parameter of the template</exception>
        /// <exception cref="DataBindingException">Thrown if data binding fails</exception>
        public object GetParameterValue(ITemplateParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(@"parameter");

            object value;
            if (parameterValues != null)
            {
                if (parameterValues.TryGetValue(parameter, out value))
                    return value;
            }
            else
            {
                parameterValues = new Dictionary<ITemplateParameter, object>();
            }

            IDataFactory dataFactory;
            if (!test.TemplateBinding.Arguments.TryGetValue(parameter, out dataFactory))
                throw new ArgumentException(String.Format(Resources.MbUnitTestState_TemplateBindingDoesNotContainParameter, parameter.Name), "parameter");

            try
            {
                value = dataFactory.GetValue(dataBindingContext);
            }
            catch (Exception ex)
            {
                throw new DataBindingException(String.Format(Resources.MbUnitTestState_ErrorWhileBindingValueForParameter, parameter.Name), ex);
            }

            parameterValues.Add(parameter, value);
            return value;
        }
    }
}
