using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Gallio.MSTestAdapter.Model;
using Gallio.Runner.Caching;

namespace Gallio.MSTestAdapter.Wrapper
{
    internal class MSTestRunner2005 : MSTestRunner
    {
        public MSTestRunner2005(IDiskCache diskCache) : base(diskCache)
        {
        }

        protected override string GetVisualStudioVersion()
        {
            return "8.0";
        }

        protected override void WriteTestList(XmlWriter writer, IEnumerable<MSTest> tests, string assemblyFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
