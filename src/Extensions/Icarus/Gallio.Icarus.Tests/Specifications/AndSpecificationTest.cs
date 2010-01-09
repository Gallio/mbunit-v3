// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
