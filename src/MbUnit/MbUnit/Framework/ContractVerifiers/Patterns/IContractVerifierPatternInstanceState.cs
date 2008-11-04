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
    public interface IContractVerifierPatternInstanceState
    {
        /// <summary>
        /// Gets othe test fixture type or null if none.
        /// </summary>
        Type FixtureType
        {
            get;
        }

        /// <summary>
        /// Gets the test fixture instance or null if none.
        /// </summary>
        object FixtureInstance
        {
            get;
        }
    }
}
