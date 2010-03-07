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

#pragma warning disable 618

namespace MbUnit.Compatibility.Tests.Framework.Xml {
    using System;
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
    
    [TestFixture]
    public class DifferenceTests {
        private Difference minorDifference;
        
        [SetUp] public void CreateMinorDifference() {
            DifferenceType id = DifferenceType.AttributeSequence;
            OldAssert.AreEqual(false, Differences.IsMajorDifference(id));
            minorDifference = new Difference(id);
        }
        
        [Test] public void ToStringContainsId() {
            string commentDifference = minorDifference.ToString();
            string idValue = "type: " + (int)DifferenceType.AttributeSequence;
            OldAssert.AreEqual( 
                            true, 
                            commentDifference.IndexOfAny(idValue.ToCharArray()) > 0,
							"contains " + idValue);
        }
    }
}
