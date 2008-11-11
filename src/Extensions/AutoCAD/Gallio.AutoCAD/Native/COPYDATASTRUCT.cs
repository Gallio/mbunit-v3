using System;
using System.Runtime.InteropServices;

namespace Gallio.AutoCAD.Native
{
    /// <summary>
    /// The <c>COPYDATASTRUCT</c> structure contains data to be passed
    /// to another application by the <see cref="MessageTypes.WM_COPYDATA"/> message. 
    /// </summary>
    internal struct COPYDATASTRUCT
    {
        public COPYDATASTRUCT(string message)
        {
            dwData = new IntPtr(1);
            cbData = (message.Length + 1) * Marshal.SystemDefaultCharSize;
            lpData = message;
        }

        /// <summary>
        /// Specifies data to be passed to the receiving application. 
        /// </summary>
        public IntPtr dwData;

        /// <summary>
        /// Specifies the size, in bytes, of the data pointed to by the
        /// <c>lpData</c> member.
        /// </summary>
        public int cbData;

        /// <summary>
        /// Pointer to data to be passed to the receiving application.
        /// </summary>
        /// <remarks>
        /// This has been specialized for string data to avoid having
        /// to marshal the string to an IntPtr manually.
        /// </remarks>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpData; // PVOID: Pointer to command string.
    }
}
