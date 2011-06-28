// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
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
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Common.Markup;
using Gallio.Framework;
using Gallio.Framework.Data.DataObjects;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Core;
using MbUnit.Framework;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    [RunSample(typeof(TwoFileCartesianSample))]
    [RunSample(typeof(ThreeFileCartesianSample))]
    [RunSample(typeof(FourFileCartesianSample))]
    [TestsOn(typeof(XmlDataObjectCartesianAttribute))]
    public class XmlDataObjectCartesianTests : BaseTestWithSampleRunner
    {
        [Test]
        public void VerifyTwoFileCartesianSample()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
            CodeReference.CreateFromMember(typeof(TwoFileCartesianSample).GetMethod("Test")));

            Assert.AreElementsEqual(new[] {
                     "ID = 'Admin', UserName = 'Admin', Password = 'Password', Url = 'http://news.google.com/nwshp'",
                     "ID = 'Admin', UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/imghp'",
                     "ID = 'Admin', UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/accounts/ManageAccount'",
                     "ID = 'User', UserName = 'User123', Password = 'Password', Url = 'http://news.google.com/nwshp'",
                     "ID = 'User', UserName = 'User123', Password = 'Password', Url = 'http://www.google.com/imghp'",
                     "ID = 'User', UserName = 'User123', Password = 'Password', Url = 'http://www.google.com/accounts/ManageAccount'"
            },
            run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()),
           (x, y) => y.Contains(x));
        }

        [Test]
        public void VerifyThreeFileCartesianSample()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
            CodeReference.CreateFromMember(typeof(ThreeFileCartesianSample).GetMethod("Test")));

            Assert.AreElementsEqual(new[] {
                     "UserName = 'Admin', Password = 'Password', Url = 'http://news.google.com/nwshp', CustomerName = 'John Jacobs'",
                     "UserName = 'Admin', Password = 'Password', Url = 'http://news.google.com/nwshp', CustomerName = 'Bill Bailey'",
                     "UserName = 'Admin', Password = 'Password', Url = 'http://news.google.com/nwshp', CustomerName = 'Erin Everest'",

                     "UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/imghp', CustomerName = 'John Jacobs'",
                     "UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/imghp', CustomerName = 'Bill Bailey'",
                     "UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/imghp', CustomerName = 'Erin Everest'",

                     "UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'John Jacobs'",
                     "UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'Bill Bailey'",
                     "UserName = 'Admin', Password = 'Password', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'Erin Everest'",
            },
            run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()),
           (x, y) => y.Contains(x));
        }

        [Test]
        public void VerifyFourFileCartesianSample()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
            CodeReference.CreateFromMember(typeof(FourFileCartesianSample).GetMethod("Test")));

            Assert.AreElementsEqual(new[] {
                     "UserName = 'Admin', Url = 'http://news.google.com/nwshp', CustomerName = 'John Jacobs', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://news.google.com/nwshp', CustomerName = 'John Jacobs', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://news.google.com/nwshp', CustomerName = 'Bill Bailey', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://news.google.com/nwshp', CustomerName = 'Bill Bailey', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://news.google.com/nwshp', CustomerName = 'Erin Everest', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://news.google.com/nwshp', CustomerName = 'Erin Everest', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://www.google.com/imghp', CustomerName = 'John Jacobs', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://www.google.com/imghp', CustomerName = 'John Jacobs', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://www.google.com/imghp', CustomerName = 'Bill Bailey', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://www.google.com/imghp', CustomerName = 'Bill Bailey', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://www.google.com/imghp', CustomerName = 'Erin Everest', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://www.google.com/imghp', CustomerName = 'Erin Everest', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'John Jacobs', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'John Jacobs', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'Bill Bailey', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'Bill Bailey', FourthName = 'Knight'",

                     "UserName = 'Admin', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'Erin Everest', FourthName = 'Archer'",
                     "UserName = 'Admin', Url = 'http://www.google.com/accounts/ManageAccount', CustomerName = 'Erin Everest', FourthName = 'Knight'",
            },
            run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()),
           (x, y) => y.Contains(x));
        }


        [TestFixture, Explicit]
        internal class TwoFileCartesianSample
        {
            [Test]
            [XmlDataObjectCartesian(
                @"..\Framework\TestData1.xml", "//LoginCredentials/Credentials",
                @"..\Framework\TestData2.xml", "//TestUrls/Url[@TestGroup='1']/.")]
            public void Test(dynamic credentials, dynamic url)
            {
                TestLog.WriteLine("ID = '{0}', UserName = '{1}', Password = '{2}', Url = '{3}'",
                                    credentials.ID, credentials.UserName.Value, credentials.Password.Value, url.Value);
            }
        }

        [TestFixture, Explicit]
        internal class ThreeFileCartesianSample
        {
            [Test]
            [XmlDataObjectCartesian(
                @"..\Framework\TestData1.xml", "//LoginCredentials/Credentials[@ID='Admin']",
                @"..\Framework\TestData2.xml", "//TestUrls/Url[@TestGroup='1']",
                @"..\Framework\TestData3.xml", "//TestData/TestCase/TestStep")]
            public void Test(dynamic credentials, dynamic url, dynamic teststep)
            {
                TestLog.WriteLine("UserName = '{0}', Password = '{1}', Url = '{2}', CustomerName = '{3}'",
                                    credentials.UserName.Value, credentials.Password.Value, url.Value, teststep.Customer.Name.Value);
            }
        }

        [TestFixture, Explicit]
        internal class FourFileCartesianSample
        {
            [Test]
            [XmlDataObjectCartesian(
                @"..\Framework\TestData1.xml", "//LoginCredentials/Credentials[@ID='Admin']",
                @"..\Framework\TestData2.xml", "//TestUrls/Url[@TestGroup='1']/.",
                @"..\Framework\TestData3.xml", "//TestData/TestCase/TestStep",
                @"..\Framework\TestData4.xml", "//FourthSet/Test[@ID='England' or @ID='France']")]
            public void Test(dynamic credentials, dynamic url, dynamic teststep, dynamic fourth)
            {
                TestLog.WriteLine("UserName = '{0}', Url = '{1}', CustomerName = '{2}', FourthName = '{3}'",
                                    credentials.UserName.Value, url.Value, teststep.Customer.Name.Value, fourth.Name.Value);
            }
        }
    }
}
