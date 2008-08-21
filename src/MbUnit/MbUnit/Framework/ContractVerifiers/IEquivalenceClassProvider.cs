using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Provides equivalence classes of object instances.
    /// </para>
    /// <para>
    /// That interface is one of the two side parts contract for the equality 
    /// contract verifier <see cref="VerifyEqualityContractAttribute"/>.
    /// </para>
    /// </summary>
    public interface IEquivalenceClassProvider<T>
    {
        /// <summary>
        /// Provides equivalence classes of object instances.
        /// </summary>
        /// <returns>A collection of equivalence classes.</returns>
        EquivalenceClassCollection<T> GetEquivalenceClasses();
    }
}
