using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runner.Caching;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal class MSTestRunner2010 : MSTestRunner2008
    {
        public MSTestRunner2010(IDiskCache diskCache) : base(diskCache)
        {
        }

        protected override string GetVisualStudioVersion()
        {
            return "10.0";
        }
    }
}
