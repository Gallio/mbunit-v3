using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.VisualStudio.Interop.Tests
{
    [TestsOn(typeof(VisualStudioException))]
    public class VisualStudioExceptionTest
    {
        [VerifyContract]
        public readonly IContract ExceptionContract = new ExceptionContract<VisualStudioException>()
        {
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true
        };
    }
}
