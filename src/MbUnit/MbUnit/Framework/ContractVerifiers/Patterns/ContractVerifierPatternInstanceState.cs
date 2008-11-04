using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Represents the run-time state of a single instance of a 
    /// test pattern that is to be executed in the scope
    /// of contract verifier.
    /// </summary>
    internal sealed class ContractVerifierPatternInstanceState : IContractVerifierPatternInstanceState
    {
        /// <summary>
        /// Gets the test fixture type or null if none.
        /// </summary>
        public Type FixtureType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the test fixture instance or null if none.
        /// </summary>
        public object FixtureInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fixtureType">Gets the test fixture type or null if none.</param>
        /// <param name="fixtureInstance">Gets the test fixture instance or null if none.</param>
        public ContractVerifierPatternInstanceState(Type fixtureType, object fixtureInstance)
        {
            this.FixtureType = fixtureType;
            this.FixtureInstance = fixtureInstance;
        }
    }
}
