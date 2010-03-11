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
using Gallio.Common.IO;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy.Commands
{
    public class CopyCommand : ICommand
    {
        private readonly string destinationFolder;
        private readonly string sourceFolder;
        private readonly IList<string> files;
        private readonly IFileSystem fileSystem;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        public CopyCommand(string destinationFolder, string sourceFolder, IList<string> files, 
            IFileSystem fileSystem, IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.destinationFolder = destinationFolder;
            this.sourceFolder = sourceFolder;
            this.files = files;
            this.fileSystem = fileSystem;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (files.Count == 0)
                return;

            using (progressMonitor.BeginTask("Copying files", files.Count))
            {
                foreach (var file in files)
                {
                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                        CopyFile(subProgressMonitor, file);
                }
            }
        }

        private void CopyFile(IProgressMonitor progressMonitor, string filePath)
        {
            try
            {
                var destinationFilePath = Path.Combine(destinationFolder, filePath);
                
                progressMonitor.SetStatus(destinationFilePath);

                var sourceFilePath = Path.Combine(sourceFolder, filePath);

                var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
                if (!fileSystem.DirectoryExists(destinationDirectory))
                    fileSystem.CreateDirectory(destinationDirectory);

                fileSystem.CopyFile(sourceFilePath, destinationFilePath, true);
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report(string.Format("Error copying file '{0}'.", filePath), ex);
            }
        }
    }
}
