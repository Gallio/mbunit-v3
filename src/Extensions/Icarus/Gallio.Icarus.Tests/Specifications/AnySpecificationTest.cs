using Gallio.Icarus.Specifications;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Specifications
{
    [TestsOn(typeof(AnySpecification<>))]
    public class AnySpecificationTest
    {
        [Test]
        public void Match_should_always_return_true()
        {
            var specification = new AnySpecification<AnySpecificationTest>();

            var matches = specification.Matches(null);

            Assert.IsTrue(matches);
        }
    }
}
