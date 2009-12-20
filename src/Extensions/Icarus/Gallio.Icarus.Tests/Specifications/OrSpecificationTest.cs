using Gallio.Icarus.Specifications;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Specifications
{
    [TestsOn(typeof(OrSpecification<>))]
    public class OrSpecificationTest
    {
        private ISpecification<OrSpecificationTest> left;
        private ISpecification<OrSpecificationTest> right;
        private OrSpecification<OrSpecificationTest> specification;

        [SetUp]
        public void Establish_context()
        {
            left = MockRepository.GenerateStub<ISpecification<OrSpecificationTest>>();
            right = MockRepository.GenerateStub<ISpecification<OrSpecificationTest>>();
            specification = new OrSpecification<OrSpecificationTest>(left, right);            
        }

        [Test]
        public void Match_should_return_true_if_both_specs_match()
        {
            left.Stub(s => s.Matches(this)).Return(true);
            right.Stub(s => s.Matches(this)).Return(true);

            var matches = specification.Matches(this);

            Assert.IsTrue(matches);
        }

        [Test]
        public void Match_should_return_true_if_only_left_does()
        {
            left.Stub(s => s.Matches(this)).Return(true);

            var matches = specification.Matches(this);

            Assert.IsTrue(matches);
        }

        [Test]
        public void Match_should_return_true_if_only_right_does()
        {
            right.Stub(s => s.Matches(this)).Return(true);

            var matches = specification.Matches(this);

            Assert.IsTrue(matches);
        }

        [Test]
        public void Match_should_return_false_if_both_do()
        {
            var matches = specification.Matches(this);

            Assert.IsFalse(matches);
        }
    }
}
