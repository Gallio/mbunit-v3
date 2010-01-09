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
using Gallio.Model.Tree;
using MbUnit.Framework;
using Test=Gallio.Model.Tree.Test;

namespace Gallio.Tests.Model.Tree
{
    [TestsOn(typeof(Test))]
    public class TestTest
    {
        [Test]
        public void AssignsUniqueLocalIdsToChildren()
        {
            var test = new Test("name", null);

            Assert.AreEqual("localId", test.GetUniqueLocalIdForChild("localId"),
                "If no conflict, assigns local id hint to child.");
            Assert.AreEqual("localId2", test.GetUniqueLocalIdForChild("localId"),
                "If conflict, assigned suffixed local id hint to child.");
            Assert.AreEqual("localId3", test.GetUniqueLocalIdForChild("localId"),
                "If conflict, assigned suffixed local id hint to child.");
            Assert.AreEqual("localId22", test.GetUniqueLocalIdForChild("localId2"),
                "If conflict, assigned suffixed local id hint to child.");

            Assert.AreEqual("differentId", test.GetUniqueLocalIdForChild("differentId"),
                "If no conflict, assigns local id hint to child.");
        }

        [Test]
        public void UsesNameAsLocalIdIfNoHintAndNoConflicts()
        {
            var test = new Test("name", null);
            Assert.AreEqual("name", test.LocalId, "Uses name when no parent available.");

            var testWithParent = new Test("name", null);
            test.AddChild(testWithParent);
            Assert.AreEqual("name", testWithParent.LocalId, "Uses name when parent available but no conflicts.");
        }

        [Test]
        public void UsesLocalIdHintAsLocalIdIfNoConflicts()
        {
            var test = new Test("name", null);
            test.LocalIdHint = "hint";
            Assert.AreEqual("hint", test.LocalId, "Uses local id hint when no parent available.");

            var testWithParent = new Test("name", null);
            test.AddChild(testWithParent);
            testWithParent.LocalIdHint = "hint";
            Assert.AreEqual("hint", testWithParent.LocalId, "Uses local id hint when parent available but no conflicts.");
        }

        [Test]
        public void UsesSuffixedLocalIdWhenConflictsOccur()
        {
            var parent = new Test("name", null);

            var test1 = new Test("test", null);
            parent.AddChild(test1);
            Assert.AreEqual("test", test1.LocalId, "Uses name because there is no conflict.");

            var test2 = new Test("test", null);
            parent.AddChild(test2);
            Assert.AreEqual("test2", test2.LocalId, "Uses suffixed name because there is a conflict in the name and no hint.");

            var test3 = new Test("name", null);
            test3.LocalIdHint = "test";
            parent.AddChild(test3);
            Assert.AreEqual("test3", test3.LocalId, "Uses suffixed name because there is a conflict in the hint.");
        }
    }
}
