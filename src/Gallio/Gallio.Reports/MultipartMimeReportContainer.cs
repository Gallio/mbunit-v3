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
using System.IO;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Runner.Reports;

namespace Gallio.Reports
{
    /// <summary>
    /// A report container that saves a report as a multipart mime archive in a single file
    /// within another container.
    /// </summary>
    /// <remarks>
    /// This is currently specialized for saving HTML reports.
    /// It does not support loading reports.
    /// </remarks>
    internal class MultipartMimeReportContainer : IReportContainer, IDisposable
    {
        private const string MessagePartBoundary = "----=_Boundary";
        private const string EscapedMessagePartBoundary = "--" + MessagePartBoundary;

        private readonly IReportContainer inner;
        private StreamWriter archiveWriter;

        /// <summary>
        /// Creates the multipart mime report container.
        /// </summary>
        /// <param name="inner">The container to which the archived report should be saved</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null</exception>
        public MultipartMimeReportContainer(IReportContainer inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        /// <inheritdoc />
        public string ReportName
        {
            get { return inner.ReportName; }
        }

        /// <summary>
        /// Opens the archive within the inner container.
        /// </summary>
        /// <param name="archivePath">The path of the archive to create</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="archivePath"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the archive has already been opened</exception>
        public void OpenArchive(string archivePath)
        {
            if (archivePath == null)
                throw new ArgumentNullException("archivePath");
            if (archiveWriter != null)
                throw new InvalidOperationException("The archive is already open.");

            Encoding encoding = new UTF8Encoding(false);
            archiveWriter = new StreamWriter(inner.OpenWrite(archivePath, MimeTypes.MHtml, encoding), encoding);
            archiveWriter.AutoFlush = false;
            archiveWriter.NewLine = "\r\n";
            archiveWriter.WriteLine("MIME-Version: 1.0");
            archiveWriter.WriteLine("Content-Type: " + MimeTypes.MHtml + "; type=\"text/html\"; boundary=\"" + MessagePartBoundary + "\"");
            archiveWriter.WriteLine();
            archiveWriter.WriteLine("This is a multi-part message in MIME format.");
            archiveWriter.WriteLine();
            archiveWriter.WriteLine(EscapedMessagePartBoundary);
        }

        /// <summary>
        /// Finishes writing out the MIME archive and closes it.
        /// Does nothing if the archive is not open.
        /// </summary>
        public void CloseArchive()
        {
            if (archiveWriter == null)
                return;

            archiveWriter.Close();
            archiveWriter = null;
        }

        /// <inheritdoc />
        public void DeleteReport()
        {
            inner.DeleteReport();
        }

        /// <inheritdoc />
        public Stream OpenRead(string path)
        {
            throw new NotSupportedException("Cannot read reports from multipart mime archives.");
        }

        /// <inheritdoc />
        public Stream OpenWrite(string path, string contentType, Encoding encoding)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (archiveWriter == null)
                throw new InvalidOperationException("The archive is not open.");

            archiveWriter.WriteLine("Content-Type: {0}", contentType ?? MimeTypes.GetMimeTypeByExtension(Path.GetExtension(path)) ?? MimeTypes.Binary);
            archiveWriter.WriteLine("Content-Transfer-Encoding: base64");
            archiveWriter.WriteLine("Content-Location: file:///{0}", Uri.EscapeUriString(path.Replace('\\', '/')));
            archiveWriter.WriteLine();

            // FIXME: Buffering the whole stream in memory is not very efficient for large files.
            TemporaryMemoryStream stream = new TemporaryMemoryStream();
            stream.Closing += delegate
            {
                stream.Position = 0;
                archiveWriter.Write(Convert.ToBase64String(stream.GetBuffer(), 0, (int) stream.Length, Base64FormattingOptions.InsertLineBreaks));
                archiveWriter.WriteLine();
                archiveWriter.WriteLine(EscapedMessagePartBoundary);
            };

            return stream;
        }

        /// <inheritdoc />
        public string EncodeFileName(string fileName)
        {
            // Note: IE 7 does not seem to correctly resolve references to files with spaces
            //       even when the Urls are properly (or improperly) escaped.  So we convert
            //       spaces to underscores here to ensure everything works out.
            return inner.EncodeFileName(fileName).Replace(' ', '_');
        }

        void IDisposable.Dispose()
        {
            CloseArchive();
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
