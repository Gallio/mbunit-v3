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
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    internal static class ResourceSearchRules
    {
        public static IEnumerable<string> GetSearchPaths(DirectoryInfo baseDirectory, IEnumerable<string> probingPaths, string resourcePath)
        {
            if (resourcePath != null && Path.IsPathRooted(resourcePath))
            {
                yield return resourcePath;
                yield break;
            }

            string baseDirectoryPath = baseDirectory.FullName;
            yield return CombineWithResourcePathIfNotNull(baseDirectoryPath, resourcePath);

            string baseDirectoryBinPath = Path.Combine(baseDirectoryPath, "bin");
            yield return CombineWithResourcePathIfNotNull(baseDirectoryBinPath, resourcePath);

            foreach (string probingPath in probingPaths)
            {
                yield return CombineWithResourcePathIfNotNull(Path.Combine(baseDirectoryPath, probingPath), resourcePath);
                yield return CombineWithResourcePathIfNotNull(Path.Combine(baseDirectoryBinPath, probingPath), resourcePath);
            }
        }

        private static string CombineWithResourcePathIfNotNull(string basePath, string resourcePath)
        {
            return resourcePath != null ? Path.Combine(basePath, resourcePath) : basePath;
        }
    }
}
