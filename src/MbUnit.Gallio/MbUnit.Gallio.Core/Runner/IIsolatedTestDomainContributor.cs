using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// <para>
    /// An isolated test domain contribution applies contributions to
    /// a <see cref="IsolatedTestDomain" /> prior to loading test
    /// projects.
    /// </para>
    /// <para>
    /// An example of a contribution is one that registers additional
    /// bootstrap assemblies with binding redirects for use inside the
    /// isolated test domain.
    /// </para>
    /// </summary>
    public interface IIsolatedTestDomainContributor
    {
        /// <summary>
        /// Applies contributions to the specified domain.
        /// </summary>
        /// <param name="domain">The domain to which contributions are applied</param>
        void Apply(IsolatedTestDomain domain);
    }
}
