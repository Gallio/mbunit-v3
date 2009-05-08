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

namespace Gallio.Runtime.FileTypes
{
    /// <summary>
    /// Describes a file type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// File type objects are unique and can be compared for equality by reference.
    /// </para>
    /// </remarks>
    public sealed class FileType
    {
        private readonly string id;
        private readonly string description;
        private readonly FileType superType;

        /// <summary>
        /// Creates a file type object.
        /// </summary>
        /// <param name="id">The file type id</param>
        /// <param name="description">The file type description</param>
        /// <param name="superType">The super type of the file type, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> 
        /// or <paramref name="description"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is empty</exception>
        public FileType(string id, string description, FileType superType)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (id.Length == 0)
                throw new ArgumentException("The file type id must not be empty.", "id");
            if (description == null)
                throw new ArgumentNullException("description");

            this.id = id;
            this.description = description;
            this.superType = superType;
        }
        
        /// <summary>
        /// Gets the id of the file type.
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
        /// Gets the super type of this file type, or null if none.
        /// </summary>
        public FileType SuperType
        {
            get { return superType; }
        }

        /// <summary>
        /// Returns true if this file type is equal to or is a subtype of some other type.
        /// </summary>
        /// <param name="otherType">The other type</param>
        /// <returns>True if the condition is satisfied</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="otherType"/> is null</exception>
        public bool IsSameOrSubtypeOf(FileType otherType)
        {
            if (otherType == null)
                throw new ArgumentNullException("otherType");

            FileType currentType = this;
            do
            {
                if (currentType == otherType)
                    return true;

                currentType = currentType.SuperType;
            }
            while (currentType != null);

            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0}: {1}", id, description);
        }
    }
}
