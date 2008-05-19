using System;
using Gallio.Model;
using JetBrains.ReSharper.Daemon;

namespace Gallio.ReSharperRunner.Daemons
{
#if ! RESHARPER_31
    [StaticSeverityHighlighting(Severity.WARNING)]
#endif
    internal sealed class WarningAnnotationHighlighting : AnnotationHighlighting
    {
        public WarningAnnotationHighlighting(Annotation annotation)
            : base(annotation)
        {
        }

#if RESHARPER_31
        public override string AttributeId
        {
            get { return HighlightingAttributeIds.WARNING_ATTRIBUTE; }
        }

        public override OverlapResolvePolicy OverlapResolvePolicy
        {
            get{ return OverlapResolvePolicy.WARNING; }
        }

        public override Severity Severity
        {
            get { return Severity.WARNING; }
        }
#endif
    }
}
