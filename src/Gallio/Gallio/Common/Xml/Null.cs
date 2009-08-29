using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// The singleton instance representing a null XML node.
    /// </summary>
    public class Null : INode
    {
        /// <summary>
        /// The singleton instance representing a null node.
        /// </summary>
        public static readonly Null Instance = new Null();

        private Null()
        {
        }

        /// <inheritdoc />
        public INode Child
        {
            get
            {
                return Instance;
            }
        }

        /// <inheritdoc />
        public bool IsNull
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public string ToXml()
        {
            return String.Empty;
        }

        /// <inheritdoc />
        public DiffSet Diff(INode expected, Path path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");

            return DiffSet.Empty;
        }
    }
}
