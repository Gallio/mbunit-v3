// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// Native Win32 methods used to load the executable unmanaged test repository.
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// Loads the specified module into the address space of the calling process.
        /// </summary>
        /// <param name="lpLibFileName">The name of the module.</param>
        /// <returns>If the function succeeds, an handle to the module; otherwise, IntPtr.Zero.</returns>
        /// <remarks>
        /// <para>
        /// To get extended error information, call <see cref="Marhsal.GetLastWin32Error"/>.
        /// </para>
        /// </remarks>
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        /// <summary>
        /// rees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="hModule">A handle to the loaded library module.</param>
        /// <returns>True if the function succees; otherwise, false.</returns>
        /// <remarks>
        /// <para>
        /// To get extended error information, call <see cref="Marhsal.GetLastWin32Error"/>.
        /// </para>
        /// </remarks>
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">A handle to the DLL module that contains the function or variable.</param>
        /// <param name="procName">The function or variable name, or the function's ordinal value.</param>
        /// <returns>If the function succeeds, the address of the exported function or variable; otherwise, IntPtr.Zero.</returns>
        /// <remarks>
        /// <para>
        /// To get extended error information, call <see cref="Marhsal.GetLastWin32Error"/>.
        /// </para>
        /// </remarks>
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }
}