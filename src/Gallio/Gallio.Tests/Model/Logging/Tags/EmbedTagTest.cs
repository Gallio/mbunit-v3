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


using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    public class EmbedTagTest : BaseTagTest<EmbedTag>
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<EmbedTag>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = equivalenceClasses
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

        protected override void PrepareLogWriterForWriteToTest(TestLogWriter writer)
        {
            writer.AttachPlainText("attachment1", "abc");
            writer.AttachPlainText("attachment2", "def");
        }
    }
}
