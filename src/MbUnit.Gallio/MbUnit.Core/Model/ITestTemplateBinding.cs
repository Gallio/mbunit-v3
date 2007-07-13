using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Model
{
    public interface ITestTemplateBinding
    {
        ITestTemplate Template { get; }

        TestScope Scope { get; }

        IDictionary<string, object> Arguments { get; }

    }
}
