using System;
using System.Drawing;
using Gallio.Model;
using JetBrains.ReSharper.Daemon;

namespace Gallio.ReSharperRunner
{
    internal class GallioAnnotationHighlighting : IHighlighting
    {
        private readonly Annotation annotation;

        public GallioAnnotationHighlighting(Annotation annotation)
        {
            this.annotation = annotation;
        }

        public string AttributeId
        {
            get { return "Gallio Annotation"; }
        }

        public OverlapResolvePolicy OverlapResolvePolicy
        {
            get { return OverlapResolvePolicy.NONE; }
        }

        public Severity Severity
        {
            get
            {
                switch (annotation.Type)
                {
                    case AnnotationType.Info:
                        return Severity.INFO;

                    case AnnotationType.Warning:
                        return Severity.WARNING;

                    case AnnotationType.Error:
                        return Severity.ERROR;

                    default:
                        return Severity.DO_NOT_SHOW;
                }
            }
        }

        public string ToolTip
        {
            get
            {
                if (annotation.Details == null)
                    return annotation.Message;
                return string.Concat(annotation.Message, "\n", annotation.Details);
            }
        }

        public string ErrorStripeToolTip
        {
            get { return null; }
        }

        public Color ColorOnStripe
        {
            get { return Color.Empty; }
        }

        public bool ShowToolTipInStatusBar
        {
            get { return false; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }
    }
}
