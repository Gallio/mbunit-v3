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
using Gallio.Common.Markup;
using Gallio.Common.Text.RegularExpression;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using NHamcrest.Core;

namespace Gallio.Tests.Common.Markup.Tags
{
    public class BinaryAttachmentTest
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<BinaryAttachment>
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { new BinaryAttachment("abc", MimeTypes.PlainText, new byte[] { 1, 2, 3 }) },
                { new BinaryAttachment("def", MimeTypes.PlainText, new byte[] { 1, 2, 3 }) },
                { new BinaryAttachment("abc", MimeTypes.Xml, new byte[] { 1, 2, 3 }) },
                { new BinaryAttachment("abc", MimeTypes.PlainText, new byte[] { 1, 2 }) }
            }
        };

        [Test]
        public void NullAttachmentNamePicksAUniqueOne()
        {
            Assert.IsNotNull(new BinaryAttachment(null, MimeTypes.PlainText, new byte[0]));
        }

        [VerifyContract]
        public readonly IContract HashCodeAcceptanceTests = new HashCodeAcceptanceContract<BinaryAttachment>
        {
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = GetDistinctInstances()
        };

        private static IEnumerable<BinaryAttachment> GetDistinctInstances()
        {
            var nameGenerator = new RegexLite(@"[A-Za-z0-9]{5,30}");
            var random = new Random();

            for (int i = 0; i < 10000; i++)
                foreach (var mimeType in MimeTypes.All)
                {
                    var bytes = new byte[random.Next(0, 1000)];
                    random.NextBytes(bytes);
                    yield return new BinaryAttachment(nameGenerator.GetRandomString(random), mimeType, bytes);
                }
        }

    	[Test]
    	public void Throw_if_name_is_too_long()
    	{
			Assert.That(() => new BinaryAttachment("aVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryVeryLongString", MimeTypes.Png, new byte[0]), 
				Throws.An<ArgumentException>()
					.With(e => e.Message == "name must be 100 chars or less\r\nParameter name: name" && e.ParamName == "name"));
    	}
    }
}
