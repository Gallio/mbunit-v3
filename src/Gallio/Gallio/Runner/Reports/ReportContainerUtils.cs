// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Utilities;
using System.IO;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Utilities for working with <see cref="IReportContainer" />.
    /// </summary>
    public static class ReportContainerUtils
    {
        /// <summary>
        /// Recursively copies files and folders from the source path in the native
        /// file system to the destination path within the report container.
        /// </summary>
        /// <param name="container">The container</param>
        /// <param name="sourcePathInFileSystem">The source file or directory path in the native file system</param>
        /// <param name="destPathInContainer">The destination file or directory path in the report container</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="container" />,
        /// <paramref name="sourcePathInFileSystem"/> or <paramref name="destPathInContainer"/> is null</exception>
        public static void CopyToReport(IReportContainer container, string sourcePathInFileSystem, string destPathInContainer)
        {
            FileUtils.CopyAllIndirect(sourcePathInFileSystem, destPathInContainer, null,
                delegate(string sourceFilePath, string destFilePath)
                {
                    using (Stream sourceStream = File.OpenRead(sourceFilePath))
                    {
                        using (Stream destStream = container.OpenWrite(destFilePath, MimeTypes.GetMimeTypeByExtension(Path.GetExtension(sourceFilePath)), null))
                        {
                            FileUtils.CopyStreamContents(sourceStream, destStream);
                        }
                    }
                });
        }
    }
}
