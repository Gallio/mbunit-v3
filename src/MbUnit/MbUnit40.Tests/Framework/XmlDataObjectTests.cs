using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using Gallio.Framework;
using Gallio.Framework.Data.DataObjects;
using MbUnit.Core;
using MbUnit.Framework;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    [TestsOn(typeof(XmlDataObjectAttribute))]
    public class XmlDataObjectTests
    {
        // START HERE: this prime example shows how to access data using the dynamic XmlDataObject attribute.
        // Notice how you can specify specific XPaths into your XDocument.  The FilePath specifies where the Xml file 
        // is located relative to the binary
        [Test]
        [XmlDataObject("//TestData/TestCase/TestStep[@ID='2']", FilePath=@"..\Framework\TestData3.xml")]
        public void TestUsingXmlData(dynamic TestStep)
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
        public void HandlingMultipleSameNameElementsPolymorphically(dynamic Order)
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
        public void ResourceFileTest(dynamic TestStep)
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

        /// <summary>
        /// This is useful utility for finding the way that Resource Files are represented in the Assembly manifest
        /// </summary>
        [Test]
        public void ResourcesFile()
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            string[] resources = thisExe.GetManifestResourceNames();
            string list = "";

            // Build the string of resources.
            foreach (string resource in resources)
            {
                list += resource + "\r\n";
                TestLog.WriteLine(resource);
            }
        }
    }
}

