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
using ICSharpCode.SharpZipLib.Zip;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// An archive based representation of a report container that uses
    /// a compressed file to store report contents.
    /// </summary>
    public class ArchiveReportContainer : AbstractReportContainer
    {
        private ZipOutputStream archiveStream;

         /// <summary>
        /// Creates a filed archive based representation of a report container.
        /// </summary>
        /// <param name="reportDirectory">The report directory path.</param>
        /// <param name="reportName">The report name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportDirectory"/> or
        /// <paramref name="reportName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="reportName"/> is an empty string.</exception>
        public ArchiveReportContainer(string reportDirectory, string reportName)
            : base(reportDirectory, reportName)
        {
        }

        /// <inheritdoc />
        public override void DeleteReport()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Stream OpenRead(string path)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Stream OpenWrite(string path, string contentType, Encoding encoding)
        {
            ValidateFilePath(path);
            OpenArchiveStream();
            var zipEntry = new ZipEntry(path);
            zipEntry.DateTime = DateTime.Now;
            archiveStream.PutNextEntry(zipEntry);
            var stream = new TemporaryMemoryStream();
            
            stream.Closing += delegate
            {
                stream.Position = 0;
                archiveStream.Write(stream.GetBuffer(), 0, (int)stream.Length);
            };

            return stream;
        }

        private void OpenArchiveStream()
        {
            if (archiveStream == null)
            {
                if (!Directory.Exists(ReportDirectory))
                {
                    Directory.CreateDirectory(ReportDirectory);
                }

                var fileStream = File.Create(Path.Combine(ReportDirectory, ReportName + ".zip"));
                archiveStream = new ZipOutputStream(fileStream);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            CloseArchiveStream();
            base.Dispose();
        }

        private void CloseArchiveStream()
        {
            if (archiveStream != null)
            {          
                archiveStream.Finish();
                archiveStream.Dispose();
                archiveStream = null;
            }
        }

        private sealed class TemporaryMemoryStream : MemoryStream
        {
            public event EventHandler Closing;

            public override void Close()
            {
                if (Closing != null)
                {
                    Closing(this, EventArgs.Empty);
                    Closing = null;
                }

                base.Close();
            }
        }
    }
}
