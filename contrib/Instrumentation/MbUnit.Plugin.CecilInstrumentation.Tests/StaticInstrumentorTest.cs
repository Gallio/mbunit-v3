extern alias MbUnit2;
using System;
using System.Reflection;
using MbUnit.Plugin.CecilInstrumentation;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Plugin.CecilInstrumentation.Tests
{
    [TestFixture]
    [TestsOn(typeof(StaticInstrumentor))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class StaticInstrumentorTest : BaseInstrumentorTest<StaticInstrumentor>
    {
    }
}