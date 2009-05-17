using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Common
{
    [TestsOn(typeof(ConditionContext))]
    public class ConditionContextTest
    {
        [Test]
        public void HasProperty_WhenNamespaceIsNull_Throws()
        {
            var context = MockRepository.GenerateStub<MockableConditionContext>();

            Assert.Throws<ArgumentNullException>(() => context.HasProperty(null, "identifier"));
        }

        [Test]
        public void HasProperty_WhenIdentifierIsNull_Throws()
        {
            var context = MockRepository.GenerateStub<MockableConditionContext>();

            Assert.Throws<ArgumentNullException>(() => context.HasProperty("namespace", null));
        }

        [Test]
        public void HasProperty_WhenArgumentsValid_ForwardsToImpl()
        {
            var context = MockRepository.GenerateStub<MockableConditionContext>();
            context.Stub(x => x.HasProperty("namespace", "identifier")).Return(true);

            Assert.AreEqual(true, context.HasProperty("namespace", "identifier"));
        }
    }
}
