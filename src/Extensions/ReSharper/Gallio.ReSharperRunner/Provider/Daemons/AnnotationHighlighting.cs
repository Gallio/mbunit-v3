// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Drawing;
using Gallio.Model;
using JetBrains.ReSharper.Daemon;

namespace Gallio.ReSharperRunner.Provider.Daemons
{
    internal abstract class AnnotationHighlighting : IHighlighting
    {
        private readonly AnnotationState annotation;

        protected AnnotationHighlighting(AnnotationState annotation)
        {
            this.annotation = annotation;
        }

        public static AnnotationHighlighting CreateHighlighting(AnnotationState annotation)
        {
            switch (annotation.Type)
            {
                case AnnotationType.Error:
                    return new ErrorAnnotationHighlighting(annotation);
                case AnnotationType.Warning:
                    return new WarningAnnotationHighlighting(annotation);
                case AnnotationType.Info:
                    return new InfoAnnotationHighlighting(annotation);
                default:
                    throw new ArgumentException("Unsupported annotation.", "annotation");
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
            get { return ToolTip; }
        }

        public int NavigationOffsetPatch
        {
            get { return 0; }
        }

#if RESHARPER_31
        public abstract string AttributeId { get; }
        public abstract OverlapResolvePolicy OverlapResolvePolicy { get; }
        public abstract Severity Severity { get; }

        public Color ColorOnStripe
        {
            get { return Color.Empty; }
        }

        public bool ShowToolTipInStatusBar
        {
            get { return true; }
        }
#endif

#if RESHARPER_45_OR_NEWER
        public bool IsValid()
        {
            return true;
        }
#endif
    }
}