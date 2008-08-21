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
using Gallio.Collections;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging
{
    [TestsOn(typeof(StructuredText))]
    [VerifyEqualityContract(typeof(StructuredText))]
    public class StructuredTextTest : IEquivalenceClassProvider<StructuredText>
    {
        public EquivalenceClassCollection<StructuredText> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<StructuredText>.FromDistinctInstances(
                new StructuredText("lalalala"),
                new StructuredText(new BodyTag() { Contents = { new TextTag("blah") }}),
                new StructuredText(new BodyTag() { Contents = { new TextTag("blah") }},
                    new[] { new TextAttachment("abc", MimeTypes.PlainText, "blah") })
                );
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorWithTextThrowsIfStringIsNull()
        {
            new StructuredText((string)null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorWithBodyTagThrowsIfBodyTagIsNull()
        {
            new StructuredText((BodyTag)null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorWithBodyTagAndAttachmentThrowsIfBodyTagIsNull()
        {
            new StructuredText(null, EmptyArray<Attachment>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorWithBodyTagAndAttachmentThrowsIfAttachmentListIsNull()
        {
            new StructuredText(new BodyTag(), null);
        }

        [Test]
        public void ConstructorWithTextInitializesProperties()
        {
            StructuredText text = new StructuredText("blah");
            Assert.AreEqual(0, text.Attachments.Count);

            NewAssert.AreEqual(new BodyTag() { Contents = { new TextTag("blah") } }, text.BodyTag);
        }
    }
}
