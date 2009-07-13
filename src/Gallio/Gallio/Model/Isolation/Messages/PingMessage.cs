using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Messaging;

namespace Gallio.Model.Isolation.Messages
{
    /// <summary>
    /// Tells the server that the client is active.
    /// </summary>
    [Serializable]
    public class PingMessage : Message
    {
    }
}