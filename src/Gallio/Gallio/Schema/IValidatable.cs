using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Schema
{
    /// <summary>
    /// Provides a method that may be called by client code to validate that schema
    /// data is well-formed.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <exception cref="ValidationException">Thrown if the data is not well-formed</exception>
        void Validate();
    }
}
