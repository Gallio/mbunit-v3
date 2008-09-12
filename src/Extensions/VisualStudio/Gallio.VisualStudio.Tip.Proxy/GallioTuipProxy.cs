using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Proxies the <see cref="ITuip" /> interface over to the actual implementation
    /// after initializing the loader.
    /// </summary>
    public class GallioTuipProxy : ITuip, SGallioTestService
    {
        private readonly ITuip target;

        public GallioTuipProxy(IServiceProvider serviceProvider)
        {
            target = ProxyHelper.GetTargetFactory().CreateTuip(serviceProvider);
        }

        public void InvokeEditor(UIBlob uiBlob, ITestElement test)
        {
            target.InvokeEditor(uiBlob, test);
        }

        public void InvokeResultViewer(TestResultMessage result)
        {
            target.InvokeResultViewer(result);
        }

        public void CloseResultViewer(TestResultMessage result)
        {
            target.CloseResultViewer(result);
        }

        public void UpdateTestProperty(ITestElement test, PropertyDescriptor propertyToChange)
        {
            target.UpdateTestProperty(test, propertyToChange);
        }

        public void UpdateTestCustomProperty(ITestElement test, string propertyToChange)
        {
            target.UpdateTestCustomProperty(test, propertyToChange);
        }

        public bool IsTestPropertiesReadOnly(ITestElement test)
        {
            return target.IsTestPropertiesReadOnly(test);
        }

        public IRunConfigurationCustomEditor RunConfigurationEditor
        {
            get { return target.RunConfigurationEditor; }
        }
    }
}
