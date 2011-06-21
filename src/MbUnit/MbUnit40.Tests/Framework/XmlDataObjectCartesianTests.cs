using System;
using System.Dynamic;
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Framework.Data.DataObjects;
using MbUnit.Core;
using MbUnit.Framework;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    [TestsOn(typeof(XmlDataObjectCartesianAttribute))]
    public class XmlDataObjectCartesianTests
    {
        /// <summary>
        /// This example shows how to return the Cartesian product of two different XML documents.
        /// </summary>
        [Test]
        [XmlDataObjectCartesian(
            @"..\Framework\TestData1.xml", "//LoginCredentials/Credentials[@ID='Admin']",
            @"..\Framework\TestData2.xml", "//TestUrls/Url[@TestGroup='1']/.")]
        public void TwoFilesJoined(dynamic Credentials, dynamic Url)
        {
            TestLog.WriteLine("Credentials.ID = " + Credentials.ID);
            TestLog.WriteLine("Credentials.UserName.Value = " + Credentials.UserName.Value);
            TestLog.WriteLine("Credentials.Password.Value = " + Credentials.Password.Value);

            TestLog.WriteLine("Url.TestGroup = " + Url.TestGroup);
            TestLog.WriteLine("Url.Name = " + Url.Name);
            TestLog.WriteLine("Url.Value = " + Url.Value);

            // Perform verification
            Assert.IsTrue(Int32.Parse(Credentials.SecurityCode) % 3 == 0);
            Assert.Contains(Url.Name, "google.com");
        }

        /// <summary>
        /// This example shows how to return the Cartesian product of three different XML documents.
        /// </summary>
        [Test]
        [XmlDataObjectCartesian(
            @"..\Framework\TestData1.xml", "//LoginCredentials/Credentials[@ID='Admin']",
            @"..\Framework\TestData2.xml", "//TestUrls/Url[@TestGroup='1']/.",
            @"..\Framework\TestData3.xml", "//TestData/TestCase/TestStep")]
        public void ThreeFilesJoined(dynamic Credentials, dynamic Url, dynamic Customer)
        {
            TestLog.WriteLine("Credentials.ID = " + Credentials.ID);
            TestLog.WriteLine("Credentials.UserName.Value = " + Credentials.UserName.Value);
            TestLog.WriteLine("Credentials.Password.Value = " + Credentials.Password.Value);

            TestLog.WriteLine("Url.TestGroup = " + Url.TestGroup);
            TestLog.WriteLine("Url.Name = " + Url.Name);
            TestLog.WriteLine("Url.Value = " + Url.Value);

            TestLog.WriteLine("Customer.Name.Value = " + Customer.Name.Value);
            TestLog.WriteLine("Customer.City.Value = " + Customer.City.Value);
            TestLog.WriteLine("Customer.State.Value = " + Customer.State.Value);
            TestLog.WriteLine("Customer.Zip.Value = " + Customer.Zip.Value);

            Assert.IsTrue(Int32.Parse(Credentials.SecurityCode) % 3 == 0);
            Assert.Contains(Url.Name, "google.com");

            // Verification of the Customer ID's which should be between 1001 and 1003 in the Test Data
            int CustomerID = Int32.Parse(Customer.ID);
            Assert.IsTrue(CustomerID >= 1001 && CustomerID <= 1003);
        }

        /// <summary>
        /// This example shows how to return the Cartesian product of four different XML documents.
        /// </summary>
        [Test]
        [XmlDataObjectCartesian(
            @"..\Framework\TestData1.xml", "//LoginCredentials/Credentials[@ID='Admin']",
            @"..\Framework\TestData2.xml", "//TestUrls/Url[@TestGroup='1']/.",
            @"..\Framework\TestData3.xml", "//TestData/TestCase/TestStep/Customer",
            @"..\Framework\TestData4.xml", "//FourthSet/Test")]
        public void FourFilesJoined(dynamic Credentials, dynamic Url, dynamic Customer, dynamic Test4)
        {
            TestLog.WriteLine("Credentials.ID = " + Credentials.ID);
            TestLog.WriteLine("Credentials.UserName.Value = " + Credentials.UserName.Value);
            TestLog.WriteLine("Credentials.Password.Value = " + Credentials.Password.Value);

            TestLog.WriteLine("Url.TestGroup = " + Url.TestGroup);
            TestLog.WriteLine("Url.Name = " + Url.Name);
            TestLog.WriteLine("Url.Value = " + Url.Value);

            TestLog.WriteLine("Customer.Name.Value = " + Customer.Name.Value);
            TestLog.WriteLine("Customer.City.Value = " + Customer.City.Value);
            TestLog.WriteLine("Customer.State.Value = " + Customer.State.Value);
            TestLog.WriteLine("Customer.Zip.Value = " + Customer.Zip.Value);

            TestLog.WriteLine("Test4.Data.Value = " + Test4.Data.Value);
            TestLog.WriteLine("Test4.Name.Value = " + Test4.Name.Value);

            Assert.IsTrue(Int32.Parse(Credentials.SecurityCode) % 3 == 0);
            Assert.Contains(Url.Name, "google.com");

            // Verification of the Customer ID's which should be between 1001 and 1003 in the Test Data
            int CustomerID = Int32.Parse(Customer.ID);
            Assert.IsTrue(CustomerID >= 1001 && CustomerID <= 1003);

            int DataNumber = Int32.Parse(Test4.Data.Value);
            Assert.IsTrue(DataNumber >= 100 && DataNumber <= 999);
        }
        
    }
}
