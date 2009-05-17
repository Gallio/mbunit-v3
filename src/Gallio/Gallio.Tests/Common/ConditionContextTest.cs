// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
