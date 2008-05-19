using System;
using Gallio.Model;
using JetBrains.ReSharper.Daemon;

namespace Gallio.ReSharperRunner.Daemons
{
#if ! RESHARPER_31
    [StaticSeverityHighlighting(Severity.SUGGESTION)]
#endif
    internal sealed class InfoAnnotationHighlighting : AnnotationHighlighting
    {
        public InfoAnnotationHighlighting(Annotation annotation)
            : base(annotation)
        {
        }

#if RESHARPER_31
        public override string AttributeId
        {
            get { return HighlightingAttributeIds.SUGGESTION_ATTRIBUTE; }
        }

        public override OverlapResolvePolicy OverlapResolvePolicy
        {
            get { return OverlapResolvePolicy.NONE; }
        }

        public override Severity Severity
        {
            get { return Severity.SUGGESTION; }
        }
#endif
    }
}
