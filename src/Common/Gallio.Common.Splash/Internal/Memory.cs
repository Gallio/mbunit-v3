using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    internal unsafe class Memory
    {
        public static void* Alloc(int bytes)
        {
            return Marshal.AllocCoTaskMem(bytes).ToPointer();
        }

        public static void* ReAlloc(void* memory, int bytes)
        {
            return Marshal.ReAllocCoTaskMem(new IntPtr(memory), bytes).ToPointer();
        }

        public static void Free(void* memory)
        {
            Marshal.FreeCoTaskMem(new IntPtr(memory));
        }
    }
}
