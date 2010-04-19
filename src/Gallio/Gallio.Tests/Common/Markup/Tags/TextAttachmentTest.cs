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
using System.Linq;
using System.Collections.Generic;
using Gallio.Common.Markup;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Markup.Tags
{
    public class TextAttachmentTest
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<TextAttachment>
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                new TextAttachment("abc", MimeTypes.PlainText, "text"),
                new TextAttachment("def", MimeTypes.PlainText, "text"),
                new TextAttachment("abc", MimeTypes.Xml, "text"),
                new TextAttachment("abc", MimeTypes.PlainText, "blah")
            }
        };

        [VerifyContract]
        public readonly IContract HashCodeTests = new HashCodeAcceptanceContract<TextAttachment>
        {
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = GetDistinctInstances()
        };

        private static IEnumerable<TextAttachment> GetDistinctInstances()
        {
            foreach (var contentType in MimeTypes.All)
            {
                foreach (var text in DataGenerators.Strings.Random(200, @"[A-Za-z0-9 ]{5,50}"))
                {
                    foreach (var name in DataGenerators.Strings.Random(200, @"[A-Za-z0-9]{5,30}"))
                        yield return new TextAttachment(name, contentType, text);

                    for (int i = 0; i < 200; i++) // Null name will be replaced by a 64-bit hash.
                        yield return new TextAttachment(null, contentType, text);
                }
            }
        }

        [Test]
        public void NullAttachmentNamePicksAUniqueOne()
        {
            Assert.IsNotNull(new TextAttachment(null, MimeTypes.PlainText, "foo"));
        }
    }
}
