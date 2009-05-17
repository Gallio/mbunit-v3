using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Provides traits for <see cref="ITestRunnerExtensionFactory" />.
    /// </summary>
    public class TestRunnerExtensionFactoryTraits : Traits
    {
        /// <summary>
        /// Gets or sets a condition (using the syntax of <see cref="Condition" /> that
        /// governs whether the extension provided by the factory should be automatically
        /// installed on every run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The condition may contain references to properties of the form ${env:VariableName}
        /// which test whether a given environment variable has been defined.
        /// </para>
        /// </remarks>
        public Condition AutoActivationCondition { get; set; }
    }
}
