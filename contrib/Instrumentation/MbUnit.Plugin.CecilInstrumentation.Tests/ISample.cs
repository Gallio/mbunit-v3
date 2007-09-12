using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation.Tests
{
    public interface ISample
    {
        void AppendHello();

        void Append(string message);
    }
}
