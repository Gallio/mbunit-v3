using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation.Tests
{
    [DebuggerDisplay("SampleWrapper")] // prevent ToString from being called automatically
    public class SampleWrapper : ISample
    {
        private readonly object instance;

        public SampleWrapper(object instance)
        {
            this.instance = instance;
        }

        public object Instance
        {
            get { return instance; }
        }

        public void AppendHello()
        {
            instance.GetType().InvokeMember(@"AppendHello", BindingFlags.InvokeMethod |
                BindingFlags.Instance | BindingFlags.Public, null, instance,
                null);
        }

        public void Append(string message)
        {
            instance.GetType().InvokeMember(@"Append", BindingFlags.InvokeMethod |
                BindingFlags.Instance | BindingFlags.Public, null, instance,
                new object[] { message });
        }

        public override string ToString()
        {
            return (string) instance.GetType().InvokeMember(@"ToString", BindingFlags.InvokeMethod |
                BindingFlags.Instance | BindingFlags.Public, null, instance,
                null);
        }
    }
}
