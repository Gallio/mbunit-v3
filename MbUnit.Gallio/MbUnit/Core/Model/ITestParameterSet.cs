using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// <para>
    /// A parameter set groups one or more parameters logically and associates
    /// common meta-data with them.  A parameter set may be used for documentation
    /// purposes, or it take part in the data binding process by defining a
    /// group of parameters whose values are jointly drawn from a common source
    /// with multiple correlated values such as a set of columns within a single row
    /// of a table.
    /// </para>
    /// <para>
    /// All parameters belong to some parameter set.  If the name of the parameter
    /// set is empty, the set is considered anonymous.
    /// </para>
    /// </summary>
    public interface ITestParameterSet : ITestComponent
    {
        /// <summary>
        /// Gets the list of parameters in a parameter set.
        /// </summary>
        /// <remarks>
        /// The order in which the parameters appear is not significant and does
        /// not necessarily correspond to the sequence of <see cref="ITestParameter.Index" /> values.
        /// </remarks>
        IList<ITestParameter> Parameters { get; }
    }
}
