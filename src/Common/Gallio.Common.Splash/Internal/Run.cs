using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// Internal structure describing a run of text or an embedded object in the document.
    /// </summary>
    /// <remarks>
    /// Packed into 4 bytes per Run.  Keep it that way.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Run
    {
        private const int RunKindMask = 0x03;

        private byte styleIndex;
        private byte bitFields;
        private ushort miscShort;

        public RunKind RunKind
        {
            get { return (RunKind)(bitFields & RunKindMask); }
        }

        public int StyleIndex
        {
            get { return styleIndex; }
        }

        public int CharCount
        {
            get
            {
                switch (RunKind)
                {
                    case RunKind.Text:
                        return miscShort;
                    case RunKind.Object:
                    case RunKind.Tab:
                        return 1;
                    default:
                        throw new NotSupportedException();
                }
            }
            set
            {
                switch (RunKind)
                {
                    case RunKind.Text:
                        miscShort = (ushort)value;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public int ObjectIndex
        {
            get
            {
                switch (RunKind)
                {
                    case RunKind.Object:
                        return miscShort;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void InitializeTextRun(int styleIndex)
        {
            this.styleIndex = (byte)styleIndex;
            bitFields = (byte)RunKind.Text;
            miscShort = 0;
        }

        public void InitializeObjectRun(int styleIndex, int objectIndex)
        {
            this.styleIndex = (byte)styleIndex;
            bitFields = (byte)RunKind.Object;
            miscShort = (ushort)objectIndex;
        }

        public void InitializeTabRun(int styleIndex)
        {
            this.styleIndex = (byte)styleIndex;
            bitFields = (byte)RunKind.Tab;
            miscShort = 0;
        }
    }
}
