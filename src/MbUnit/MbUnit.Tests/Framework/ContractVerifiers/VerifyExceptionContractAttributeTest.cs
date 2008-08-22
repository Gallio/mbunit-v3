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
using System.Linq;
using System.Runtime.Serialization;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [TestsOn(typeof(VerifyExceptionContractAttribute))]
    public class VerifyExceptionContractAttributeTest : VerifyContractAttributeBaseTest
    {
        [Test]
        [Row(typeof(FullContractOnSerializedExceptionSampleTest), "HasSerializableAttribute", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSampleTest), "HasSerializationConstructor", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSampleTest), "IsDefaultConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSampleTest), "IsStandardMessageConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSampleTest), "IsStandardMessageAndInnerExceptionConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(FullContractOnUnserializedExceptionSampleTest), "HasSerializableAttribute", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSampleTest), "HasSerializationConstructor", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSampleTest), "IsDefaultConstructorWellDefined", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSampleTest), "IsStandardMessageConstructorWellDefined", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSampleTest), "IsStandardMessageAndInnerExceptionConstructorWellDefined", TestStatus.Failed)]
        [Row(typeof(PartialContractOnUnserializedExceptionSampleTest), "HasSerializableAttribute", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnUnserializedExceptionSampleTest), "HasSerializationConstructor", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnUnserializedExceptionSampleTest), "IsDefaultConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(PartialContractOnUnserializedExceptionSampleTest), "IsStandardMessageConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(PartialContractOnUnserializedExceptionSampleTest), "IsStandardMessageAndInnerExceptionConstructorWellDefined", TestStatus.Passed)]
        public void VerifySampleExceptionContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("ExceptionContract", fixtureType, testMethodName, expectedTestStatus);
        }

        [VerifyExceptionContract(typeof(SerializedExceptionSample),
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true),
        Explicit]
        private class FullContractOnSerializedExceptionSampleTest
        {
        }

        [VerifyExceptionContract(typeof(UnserializedExceptionSample),
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true),
        Explicit]
        private class FullContractOnUnserializedExceptionSampleTest
        {
        }

        [VerifyExceptionContract(typeof(UnserializedExceptionSample),
            ImplementsSerialization = false,
            ImplementsStandardConstructors = true),
        Explicit]
        private class PartialContractOnUnserializedExceptionSampleTest
        {
        }

        /// <summary>
        /// Sample exception which has the 3 recommended constructors
        /// and supports serialization.
        /// </summary>
        [Serializable]
        internal class SerializedExceptionSample : Exception, ISerializable
        {
            public SerializedExceptionSample()
            {
            }

            public SerializedExceptionSample(string message)
                : base(message)
            {
            }

            public SerializedExceptionSample(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            protected SerializedExceptionSample(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
            }
        }

        /// <summary>
        /// Sample exception which has the 3 recommended constructors
        /// but does not support serialization.
        /// </summary>
        internal class UnserializedExceptionSample : Exception
        {
            public UnserializedExceptionSample()
            {
            }

            public UnserializedExceptionSample(string message)
                : base(message)
            {
            }

            public UnserializedExceptionSample(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}
