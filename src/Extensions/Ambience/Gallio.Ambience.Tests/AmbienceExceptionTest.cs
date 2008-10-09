using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Ambience.Tests
{
    [VerifyExceptionContract(typeof(AmbienceException))]
    public class AmbienceExceptionTest
    {
    }
}
