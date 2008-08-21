namespace Gallio.Model.Logging.Tags
{
    /// <summary>
    /// Visits a <see cref="Tag" />.
    /// </summary>
    public interface ITagVisitor
    {
        /// <summary>
        /// Visits a body tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitBodyTag(BodyTag tag);

        /// <summary>
        /// Visits a section tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitSectionTag(SectionTag tag);

        /// <summary>
        /// Visits a marker tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitMarkerTag(MarkerTag tag);

        /// <summary>
        /// Visits an embedded attachment tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitEmbedTag(EmbedTag tag);

        /// <summary>
        /// Visits a text tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitTextTag(TextTag tag);
    }
}