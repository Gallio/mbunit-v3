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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Data.Generation;
using MbUnit.Framework;
using Gallio.Framework;
using Gallio.Common.Collections;
using Gallio.Common.Text.RegularExpression;
using Rhino.Mocks;

namespace Gallio.Tests.Common.Text.RegularExpression
{
    [TestFixture]
    [TestsOn(typeof(ElementGroup))]
    public class ElementGroupTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_quantifier_should_throw_exception()
        {
            new ElementGroup(null, EmptyArray<IElement>.Instance);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_children_should_throw_exception()
        {
            new ElementGroup(Quantifier.One, null);
        }

        [Test]
        [Row(1, 1, new[] { "ABCDEFGHI" })]
        [Row(2, 3, new[] { "ABCDEFGHIABCDEFGHI", "ABCDEFGHIABCDEFGHIABCDEFGHI" })]
        public void GetRandomString(int minimum, int maximum, string[] expected)
        {
            var mockChild1 = MockRepository.GenerateMock<IElement>();
            var mockChild2 = MockRepository.GenerateMock<IElement>();
            var mockChild3 = MockRepository.GenerateMock<IElement>();
            mockChild1.Expect(x => x.GetRandomString()).Repeat.Any().Return("ABC");
            mockChild2.Expect(x => x.GetRandomString()).Repeat.Any().Return("DEF");
            mockChild3.Expect(x => x.GetRandomString()).Repeat.Any().Return("GHI");
            var element = new ElementGroup(new Quantifier(minimum, maximum), new[] { mockChild1, mockChild2, mockChild3 });
            string actual = element.GetRandomString();
            Assert.Contains(expected, actual);
            mockChild1.VerifyAllExpectations();
            mockChild2.VerifyAllExpectations();
            mockChild3.VerifyAllExpectations();
        }
    }
}
