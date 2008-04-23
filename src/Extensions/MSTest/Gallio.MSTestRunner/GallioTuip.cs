using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.MSTestRunner
{
    internal class GallioTuip : BaseTuip, SGallioTestService
    {
        private ITmi tmi;

        public GallioTuip(GallioPackage serviceProvider)
            : base(serviceProvider)
        {
        }

        public ITmi Tmi
        {
            get
            {
                if (tmi == null)
                {
                    PropertyInfo property = GetType().GetProperty("TmiInstance", BindingFlags.Instance | BindingFlags.NonPublic);
                    tmi = (ITmi)property.GetValue(this, null);
                }

                return tmi;
            }
        }

        public override void InvokeResultViewer(TestResultMessage result)
        {
        }

        public override void CloseResultViewer(TestResultMessage result)
        {
        }

        public override IRunConfigurationCustomEditor RunConfigurationEditor
        {
            get { return null; }
        }
    }
}
