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
using System.Collections.Generic;
using System.Linq;
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
    [TestsOn(typeof(XmlDataObjectAttribute))]
    [RunSample(typeof(XmlDataObjectWithTestData1File))]
    [RunSample(typeof(XmlDataObjectWithTestData3File))]
    public class XmlDataObjectTests : BaseTestWithSampleRunner
    {
        [Test]
        public void VerifyTestOnTestData1File()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                    CodeReference.CreateFromMember(typeof(XmlDataObjectWithTestData1File).GetMethod("Test")));

            Assert.AreElementsEqual(new[] {
                     "ID = 'Admin', UserName = 'Admin', Password = 'Password'",
                     "ID = 'User', UserName = 'User123', Password = 'Password'" },
              run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()),
             (x, y) => y.Contains(x));
        }

        [Test]
        public void VerifyTestOnTestData3File()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                    CodeReference.CreateFromMember(typeof(XmlDataObjectWithTestData3File).GetMethod("Test")));

            Assert.AreElementsEqual(new[] {
                     "CustomerName = 'John Jacobs', CustomerCity = 'Chicago', CustomerState = 'IL', CustomerZip = '60641'",
                     "CustomerName = 'Bill Bailey', CustomerCity = 'Chicago', CustomerState = 'IL', CustomerZip = '60652'",
                     "CustomerName = 'Erin Everest', CustomerCity = 'Alameda', CustomerState = 'CA', CustomerZip = '94501'"
              },
              run.Children.Select(x => x.TestLog.GetStream(MarkupStreamNames.Default).ToString()),
             (x, y) => y.Contains(x));
        }

        [TestFixture, Explicit]
        internal class XmlDataObjectWithTestData1File
        {
            [Test, XmlDataObject("//LoginCredentials/Credentials", ResourcePath = "MbUnit40.Tests.Framework.TestData1.xml")]
            public void Test(dynamic credentials)
            {
                TestLog.WriteLine("ID = '{0}', UserName = '{1}', Password = '{2}'",
                                    credentials.ID, credentials.UserName.Value, credentials.Password.Value);
            }
        }

        [TestFixture, Explicit]
        internal class XmlDataObjectWithTestData3File
        {
            [Test, XmlDataObject("//TestData/TestCase/TestStep", FilePath = @"..\Framework\TestData3.xml")]
            public void Test(dynamic teststep)
            {
                TestLog.WriteLine("CustomerName = '{0}', CustomerCity = '{1}', CustomerState = '{2}', CustomerZip = '{3}'",
                            teststep.Customer.Name.Value, teststep.Customer.City.Value,
                            teststep.Customer.State.Value, teststep.Customer.Zip.Value);
            }
        }


        #region Sample Code
        // These aren't really tests as much as example of how to access data using the dynamic XmlDataObject attribute.
        // Notice how you can specify specific XPaths into your XDocument.  The FilePath specifies where the Xml file 
        // is located relative to the binary
        [Test]
        [XmlDataObject("//TestData/TestCase/TestStep[@ID='2']", FilePath=@"..\Framework\TestData3.xml")]
        public void UsingXmlDataSample(dynamic TestStep)
        {
            // Prints the entire object graph to the test log
            TestLog.WriteLine(((XmlDataObject)TestStep).ToString(-1));

            // Here's how to access values stored in a attribute - just call it by name
            TestLog.WriteLine("TestStep.ID = " + TestStep.ID);
            TestLog.WriteLine("TestStep.Description = " + TestStep.Description);

            Assert.AreEqual<string>("2", TestStep.ID);
            Assert.AreEqual<string>("1st Smoke Test", TestStep.Description);
            
            // Here's how to access the inner text of an Element - use ".Value" dynamic property
            TestLog.WriteLine("TestStep.Customer.Name.Value = " + TestStep.Customer.Name.Value);
            TestLog.WriteLine("TestStep.Customer.Zip.Value = " + TestStep.Customer.Zip.Value);

            // Do some verification with the data
            Assert.AreEqual<string>("John Jacobs", TestStep.Customer.Name.Value);
            Assert.AreEqual<string>("Chicago", TestStep.Customer.City.Value);
            Assert.AreEqual<string>("IL", TestStep.Customer.State.Value);
            Assert.AreEqual<string>("60641", TestStep.Customer.Zip.Value);


            // Here's how to check to see if a certain Element or Attribute exists or not
            Assert.IsNotNull(TestStep.Description);
            Assert.IsNull(TestStep.NameOfElementWhichDoesntExist);

            // When there are multiple Elements with the same name, the dynamic property returns List<XmlDataObject>
            List<XmlDataObject> ProductList = TestStep.Orders.Product;

            TestLog.WriteLine("TestStep.Orders.Product[0].ID = " + TestStep.Orders.Product[0].ID);
            TestLog.WriteLine("TestStep.Orders.Product[1].ID = " + TestStep.Orders.Product[1].ID);
            TestLog.WriteLine("TestStep.Orders.Product[2].ID = " + TestStep.Orders.Product[2].ID);

            // Add it up!
            int QuantityTotal = 0;
            foreach (dynamic Product in ProductList)
            {
                QuantityTotal += Int32.Parse(Product.Quantity);
            }
            Assert.AreEqual<int>(6, QuantityTotal);

            // Better yet, use a LINQ extension method!
            Assert.AreEqual<int>(6, ProductList.Sum<dynamic>(Product => Int32.Parse(Product.Quantity)));

            // The XmlDataObject.IsOne() static method detects if there's only one Element returns by a dynamic accessor
            Assert.IsTrue(XmlDataObject.IsOne(TestStep.Customer));

            // Type checking accomplishes the same thing...
            Assert.IsInstanceOfType(typeof(XmlDataObject), TestStep.Customer);

            // Multiple Elements can be detected using the XmlDataObject.IsMany() static method
            Assert.IsTrue(XmlDataObject.IsMany(TestStep.Orders.Product));

            // Type checking shows a List<XmlDataObject> for Elements with multiple children of the same name
            Assert.IsInstanceOfType(typeof(List<XmlDataObject>), TestStep.Orders.Product);
        }

        // This example illustrates how to handle multiple Elements with the same name with the 
        // XmlDataObject.AsList() static method
        [Test]
        [XmlDataObject("//TestData/TestCase/TestStep/Orders", FilePath = @"..\Framework\TestData3.xml")]
        public void HandlingMultipleSameNameElementsPolymorphicallySample(dynamic Order)
        {
            // Whether there is one or many Product Element, this enables us to treat them as Lists
            var ProductList = XmlDataObject.AsList(Order.Product);

            int QuantityTotal = 0;
            foreach (dynamic Product in ProductList)
            {
                QuantityTotal += Int32.Parse(Product.Quantity);
            }

            // All the test data in TestData3.xml has Product Quantities totalling to "6"
            Assert.AreEqual<int>(6, QuantityTotal);
        }

        /// <summary>
        /// This test demonstrates the usage of a ResourceFile 
        /// </summary>
        [Test]
        [XmlDataObject("//TestCase[@ID='563']/TestStep[@ID='7']", ResourcePath = "MbUnit40.Tests.Framework.TestData3.xml")]
        public void ResourceFileSample(dynamic TestStep)
        {
            // Prints the entire object graph to the test log
            TestLog.WriteLine(((XmlDataObject)TestStep).ToString(-1));

            // All of the accessor attempts should succeed with the test data
            TestLog.WriteLine("TestStep.Customer.Name.Value = " + TestStep.Customer.Name.Value);
            TestLog.WriteLine("TestStep.Customer.City.Value = " + TestStep.Customer.City.Value);
            TestLog.WriteLine("TestStep.Customer.State.Value = " + TestStep.Customer.State.Value);
            TestLog.WriteLine("TestStep.Customer.Zip.Value = " + TestStep.Customer.Zip.Value);

            // Verification
            Assert.AreEqual<string>("Erin Everest", TestStep.Customer.Name.Value);
            Assert.AreEqual<string>("Alameda", TestStep.Customer.City.Value);
            Assert.AreEqual<string>("CA", TestStep.Customer.State.Value);
            Assert.AreEqual<string>("94501", TestStep.Customer.Zip.Value);
        }
        #endregion
    }
}

