using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// Provides a mechanism for inspecting the metadata and contents of a particular file.
    /// </summary>
    public interface IFileInspector
    {
        /// <summary>
        /// Tries to get the file info associated with the original path of the file.
        /// </summary>
        /// <param name="fileInfo">Returns the file info when available, or null otherwise</param>
        /// <returns>True if the file info was available</returns>
        bool TryGetFileInfo(out FileInfo fileInfo);

        /// <summary>
        /// Tries to get the contents of the file as a string. 
        /// </summary>
        /// <param name="contents">Returns the file contents when available, or null otherwise</param>
        /// <returns>True if the file contents were available</returns>
        bool TryGetContents(out string contents);

        /// <summary>
        /// Tries to get the file stream opened for reading.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The caller does not need to close the stream; it will be automatically
        /// released by the file inspector when finished.
        /// </para>
        /// </remarks>
        /// <param name="stream">Returns the file stream when available, or null otherwise</param>
        /// <returns>True if the file stream was available</returns>
        bool TryGetStream(out Stream stream);
    }
}
