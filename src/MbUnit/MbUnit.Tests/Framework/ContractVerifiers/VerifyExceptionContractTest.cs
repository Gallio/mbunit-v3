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
using Gallio.Tests;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [RunSample(typeof(FullContractOnSerializedExceptionSample))]
    [RunSample(typeof(FullContractOnUnserializedExceptionSample))]
    [RunSample(typeof(PartialContractOnUnserializedExceptionSample))]
    public class VerifyExceptionContractTest : AbstractContractVerifierTest
    {
        [Test]
        [Row(typeof(FullContractOnSerializedExceptionSample), "HasSerializableAttribute", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSample), "HasSerializationConstructor", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSample), "IsDefaultConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSample), "IsStandardMessageConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(FullContractOnSerializedExceptionSample), "IsStandardMessageAndInnerExceptionConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(FullContractOnUnserializedExceptionSample), "HasSerializableAttribute", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSample), "HasSerializationConstructor", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSample), "IsDefaultConstructorWellDefined", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSample), "IsStandardMessageConstructorWellDefined", TestStatus.Failed)]
        [Row(typeof(FullContractOnUnserializedExceptionSample), "IsStandardMessageAndInnerExceptionConstructorWellDefined", TestStatus.Failed)]
        [Row(typeof(PartialContractOnUnserializedExceptionSample), "HasSerializableAttribute", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnUnserializedExceptionSample), "HasSerializationConstructor", TestStatus.Inconclusive)]
        [Row(typeof(PartialContractOnUnserializedExceptionSample), "IsDefaultConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(PartialContractOnUnserializedExceptionSample), "IsStandardMessageConstructorWellDefined", TestStatus.Passed)]
        [Row(typeof(PartialContractOnUnserializedExceptionSample), "IsStandardMessageAndInnerExceptionConstructorWellDefined", TestStatus.Passed)]
        public void VerifySampleExceptionContract(Type fixtureType, string testMethodName, TestStatus expectedTestStatus)
        {
            VerifySampleContract("ExceptionTests", fixtureType, testMethodName, expectedTestStatus);
        }

        [Explicit]
        internal class FullContractOnSerializedExceptionSample
        {
            public readonly IContractVerifier ExceptionTests = new VerifyExceptionContract<SampleSerializedException>()
            {
                ImplementsSerialization = true,
                ImplementsStandardConstructors = true
            };
        }

        [Explicit]
        internal class FullContractOnUnserializedExceptionSample
        {
            public readonly IContractVerifier ExceptionTests = new VerifyExceptionContract<SampleUnserializedException>()
            {
                ImplementsSerialization = true,
                ImplementsStandardConstructors = true
            };
        }

        [Explicit]
        internal class PartialContractOnUnserializedExceptionSample
        {
            public readonly IContractVerifier ExceptionTests = new VerifyExceptionContract<SampleUnserializedException>()
            {
                ImplementsSerialization = false,
                ImplementsStandardConstructors = true
            };
        }

        /// <summary>
        /// Sample exception which has the 3 recommended constructors
        /// and supports serialization.
        /// </summary>
        [Serializable]
        internal class SampleSerializedException : Exception, ISerializable
        {
            public SampleSerializedException()
            {
            }

            public SampleSerializedException(string message)
                : base(message)
            {
            }

            public SampleSerializedException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            protected SampleSerializedException(SerializationInfo info, StreamingContext context)
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
        internal class SampleUnserializedException : Exception
        {
            public SampleUnserializedException()
            {
            }

            public SampleUnserializedException(string message)
                : base(message)
            {
            }

            public SampleUnserializedException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}
