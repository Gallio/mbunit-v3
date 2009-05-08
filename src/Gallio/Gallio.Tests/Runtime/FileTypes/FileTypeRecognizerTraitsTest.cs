// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(FileTypeRecognizerTraits))]
    public class FileTypeRecognizerTraitsTest
    {
        [Test]
        public void Constructor_WhenIdIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileTypeRecognizerTraits(null, "description"));
        }

        [Test]
        public void Constructor_WhenIdIsEmpty_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FileTypeRecognizerTraits("", "description"));
            Assert.Contains(ex.Message, "The file type id must not be empty.");
        }

        [Test]
        public void Constructor_WhenDescriptionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FileTypeRecognizerTraits("id", null));
        }

        [Test]
        public void Constructor_WhenArgumentsValid_SetsProperties()
        {
            var traits = new FileTypeRecognizerTraits("id", "description");

            Assert.AreEqual("id", traits.Id);
            Assert.AreEqual("description", traits.Description);
        }
    }
}
