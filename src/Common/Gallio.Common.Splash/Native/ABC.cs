using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ABC
    {
        /// <summary>
        /// The A spacing of the character. The A spacing is the distance to add to the current position before drawing the character glyph.
        /// </summary>
        public int abcA;

        /// <summary>
        /// The B spacing of the character. The B spacing is the width of the drawn portion of the character glyph.
        /// </summary>
        public int abcB;

        /// <summary>
        /// The C spacing of the character. The C spacing is the distance to add to the current position to provide white space to the right of the character glyph.
        /// </summary>
        public int abcC;

        public int TotalWidth
        {
            get { return abcA + abcB + abcC; }
        }
    }
}
