// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers.Patterns.HasAttribute;

namespace MbUnit.Tests.Framework.ContractVerifiers.HasAttribute
{
    [TestFixture]
    public class HasAttributePatternBuilderTest
    {
        private class CustomAttribute : Attribute
        {
        }


        [Test]
        [ExpectedArgumentNullException]
        public void SetNullTargetType()
        {
            new HasAttributePatternBuilder().SetTargetType(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullAttributeType()
        {
            new HasAttributePatternBuilder().SetAttributeType(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void SetNonAttributeType()
        {
            new HasAttributePatternBuilder().SetAttributeType(typeof(object));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakePatternWithMissingTargetType()
        {
            new HasAttributePatternBuilder()
                .SetAttributeType(typeof(CustomAttribute))
                .ToPattern();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakePatternWithMissingAttributeType()
        {
            new HasAttributePatternBuilder()
                .SetTargetType(typeof(object))
                .ToPattern();
        }

        [Test]
        public void MakePatternOk()
        {
            var pattern = new HasAttributePatternBuilder()
                .SetTargetType(typeof(object))
                .SetAttributeType(typeof(CustomAttribute))
                .ToPattern();
            Assert.IsNotNull(pattern);
            Assert.IsInstanceOfType(typeof(HasAttributePattern), pattern);
        }
    }
}
