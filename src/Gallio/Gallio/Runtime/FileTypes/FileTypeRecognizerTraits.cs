using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// Provides metadata about a <see cref="IFileTypeRecognizer" />.
    /// </summary>
    public class FileTypeRecognizerTraits : Traits
    {
        private readonly string id;
        private readonly string description;

        /// <summary>
        /// Creates traits for a file type recognizer.
        /// </summary>
        /// <param name="id">The file type id</param>
        /// <param name="description">The file type description</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or
        /// <paramref name="description"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is empty</exception>
        public FileTypeRecognizerTraits(string id, string description)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (id.Length == 0)
                throw new ArgumentException("The file type name must not be empty.", "id");
            if (description == null)
                throw new ArgumentNullException("description");

            this.id = id;
            this.description = description;
        }

        /// <summary>
        /// Gets the file type id.
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the file type description.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets or sets the id of the file type, or null if none.
        /// </summary>
        public string SuperTypeId { get; set; }

        /// <summary>
        /// Specifies a regular expression used to identify the type of a file by its name,
        /// or null if none.
        /// </summary>
        public string FileNameRegex { get; set; }

        /// <summary>
        /// Specifies a regular expression used to identify the type of a file by its contents,
        /// or null if none.
        /// </summary>
        public string ContentsRegex { get; set; }
    }
}
