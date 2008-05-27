// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Runner.Caching
{
    /// <summary>
    /// A disk cache that stores its contents in the user's temporary directory.
    /// </summary>
    /// <seealso cref="Path.GetTempPath"/>
    public class TemporaryDiskCache : SimpleDiskCache
    {
        /// <summary>
        /// The default cache directory name.
        /// </summary>
        public static readonly string DefaultCacheDirectoryName = "Gallio.Cache";

        /// <summary>
        /// Creates a temporary disk cache within the specified subdirectory of the user's temporary directory.
        /// </summary>
        /// <param name="cacheDirectoryName">The name of the cache subdirectory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="cacheDirectoryName"/> is null</exception>
        public TemporaryDiskCache(string cacheDirectoryName)
            : base(Path.Combine(Path.GetTempPath(), cacheDirectoryName))
        {
        }

        /// <summary>
        /// Creates a temporary disk cache within the <see cref="DefaultCacheDirectoryName" /> subdirectory of the user's temporary directory.
        /// </summary>
        public TemporaryDiskCache()
            : this(DefaultCacheDirectoryName)
        {
        }
    }
}
