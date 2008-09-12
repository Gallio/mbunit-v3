using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.TestAdapter;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.VisualStudio.Tip
{
    public class DefaultProxyTargetFactory : IProxyTargetFactory
    {
        public ITestAdapter CreateTestAdapter()
        {
            return new GallioTestAdapter();
        }

        public ITip CreateTip(ITmi tmi)
        {
            return new GallioTip(tmi);
        }

        public ITuip CreateTuip(IServiceProvider serviceProvider)
        {
            return new GallioTuip(serviceProvider);
        }
    }
}
