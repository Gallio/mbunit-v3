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

using System.Linq;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Markup.Tags
{
    public class EmbedTagTest : BaseTagTest<EmbedTag>
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<EmbedTag>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = equivalenceClasses
        };

        [VerifyContract]
        public readonly IContract HashCodeTests = new HashCodeAcceptanceContract<EmbedTag>
        {
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Random.Strings(1000000, @"[A-Za-z0-9 ]{5,50}").Select(x => new EmbedTag(x))
        };

        public override EquivalenceClassCollection<EmbedTag> GetEquivalenceClasses()
        {
            return equivalenceClasses;
        }

        private static EquivalenceClassCollection<EmbedTag> equivalenceClasses = new EquivalenceClassCollection<EmbedTag>
        {
            { new EmbedTag("attachment1") },
            { new EmbedTag("attachment2") },
        };

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfAttachmentIsNull()
        {
            new EmbedTag(null);
        }

        protected override void PrepareLogWriterForWriteToTest(MarkupDocumentWriter writer)
        {
            writer.AttachPlainText("attachment1", "abc");
            writer.AttachPlainText("attachment2", "def");
        }
    }
}
