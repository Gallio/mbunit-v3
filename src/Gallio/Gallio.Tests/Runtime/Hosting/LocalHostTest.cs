using System;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestFixture]
    [TestsOn(typeof(LocalHost))]
    [TestsOn(typeof(LocalHostFactory))]
    public class LocalHostTest : AbstractHostFactoryTest
    {
        public override IHostFactory Factory
        {
            get { return new LocalHostFactory(); }
        }
    }
}