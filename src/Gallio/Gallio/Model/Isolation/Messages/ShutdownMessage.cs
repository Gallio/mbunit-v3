using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Messaging;

namespace Gallio.Model.Isolation.Messages
{
    /// <summary>
    /// Tells the client to shut down.
    /// </summary>
    [Serializable]
    public class ShutdownMessage : Message
    {
    }
}