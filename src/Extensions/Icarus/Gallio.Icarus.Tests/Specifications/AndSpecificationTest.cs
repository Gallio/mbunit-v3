using Gallio.Icarus.Specifications;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Specifications
{
    [TestsOn(typeof(AnySpecification<>))]
    public class AndSpecificationTest
    {
        private ISpecification<AndSpecificationTest> left;
        private ISpecification<AndSpecificationTest> right;
        private AndSpecification<AndSpecificationTest> specification;

        [SetUp]
        public void Establish_context()
        {
            left = MockRepository.GenerateStub<ISpecification<AndSpecificationTest>>();
            right = MockRepository.GenerateStub<ISpecification<AndSpecificationTest>>();
            specification = new AndSpecification<AndSpecificationTest>(left, right);            
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
        public void Match_should_return_false_if_left_does()
        {
            right.Stub(s => s.Matches(this)).Return(true);

            var matches = specification.Matches(this);

            Assert.IsFalse(matches);
        }

        [Test]
        public void Match_should_return_false_if_right_does()
        {
            left.Stub(s => s.Matches(this)).Return(true);

            var matches = specification.Matches(this);

            Assert.IsFalse(matches);
        }

        [Test]
        public void Match_should_return_false_if_both_do()
        {
            var matches = specification.Matches(this);

            Assert.IsFalse(matches);
        }
    }
}
