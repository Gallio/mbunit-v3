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

using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using TestDriven.UnitTesting.Exceptions;

    [TestFixture]
    public class StringAssert_Test
    {
        #region AreEqual
        [Test]
        public void AreEqualIgnoreCase()
        {
            StringAssert.AreEqualIgnoreCase("hello", "HELLO");
        }

        #endregion

        #region Contains
        [Test]
        public void DoesNotContain()
        {
            StringAssert.DoesNotContain("hello", 'k');
        }

         #endregion

        #region Emptiness
        [Test]
        public void IsEmpty()
        {
            StringAssert.IsEmpty("");
        }

        [Test]
        public void IsNotEmpty()
        {
            StringAssert.IsNonEmpty(" ");
        }
           
        #endregion
    }

