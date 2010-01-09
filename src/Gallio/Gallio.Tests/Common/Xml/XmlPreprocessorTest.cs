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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Gallio.Common.Xml;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Xml
{
    [TestsOn(typeof(XmlPreprocessor))]
    public class XmlPreprocessorTest
    {
        [Test]
        public void Define_WhenConstantIsNull_Throws()
        {
            var preprocessor = new XmlPreprocessor();

            Assert.Throws<ArgumentNullException>(() => preprocessor.Define(null));
        }

        [Test]
        public void Define_WhenConstantAlreadyDefined_DoesNothing()
        {
            var preprocessor = new XmlPreprocessor();
            preprocessor.Define("CONSTANT");

            preprocessor.Define("CONSTANT");

            Assert.IsTrue(preprocessor.IsDefined("CONSTANT"));
        }

        [Test]
        public void IsDefined_WhenConstantIsNull_Throws()
        {
            var preprocessor = new XmlPreprocessor();

            Assert.Throws<ArgumentNullException>(() => preprocessor.IsDefined(null));
        }

        [Test]
        public void IsDefined_WhenConstantIsDefined_ReturnsTrue()
        {
            var preprocessor = new XmlPreprocessor();
            preprocessor.Define("CONSTANT");

            Assert.IsTrue(preprocessor.IsDefined("CONSTANT"));
        }

        [Test]
        public void IsDefined_WhenConstantIsNotDefined_ReturnsFalse()
        {
            var preprocessor = new XmlPreprocessor();

            Assert.IsFalse(preprocessor.IsDefined("CONSTANT"));
        }

        [Test]
        public void Preprocess_WhenReaderIsNull_Throws()
        {
            var preprocessor = new XmlPreprocessor();
            var writer = XmlWriter.Create(new StringBuilder());

            Assert.Throws<ArgumentNullException>(() => preprocessor.Preprocess(null, writer));
        }

        [Test]
        public void Preprocess_WhenWriterIsNull_Throws()
        {
            var preprocessor = new XmlPreprocessor();
            var reader = XmlReader.Create(new StringReader(""));

            Assert.Throws<ArgumentNullException>(() => preprocessor.Preprocess(reader, null));
        }

        [Test]
        public void Preprocess_WhenPresetDefinesBeforeProcessing_UsesThem()
        {
            var preprocessor = new XmlPreprocessor();
            preprocessor.Define("CONSTANT");
            string input = "<root><?ifdef CONSTANT?>Text<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>Text</root>", output);
        }

        [Test]
        public void Preprocess_WhenNewDefinesCreatedDuringPreprocessing_SavesThem()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT?></root>";

            PreprocessString(preprocessor, input);

            Assert.IsTrue(preprocessor.IsDefined("CONSTANT"));
        }

        [Test]
        public void Preprocess_WhenNewDefinesCreatedWithExcessWhitespaceDuringPreprocessing_TrimsAndSavesThem()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT    ?></root>";

            PreprocessString(preprocessor, input);

            Assert.IsTrue(preprocessor.IsDefined("CONSTANT"));
        }

        [Test]
        public void Preprocess_WhenIfDefPresentAndConstantDefined_IncludesIfBlock()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT?><?ifdef CONSTANT?>If<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>If</root>", output);
        }

        [Test]
        public void Preprocess_WhenIfDefPresentWithExcessWhitespaceAndConstantDefined_IncludesIfBlock()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT?><?ifdef CONSTANT    ?>If<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>If</root>", output);
        }

        [Test]
        public void Preprocess_WhenIfDefPresentAndConstantNotDefined_ExcludesIfBlock()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?ifdef CONSTANT?>If<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root></root>", output);
        }

        [Test]
        public void Preprocess_WhenIfNDefPresentAndConstantNotDefined_IncludesIfBlock()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?ifndef CONSTANT?>If<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>If</root>", output);
        }

        [Test]
        public void Preprocess_WhenIfNDefPresentAndConstantDefined_ExcludesIfBlock()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT?><?ifndef CONSTANT?>If<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root></root>", output);
        }

        [Test]
        public void Preprocess_WhenIfNDefPresentWithExcessWhitespaceAndConstantDefined_ExcludesIfBlock()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT?><?ifndef CONSTANT    ?>If<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root></root>", output);
        }

        [Test]
        public void Preprocess_WhenIfDefAndElsePresentAndConstantDefined_IncludesIfBlockExcludesElse()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT?><?ifdef CONSTANT?>If<?else?>Else<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>If</root>", output);
        }

        [Test]
        public void Preprocess_WhenIfDefAndElsePresentAndConstantNotDefined_ExcludesIfBlockIncludesElse()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?ifdef CONSTANT?>If<?else?>Else<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>Else</root>", output);
        }

        [Test]
        public void Preprocess_WhenIfNDefAndElsePresentAndConstantNotDefined_IncludesIfBlockExcludesElse()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?ifndef CONSTANT?>If<?else?>Else<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>If</root>", output);
        }

        [Test]
        public void Preprocess_WhenIfNDefAndElsePresentAndConstantDefined_ExcludesIfBlockIncludesElse()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?define CONSTANT?><?ifndef CONSTANT?>If<?else?>Else<?endif?></root>";

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual("<root>Else</root>", output);
        }

        [Test]
        [Row("<root><?define A?><?ifdef A?><?define B?><?endif?><?ifdef B?>If<?endif?></root>",
            "<root>If</root>",
            Description = "<?define?> inside <?ifdef?> should only be defined when condition satisfied.")]
        [Row("<root><?ifdef A?><?define B?><?endif?><?ifdef B?>If<?endif?></root>",
            "<root></root>",
            Description = "<?define?> inside <?ifdef?> should not be defined when condition not satisfied.")]
        [Row("<root><?define B?><?ifdef A?><?ifdef B?>If<?endif?><?endif?></root>",
            "<root></root>",
            Description = "<?ifdef?> inside <?ifdef?> should not be satisfied if outer condition not satisfied.")]
        [Row("<root><?define A?><?define B?><?ifdef A?><?ifdef B?>If<?endif?><?endif?></root>",
            "<root>If</root>",
            Description = "<?ifdef?> inside <?ifdef?> should be satisfied if it is satisfied and outer condition satisfied.")]
        [Row("<root><?define A?><?ifdef A?><?ifdef B?>If<?endif?><?endif?></root>",
            "<root></root>",
            Description = "<?ifdef?> inside <?ifdef?> should not be satisfied if it is not satisfied and outer condition satisfied.")]
        [Row("<root><?ifdef A?><?ifndef B?>If<?endif?><?endif?></root>",
            "<root></root>",
            Description = "<?ifndef?> inside <?ifdef?> should not be satisfied if outer condition not satisfied.")]
        [Row("<root><?define A?><?ifdef A?><?ifndef B?>If<?endif?><?endif?></root>",
            "<root>If</root>",
            Description = "<?ifndef?> inside <?ifdef?> should be satisfied if it is satisfied and outer condition satisfied.")]
        [Row("<root><?define A?><?define B?><?ifdef A?><?ifndef B?>If<?endif?><?endif?></root>",
            "<root></root>",
            Description = "<?ifndef?> inside <?ifdef?> should not be satisfied if it is not satisfied and outer condition satisfied.")]
        [Row("<root><?define B?><?ifdef A?><?ifdef B?>If<?else?>Else<?endif?><?endif?></root>",
            "<root></root>",
            Description = "<?else?> inside <?ifdef?> inside <?ifdef?> should not be satisfied if outer condition not satisfied.")]
        [Row("<root><?define A?><?define B?><?ifdef A?><?ifdef B?>If<?else?>Else<?endif?><?endif?></root>",
            "<root>If</root>",
            Description = "<?else?> inside <?ifdef?> inside <?ifdef?> should not be satisfied if inner condition satisfied and outer condition satisfied.")]
        [Row("<root><?define A?><?ifdef A?><?ifdef B?>If<?else?>Else<?endif?><?endif?></root>",
            "<root>Else</root>",
            Description = "<?else?> inside <?ifdef?> inside <?ifdef?> should be satisfied if inner condition not satisfied and outer condition satisfied.")]
        [Row("<root><?define A?>Abc<?ifdef A?>Def<?ifdef B?>Ghi<?else?>Jkl<?endif?>Mno<?endif?>Pqr</root>",
            "<root>AbcDefJklMnoPqr</root>",
            Description = "After exiting block, restores previous condition.")]
        [Row("<root><?define A?><?define B?>Abc<?ifdef A?>Def<?ifdef B?>Ghi<?else?>Jkl<?endif?>Mno<?endif?>Pqr</root>",
            "<root>AbcDefGhiMnoPqr</root>",
            Description = "After exiting block, restores previous condition.")]
        [Row("<root>Abc<?ifdef A?>Def<?ifdef B?>Ghi<?else?>Jkl<?endif?>Mno<?endif?>Pqr</root>",
            "<root>AbcPqr</root>",
            Description = "After exiting block, restores previous condition.")]
        public void Preprocess_WhenBlocksAreNested_RespectsStackOrder(string input, string expectedOutput)
        {
            var preprocessor = new XmlPreprocessor();

            string output = PreprocessString(preprocessor, input);

            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void Preprocess_WhenEndIfMissing_Throws()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?ifdef Foo?></root>";

            var ex = Assert.Throws<InvalidOperationException>(() => PreprocessString(preprocessor, input));
            Assert.AreEqual("Missing <?endif?> instruction.", ex.Message);
        }

        [Test]
        public void Preprocess_WhenElseOutsideOfBlock_Throws()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?else?></root>";

            var ex = Assert.Throws<InvalidOperationException>(() => PreprocessString(preprocessor, input));
            Assert.AreEqual("Found <?else?> instruction without enclosing <?ifdef?> or <?ifndef?> block.", ex.Message);
        }

        [Test]
        public void Preprocess_WhenEndIfExcess_Throws()
        {
            var preprocessor = new XmlPreprocessor();
            string input = "<root><?endif?></root>";

            var ex = Assert.Throws<InvalidOperationException>(() => PreprocessString(preprocessor, input));
            Assert.AreEqual("Found <?endif?> instruction without matching <?ifdef?> or <?ifndef?>.", ex.Message);
        }

        private static string PreprocessString(XmlPreprocessor preprocessor, string xml)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                var xmlOutput = new StringBuilder();
                using (var writer = XmlWriter.Create(xmlOutput, new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true
                }))
                {
                    preprocessor.Preprocess(reader, writer);
                }

                return xmlOutput.ToString();
            }
        }
    }
}