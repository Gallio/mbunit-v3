using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Icarus.Utilities
{
    internal static class NativeMethods
    {
        [DllImport("shlwapi.dll")]
        public static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);
    }
}
