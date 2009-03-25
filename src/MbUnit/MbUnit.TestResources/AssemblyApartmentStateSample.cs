using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Gallio.Framework;
using MbUnit.Framework;

// Ordinarily the default is STA, we change it to MTA here for testing purposes.
[assembly: ApartmentState(System.Threading.ApartmentState.MTA)]

namespace MbUnit.TestResources
{
    public class AssemblyApartmentStateSample
    {
        // This is checked by ApartmentStateTest in MbUnit.Tests.
        [Test]
        public void WriteApartmentStateToLog()
        {
            TestLog.Write(Thread.CurrentThread.GetApartmentState());
        }
    }
}
