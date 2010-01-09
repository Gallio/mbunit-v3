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
using System.Drawing;
using System.Linq;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Common.Media;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(CaptureParameters))]
    public class CaptureParametersTest
    {
        [Test]
        public void Zoom_CanGetAndSetValue()
        {
            var parameters = new CaptureParameters();

            Assert.AreEqual(1.0, parameters.Zoom);

            parameters.Zoom = 1.0 / 16;
            Assert.AreEqual(1.0 / 16, parameters.Zoom);

            parameters.Zoom = 16;
            Assert.AreEqual(16, parameters.Zoom);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Zoom = 1.0 / 17);
            Assert.Contains(ex.Message, "The zoom factor must be between 1/16 and 16.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Zoom = 17);
            Assert.Contains(ex.Message, "The zoom factor must be between 1/16 and 16.");
        }
    }
}
