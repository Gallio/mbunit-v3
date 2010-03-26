using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml.Diffing
{
    /// <summary>
    /// Represents a type of XML diff.
    /// </summary>
    public sealed class DiffType : IEquatable<DiffType>
    {
        private readonly string description;

        /// <summary>
        /// Gets the description of the diff.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        private DiffType(string description)
        {
            this.description = description;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as DiffType);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return description.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(DiffType other)
        {
            return other != null
                && description != other.Description;
        }

        #region Catalog

        /// <summary>
        /// Unexpected content.
        /// </summary>
        public readonly static DiffType UnexpectedContent = new DiffType("Unexpected content");

        /// <summary>
        /// Unexpected comment.
        /// </summary>
        public readonly static DiffType UnexpectedComment = new DiffType("Unexpected comment");

        /// <summary>
        /// Unexpected element
        /// </summary>
        public readonly static DiffType UnexpectedElement = new DiffType("Unexpected element");

        /// <summary>
        /// Missing element
        /// </summary>
        public readonly static DiffType MissingElement = new DiffType("Missing element");

        /// <summary>
        /// Mismatched content.
        /// </summary>
        public readonly static DiffType MismatchedContent = new DiffType("Mismatched content");

        /// <summary>
        /// Mismatched comment.
        /// </summary>
        public readonly static DiffType MismatchedComment = new DiffType("Mismatched comment");

        /// <summary>
        /// Mismatched element.
        /// </summary>
        public readonly static DiffType MismatchedElement = new DiffType("Mismatched element");

        /// <summary>
        /// Unexpected attribute.
        /// </summary>
        public readonly static DiffType UnexpectedAttribute = new DiffType("Unexpected attribute");

        /// <summary>
        /// Missing attribute.
        /// </summary>
        public readonly static DiffType MissingAttribute = new DiffType("Missing attribute");

        /// <summary>
        /// Mismatched attribute.
        /// </summary>
        public readonly static DiffType MismatchedAttribute = new DiffType("Mismatched attribute");

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly static DiffType xxx = new DiffType("");

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly static DiffType xxx = new DiffType("");

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly static DiffType xxx = new DiffType("");

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly static DiffType xxx = new DiffType("");

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly static DiffType xxx = new DiffType("");

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly static DiffType xxx = new DiffType("");

        ///// <summary>
        ///// 
        ///// </summary>
        //public readonly static DiffType xxx = new DiffType("");


        

        #endregion
    }
}
