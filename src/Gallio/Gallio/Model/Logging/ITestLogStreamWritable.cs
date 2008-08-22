using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// Interface implemented by objects that can write themselves to
    /// a <see cref="TestLogStreamWriter" />
    /// </summary>
    public interface ITestLogStreamWritable
    {
        /// <summary>
        /// Writes the object to a test log stream.
        /// </summary>
        /// <param name="writer">The test log stream</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        void WriteTo(TestLogStreamWriter writer);
    }
}
