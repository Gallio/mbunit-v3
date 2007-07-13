// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace MbUnit.Tests.XmlUnit {
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
	using System;
    using System.Xml;
    
    [TestFixture]
    public class DiffResultTests {
        private DiffResult _result;
    	private XmlDiff _diff;
        private Difference _majorDifference, _minorDifference;
        
        [SetUp] public void CreateDiffResult() {
            _result = new DiffResult();
        	_diff = new XmlDiff("<a/>", "<b/>");
            _majorDifference = new Difference(DifferenceType.ElementTagName, XmlNodeType.Element, XmlNodeType.Element);
            _minorDifference = new Difference(DifferenceType.AttributeSequence, XmlNodeType.Comment, XmlNodeType.Comment);
        }
        
        [Test] public void NewDiffResultIsEqualAndIdentical() {
            Assert.AreEqual(true, _result.Identical);
            Assert.AreEqual(true, _result.Equal);
        	Assert.AreEqual("Identical", _result.StringValue);
        }
        
        [Test] public void NotEqualOrIdenticalAfterMajorDifferenceFound() {
            _result.DifferenceFound(_diff, _majorDifference);
            Assert.AreEqual(false, _result.Identical);
            Assert.AreEqual(false, _result.Equal);
        	Assert.AreEqual(_diff.OptionalDescription
        	                       + Environment.NewLine
        	                       + _majorDifference.ToString(), _result.StringValue);
        }
        
        [Test] public void NotIdenticalButEqualAfterMinorDifferenceFound() {
            _result.DifferenceFound(_diff, _minorDifference);
            Assert.AreEqual(false, _result.Identical);
            Assert.AreEqual(true, _result.Equal);
        	Assert.AreEqual(_diff.OptionalDescription
        	                       + Environment.NewLine
        	                       + _minorDifference.ToString(), _result.StringValue);
        }
    }
}
