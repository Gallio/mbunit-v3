using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Model
{
    public interface ITestTemplateBinding
    {
        ITestTemplate Template { get; }

        TestScope Scope { get; }

        IDictionary<string, object> Arguments { get; }

    }
}
