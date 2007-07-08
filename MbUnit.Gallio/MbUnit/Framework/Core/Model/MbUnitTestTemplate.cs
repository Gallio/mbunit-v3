using System;
using MbUnit.Core.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// Abstract base class for MbUnit-derived test templates.
    /// </summary>
    public abstract class MbUnitTestTemplate : BaseTestTemplate
    {
        /// <summary>
        /// Initializes a test template initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public MbUnitTestTemplate(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
        }

        /// <summary>
        /// Creates an anonymous parameter set associated with this template.
        /// </summary>
        /// <returns>The parameter set</returns>
        public MbUnitTestParameterSet CreateAnonymousParameterSet()
        {
            MbUnitTestParameterSet parameterSet = new MbUnitTestParameterSet(this, "", CodeReference);
            ParameterSets.Add(parameterSet);
            return parameterSet;
        }

        /// <summary>
        /// Sets the parameter set of a parameter by name.
        /// Automatically creates a parameter set if none with the specified name exists.
        /// Automatically deletes empty anonymous parameter sets.
        /// </summary>
        /// <param name="parameter">The parameter to move to a different parameter set</param>
        /// <param name="parameterSetName">The parameter set name</param>
        public void SetParameterSetName(MbUnitTestParameter parameter, string parameterSetName)
        {
            if (parameter.ParameterSet.Name == parameterSetName)
                return;

            if (!ParameterSets.Contains(parameter.ParameterSet))
                throw new InvalidOperationException("The parameter does not belong to any of this template's parameter sets.");

            // Remote the parameter from its old set.
            if (!parameter.ParameterSet.Parameters.Remove(parameter))
                throw new InvalidOperationException("The parameter does not belong to its own parameter set!");

            // Remove old empty anonymous parameter sets created during intermediate
            // stages of test enumeration.
            if (parameter.ParameterSet.Name.Length == 0 && parameter.ParameterSet.Parameters.Count == 0)
                ParameterSets.Remove(parameter.ParameterSet);

            // Add the parameter to its new set.
            ITestParameterSet parameterSet = GetParameterSetByName(parameterSetName);
            if (parameterSet == null)
            {
                parameterSet = new MbUnitTestParameterSet(this, parameterSetName, CodeReference);
                ParameterSets.Add(parameterSet);
            }

            parameterSet.Parameters.Add(parameter);
        }
    }
}
