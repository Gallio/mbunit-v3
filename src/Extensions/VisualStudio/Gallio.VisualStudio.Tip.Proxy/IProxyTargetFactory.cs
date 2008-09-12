using System;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.TestAdapter;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Constructs instances of target interfaces from the proxy.
    /// </summary>
    public interface IProxyTargetFactory
    {
        ITestAdapter CreateTestAdapter();

        ITip CreateTip(ITmi tmi);

        ITuip CreateTuip(IServiceProvider serviceProvider);
    }
}
