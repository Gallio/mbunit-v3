using System;
using Gallio.Framework.Utilities;
using NBehave.Spec.Framework;

namespace NBehave.Specs.Spec.Framework
{
    public abstract class SpecWithSample<TSample>
    {
        private SampleRunner sampleRunner;

        [ContextSetUp]
        public void SetUp()
        {
            sampleRunner = new SampleRunner();
            sampleRunner.AddFixture(typeof(TSample));
            sampleRunner.Run();
        }

        public SampleRunner Runner
        {
            get { return sampleRunner; }
        }
    }
}