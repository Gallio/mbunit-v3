// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.IO;
using Gallio.Common;

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Provides support for working with different implementations of the .Net runtime.
    /// </summary>
    public static class DotNetRuntimeSupport
    {
        private static Memoizer<DotNetRuntimeType> runtimeTypeMemoizer = new Memoizer<DotNetRuntimeType>();

        /// <summary>
        /// Returns true if the application is running within the Mono runtime.
        /// </summary>
        /// <remarks>
        /// It is occasionally necessary to tailor the execution of the test runner
        /// depending on whether Mono is running.  However, the number of such
        /// customizations should be very limited.
        /// </remarks>
        public static bool IsUsingMono
        {
            get { return RuntimeType == DotNetRuntimeType.Mono; }
        }

        /// <summary>
        /// Get the type of the .Net runtime currently executing this process.
        /// </summary>
        public static DotNetRuntimeType RuntimeType
        {
            get
            {
                return runtimeTypeMemoizer.Memoize(() =>
                {
                    if (Type.GetType(@"Mono.Runtime") != null)
                        return DotNetRuntimeType.Mono;

                    return DotNetRuntimeType.CLR;
                });
            }
        }

        /// <summary>
        /// When using Mono, creates <see cref="ProcessStartInfo" /> that re-enters the Mono runtime
        /// if the executable is .Net otherwise creates a standard process start info.
        /// </summary>
        /// <param name="executablePath">The executable path</param>
        /// <param name="arguments">The arguments</param>
        /// <returns>The process start info</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null</exception>
        public static ProcessStartInfo CreateReentrantProcessStartInfo(string executablePath, string arguments)
        {
            if (executablePath == null)
                throw new ArgumentNullException("executablePath");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            if (IsUsingMono && IsAssembly(executablePath))
            {
                string monoExe = GetCurrentMonoRuntimePath();
                if (monoExe != null)
                    return new ProcessStartInfo(monoExe, String.Concat("\"", executablePath, "\" ", arguments));
            }

            return new ProcessStartInfo(executablePath, arguments);
        }

        private static string GetCurrentMonoRuntimePath()
        {
            Process process = Process.GetCurrentProcess();
            if (process.MainModule != null)
                return process.MainModule.FileName;

            return null;
        }

        /// <summary>
        /// Returns true if the stream represents a CLI Assembly in Microsoft PE format.
        /// </summary>
        /// <remarks>
        /// This function does not close the stream.
        /// </remarks>
        /// <param name="stream">The stream</param>
        /// <returns>True if the stream represents an assembly</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null</exception>
        public static bool IsAssembly(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            long length = stream.Length;
            if (length < 0x40)
                return false;

            BinaryReader reader = new BinaryReader(stream);

            // Read the pointer to the PE header.
            stream.Position = 0x3c;
            uint peHeaderRva = reader.ReadUInt32();
            if (peHeaderRva == 0)
                peHeaderRva = 0x80;

            // Ensure there is at least enough room for the following structures:
            //     4 byte PE Signature
            //    20 byte PE Header
            //    28 byte Standard Fields
            //    68 byte NT Fields
            //   128 byte Data Dictionary Table
            if (peHeaderRva > length - 248)
                return false;

            // Check the PE signature.  Should equal 'PE\0\0'.
            stream.Position = peHeaderRva;
            uint peSignature = reader.ReadUInt32();
            if (peSignature != 0x00004550)
                return false;

            // Read the 15th Data Dictionary RVA field which contains the CLI header RVA.
            // When this is non-zero then the file contains CLI data otherwise not.
            stream.Position = peHeaderRva + 232;
            uint cliHeaderRva = reader.ReadUInt32();
            if (cliHeaderRva == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if the file represents a CLI Assembly in Microsoft PE format.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>True if the file represents an assembly</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePath"/> is null</exception>
        public static bool IsAssembly(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                return false;

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return IsAssembly(stream);
            }
        }
    }
}