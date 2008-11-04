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
using MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor;

namespace MbUnit.Tests.Framework.ContractVerifiers.HasConstructor
{
    [TestFixture]
    public class HasConstructorPatternBuilderTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void SetNullTargetType()
        {
            new HasConstructorPatternBuilder().SetTargetType(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void SetNullFriendlyName()
        {
            new HasConstructorPatternBuilder().SetName(null);
        }

        [Test]
        [ExpectedArgumentException]
        public void SetNullParameterTypes()
        {
            new HasConstructorPatternBuilder().SetParameterTypes(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeWithMissingTargetType()
        {
            new HasConstructorPatternBuilder()
                .SetName("Hello")
                .SetParameterTypes(typeof(int), typeof(string))
                .SetAccessibility(HasConstructorAccessibility.NonPublic)
                .ToPattern();
        }

        [Test]
        public void MakePatternOk()
        {
            var pattern = new HasConstructorPatternBuilder()
                .SetTargetType(typeof(object))
                .SetName("Hello")
                .SetParameterTypes(typeof(int), typeof(string))
                .SetAccessibility(HasConstructorAccessibility.NonPublic)
                .ToPattern();
            Assert.IsNotNull(pattern);
            Assert.IsInstanceOfType(typeof(HasConstructorPattern), pattern);
        }
    }
}
