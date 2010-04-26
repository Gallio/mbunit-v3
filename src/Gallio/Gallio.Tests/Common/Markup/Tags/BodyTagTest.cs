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
using Gallio.Common.Markup.Tags;
using Gallio.Common.Text.RegularExpression;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Markup.Tags
{
    public class BodyTagTest : BaseTagTest<BodyTag>
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<BodyTag>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = equivalenceClasses
        };

        public override EquivalenceClassCollection<BodyTag> GetEquivalenceClasses()
        {
            return equivalenceClasses;
        }

        private readonly static EquivalenceClassCollection<BodyTag> equivalenceClasses = new EquivalenceClassCollection<BodyTag>
        {
            new BodyTag(),
            new BodyTag { Contents = { new TextTag("text") }},
            new BodyTag { Contents = { new TextTag("text"), new TextTag("more") }}
        };

        [VerifyContract]
        public readonly IContract HashCodeAcceptanceTests = new HashCodeAcceptanceContract<BodyTag>
        {
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = GetDistinctInstances()
        };

        private static IEnumerable<BodyTag> GetDistinctInstances()
        {
            var nameGenerator = new RegexLite(@"[A-Za-z0-9]{5,30}");
            var random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                var tag = new BodyTag();
                int count = random.Next(0, 10);

                for (int j = 0; j < count; j++)
                    tag.Contents.Add(new TextTag(nameGenerator.GetRandomString()));

                yield return tag;
            }
        }
    }
}
