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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common.IO;
using Gallio.Framework;
using Microsoft.Cci;
using Microsoft.Cci.MetadataReader.ObjectModelImplementation;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Implementation of a debug symbol resolver based on the Microsoft Common Compiler Infrastructure Metadata Components (CCI Metadata Components)
    /// </summary>
    /// <remarks>
    /// <para>
    /// see http://ccimetadata.codeplex.com/ for more information about the CCI Metadata Components.
    /// </para>
    /// </remarks>
    public class CciDebugSymbolResolver : IDebugSymbolResolver
    {
        private readonly IDictionary<string, CciModuleCache> cache = new Dictionary<string, CciModuleCache>();
        private readonly IList<string> unavailable = new List<string>();

        /// <summary>
        /// Creates a Cci debug symbol resolver.
        /// </summary>
        public CciDebugSymbolResolver()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public CodeLocation GetSourceLocationForMethod(string assemblyPath, int methodToken)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");

            CciModuleCache data;

            if (!cache.TryGetValue(assemblyPath, out data))
            {
                if (unavailable.Contains(assemblyPath))
                    return CodeLocation.Unknown;

                try
                {
                    data = new CciModuleCache(new FileSystem(), assemblyPath);
                }
                catch (InvalidOperationException)
                {
                    unavailable.Add(assemblyPath);
                    return CodeLocation.Unknown;
                }

                cache.Add(assemblyPath, data);
            }

            return data.GetMethodLocation(Convert.ToUInt32(methodToken));
        }
    }
}
