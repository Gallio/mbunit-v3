using System;

namespace Gallio.AutoCAD.Native
{
    /// <summary>
    /// Contains constants for windows messages.
    /// </summary>
    internal static class MessageTypes
    {
        /// <summary>
        /// An application sends the <c>WM_COPYDATA</c> message
        /// to pass data to another application. 
        /// </summary>
        public const uint WM_COPYDATA = 0x4A;
    }
}
