using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Gallio.Common.Splash.Internal
{
    internal class EmbeddedObjectSite : IEmbeddedObjectSite
    {
        public Control ParentControl { get; set; }

        public Style ParagraphStyle { get; set; }

        public Style InlineStyle { get; set; }

        public bool RightToLeft { get; set; }

        public int CharIndex { get; set; }
    }
}
