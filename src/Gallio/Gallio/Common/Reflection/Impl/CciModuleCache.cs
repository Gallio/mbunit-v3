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
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common.IO;
using Microsoft.Cci;
using Microsoft.Cci.MetadataReader.ObjectModelImplementation;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Data structure that maps every method in a CCI <see cref="IModule"/> with its <see cref="CodeLocation"/>
    /// </summary>
    public class CciModuleCache
    {
        private readonly IFileSystem fileSystem;
        private readonly string assemblyPath;
        private readonly IDictionary<uint, CodeLocation> map = new Dictionary<uint, CodeLocation>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileSystem">File system wrapper.</param>
        /// <param name="assemblyPath">The path of the assembly to scan.</param>
        public CciModuleCache(IFileSystem fileSystem, string assemblyPath)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");

            this.fileSystem = fileSystem;
            this.assemblyPath = assemblyPath;
            FeedMap();
        }

        /// <summary>
        /// Returns the code location of the method with the specified metadata token.
        /// </summary>
        /// <param name="methodToken">The searched metadata token.</param>
        /// <returns>The resulting code location, or an unknown location if the method was not found.</returns>
        public CodeLocation GetMethodLocation(uint methodToken)
        {
            CodeLocation result;

            if (!map.TryGetValue(methodToken, out result))
            {
                result = CodeLocation.Unknown;
            }

            return result;
        }

        private void FeedMap()
        {
            var host = new PeReader.DefaultHost();
            var module = host.LoadUnitFrom(assemblyPath) as IModule;

            if (module == null)
                throw new InvalidOperationException(String.Format("Cannot open or read the module '{0}'.", assemblyPath));

            using (var pdbReader = new PdbReader(OpenPdbStream(), host))
            {
                foreach (INamedTypeDefinition typeDefinition in module.GetAllTypes())
                {
                    foreach (IMethodDefinition methodDefinition in typeDefinition.Methods)
                    {
                        ProcessMethodDefinition(methodDefinition, pdbReader);
                    }
                }
            }
        }

        private Stream OpenPdbStream()
        {
            string pdbFileName = Path.ChangeExtension(assemblyPath, ".pdb");

            try
            {
                return fileSystem.OpenRead(pdbFileName);
            }
            catch (FileNotFoundException exception)
            {
                throw new InvalidOperationException(String.Format("Cannot open or read the program database '{0}'.", pdbFileName), exception);
            }
        }

        private void ProcessMethodDefinition(IMethodDefinition methodDefinition, PdbReader pdbReader)
        {
            uint token = GetMethodToken_ByReadingInternals(methodDefinition);

            if (token != 0)
            {
                var locations = pdbReader.GetPrimarySourceLocationsFor(methodDefinition.Locations);
                var enumerator = locations.GetEnumerator();

                if (enumerator.MoveNext())
                {
                    IPrimarySourceLocation location = enumerator.Current;
                    map.Add(token, new CodeLocation(location.PrimarySourceDocument.Location, location.StartLine, 0));
                }
            }
        }

        // Recipe 1 (slow)
        // Using reflection to access to the "TokenValue" property (non public)
        private static uint GetMethodToken_ByReflection(IMethodDefinition methodDefinition)
        {
            Type type = methodDefinition.GetType();
            PropertyInfo property = type.GetProperty("TokenValue", BindingFlags.Instance | BindingFlags.NonPublic);
            return (uint)property.GetValue(methodDefinition, null);
        }

        // Recipe 2 (faster)
        // Use the slightly modified CCI binaries with the [InternalsVisibleTo] attribute.
        private static uint GetMethodToken_ByReadingInternals(IMethodDefinition methodDefinition)
        {
            var concrete = methodDefinition as MethodDefinition;
            return concrete != null ? concrete.TokenValue : 0;
        }
    }
}
