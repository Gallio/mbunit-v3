using System;
using Gallio.Model;
using JetBrains.ReSharper.Daemon;

namespace Gallio.ReSharperRunner.Daemons
{
#if ! RESHARPER_31
    [StaticSeverityHighlighting(Severity.ERROR)]
#endif
    internal sealed class ErrorAnnotationHighlighting : AnnotationHighlighting
    {
        public ErrorAnnotationHighlighting(Annotation annotation)
            : base(annotation)
        {
        }

#if RESHARPER_31
        public override string AttributeId
        {
            get { return HighlightingAttributeIds.ERROR_ATTRIBUTE; }
        }

        public override OverlapResolvePolicy OverlapResolvePolicy
        {
            get{ return OverlapResolvePolicy.ERROR; }
        }

        public override Severity Severity
        {
            get { return Severity.ERROR; }
        }
#endif
    }
}
