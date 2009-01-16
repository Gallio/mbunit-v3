using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Populates components lazily.
    /// </summary>
    /// <remarks>
    /// A populator function takes one parameter to specify a hint for the particular
    /// code element whose patterns should be processed to generate components.  If the hint
    /// is null or unrecognized then the populator should proceed to generate all remaining
    /// components.
    /// </remarks>
    /// <param name="codeElementHint">The code element hint to identify the location of the
    /// particular components to populate, or null to populate them all</param>
    public delegate void DeferredComponentPopulator(ICodeElementInfo codeElementHint);
}
