using System.Reflection;
using Microsoft.VisualStudio.TestTools.Common;

namespace Gallio.VisualStudio.Tip
{
    internal class WarningHandler : IWarningHandler
    {
        private readonly ITmi tmi;

        public WarningHandler(ITmi tmi)
        {
            this.tmi = tmi;
        }

        public void Write(object sender, WarningEventArgs ea)
        {
            MethodInfo method = tmi.GetType().GetMethod("WriteWarning");
            method.Invoke(tmi, new object[] { sender, ea.Warning });
        }
    }
}