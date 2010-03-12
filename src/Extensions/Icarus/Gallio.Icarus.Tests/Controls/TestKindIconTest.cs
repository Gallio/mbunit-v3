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

using System.Drawing;
using Aga.Controls.Tree;
using Gallio.Common;
using Gallio.Icarus.Controls;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    public class TestKindIconTest
    {
        [Test]
        public void If_value_is_null_image_should_be_null()
        {
            var testKindIcon = new TestTestKindIcon(tna => null, s => null);

            var image = testKindIcon.GetIcon();

            Assert.IsNull(image);
        }

        [Test]
        public void If_value_is_string_it_should_be_used_to_select_the_image()
        {
            const string nodetype = "nodeType";
            var testKindIcon = new TestTestKindIcon(tna => nodetype, s =>
            {
                Assert.AreEqual(nodetype, s);
                return null;
            });

            var image = testKindIcon.GetIcon();

            Assert.IsNull(image);
        }

        [Test]
        public void Image_from_getNodeTypeImage_should_be_returned()
        {
            var bitmap = new Bitmap(1, 1);
            var testKindIcon = new TestTestKindIcon(tna => "", s => bitmap);

            var image = testKindIcon.GetIcon();

            Assert.AreEqual(bitmap, image);
        }

        private class TestTestKindIcon : TestKindIcon
        {
            private readonly Func<TreeNodeAdv, object> getValue;
            private readonly Func<string, Image> getNodeTypeImage;

            public TestTestKindIcon(Func<TreeNodeAdv, object> getValue, 
                Func<string, Image> getNodeTypeImage) {
                this.getValue = getValue;
                this.getNodeTypeImage = getNodeTypeImage;
            }

            public override object GetValue(TreeNodeAdv node)
            {
                return getValue(node);
            }

            protected override Image GetNodeTypeImage(string nodeType)
            {
                return getNodeTypeImage(nodeType);
            }

            public Image GetIcon()
            {
                return GetIcon(new TreeNodeAdv(null));
            }
        }
    }
}
