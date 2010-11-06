using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.MbUnitCppAdapter.Tests
{
    internal static class Helper
    {
        public static string CurrentArchitecture
        {
            get { return (8 == IntPtr.Size) ? "x64" : "x86"; }
        }

        public static string GetTestResources()
        {
            return GetTestResources(CurrentArchitecture);
        }

        public static string GetTestResources(string architecture)
        {
            return "Gallio.MbUnitCppAdapter.TestResources." + architecture + ".dll";
        }
    }
}
