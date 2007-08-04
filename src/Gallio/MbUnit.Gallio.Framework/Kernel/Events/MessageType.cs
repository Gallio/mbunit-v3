using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// Describes the type and purpose of a message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// The message contains debugging output.
        /// </summary>
        Debug,

        /// <summary>
        /// The message describes a change in the status of the test harness.
        /// </summary>
        Status
    }
}
