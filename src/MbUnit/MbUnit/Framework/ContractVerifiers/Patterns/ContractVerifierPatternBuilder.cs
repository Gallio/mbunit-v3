using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Framework.Data;

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Abstract builder of test pattern for contract verifiers.
    /// The builder collects data to construct a test pattern.
    /// </summary>
    public abstract class ContractVerifierPatternBuilder
    {
        /// <summary>
        /// Constructs the test pattern.
        /// </summary>
        /// <returns>A new test pattern for a contract verifier.</returns>
        public abstract ContractVerifierPattern ToPattern();
    }
}
