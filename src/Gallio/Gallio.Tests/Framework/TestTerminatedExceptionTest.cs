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
using Gallio.Framework;
using Gallio.Model;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Framework
{
    [TestsOn(typeof(TestTerminatedException))]
    public class TestTerminatedExceptionTest
    {
        [VerifyContract]
        public readonly IContract ExceptionTests = new ExceptionContract<TestTerminatedException>()
        {
            ImplementsStandardConstructors = false            
        };

        [Test]
        public void OutcomeIsAsSpecifiedInConstructor()
        {
            var ex = new TestTerminatedException(TestOutcome.Error);
            Assert.AreEqual(TestOutcome.Error, ex.Outcome);
        }

        [Test]
        public void OutcomeSurvivesSerialization()
        {
            var ex = new TestTerminatedException(TestOutcome.Skipped);
            var deserializedEx = Assert.BinarySerializeThenDeserialize(ex);
            Assert.AreEqual(TestOutcome.Skipped, deserializedEx.Outcome);
        }
    }
}
