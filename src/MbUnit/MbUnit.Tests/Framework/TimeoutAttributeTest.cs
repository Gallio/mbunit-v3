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
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    public class TimeoutAttributeTest
    {
        [Test]
        public void ZeroTimeoutMeansInfinite()
        {
            var attrib = new TimeoutAttribute(0);
            Assert.AreEqual(0, attrib.TimeoutSeconds);
            Assert.IsNull(attrib.Timeout);
        }

        [Test]
        public void DisallowsNegativeTimeout()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeoutAttribute(-1));
        }

        [Test]
        public void TimeoutIsEvaluatedInSeconds()
        {
            var attrib = new TimeoutAttribute(120);
            Assert.AreEqual(120, attrib.TimeoutSeconds);
            Assert.AreEqual(TimeSpan.FromSeconds(120), attrib.Timeout);
        }
    }
}
