using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Normalization
{
    /// <summary>
    /// Provides a method that may be called by client code to obtain a normalized
    /// copy of an object.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    public interface INormalizable<T>
    {
        /// <summary>
        /// Obtains a normalized copy of the object or the original instance if it is already normalized.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Some object may be considered valid but have content which is not in a suitable
        /// form for further processing.  For example, a message object may have a field that contains
        /// excessive whitespace or non-printable characters that can be trimmed.  To handle
        /// these cases, the message consumer may call the <see cref="Normalize" /> method to
        /// obtain a normalized copy of the message.  However, if the message was already normalized
        /// the <see cref="Normalize" /> method could just return the same instance.
        /// </para>
        /// <para>
        /// This method may assume that the object has already been validated.
        /// </para>
        /// <para>
        /// This method is not permitted to modify the original object or its contents.  If the
        /// contents must be changed during normalization then a copy should be created
        /// and returned.
        /// </para>
        /// </remarks>
        /// <returns>The original instance or a normalized copy.</returns>
        T Normalize();
    }
}