// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
