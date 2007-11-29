using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// <para>
    /// A namespace reflection wrapper identified by its name.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface INamespaceInfo : ICodeElementInfo
    {
    }
}
