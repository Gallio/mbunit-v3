using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Logging.Tags
{
    /// <summary>
    /// An abstract base class for tag visitors that recursively traverses all tags
    /// and does nothing else by default.
    /// </summary>
    public abstract class BaseTagVisitor : ITagVisitor
    {
        /// <inheritdoc />
        public virtual void VisitBodyTag(BodyTag tag)
        {
            tag.AcceptContents(this);
        }

        /// <inheritdoc />
        public virtual void VisitSectionTag(SectionTag tag)
        {
            tag.AcceptContents(this);
        }

        /// <inheritdoc />
        public virtual void VisitMarkerTag(MarkerTag tag)
        {
            tag.AcceptContents(this);
        }

        /// <inheritdoc />
        public virtual void VisitEmbedTag(EmbedTag tag)
        {
        }

        /// <inheritdoc />
        public virtual void VisitTextTag(TextTag tag)
        {
        }
    }
}
