using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation.Tests
{
    [DebuggerDisplay("Sample")] // prevent ToString from being called automatically
    public class Sample : ISample
    {
        private readonly StringBuilder builder = new StringBuilder();

        public void AppendHello()
        {
            builder.Append("Hello");
        }

        public void Append(string message)
        {
            builder.Append(message);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
