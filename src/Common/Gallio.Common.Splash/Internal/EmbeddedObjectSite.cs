using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    internal class EmbeddedObjectSite : IEmbeddedObjectSite
    {
        public Style ParagraphStyle { get; set; }

        public Style InlineStyle { get; set; }

        public bool RightToLeft { get; set; }

        public int CharIndex { get; set; }
    }
}
