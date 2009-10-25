using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Platform;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Platform
{
    public class ProcessSupportTest
    {
        [Test]
        [Explicit("Must be verified manually due to significant OS dependencies.")]
        public void PrintProcessInformationForManualInspection()
        {
            TestLog.WriteLine("Is32BitProcess: {0}", ProcessSupport.Is32BitProcess);
            TestLog.WriteLine("Is64BitProcess: {0}", ProcessSupport.Is64BitProcess);
            TestLog.WriteLine("ProcessType: {0}", ProcessSupport.ProcessType);
            TestLog.WriteLine("ProcessIntegrityLevel: {0}", ProcessSupport.ProcessIntegrityLevel);
            TestLog.WriteLine("HasElevatedPrivileges: {0}", ProcessSupport.HasElevatedPrivileges);
        }
    }
}
