using System;
using System.Dynamic;
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Framework.Data.DataObjects;
using MbUnit.Core;
using MbUnit.Framework;

namespace MbUnit.Tests
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
        }

        /// <summary>
        /// This example shows how to return the Cartesian product of three different XML documents.
        /// </summary>
        [Test]
        [XmlDataObjectCartesian(
            @"..\Framework\TestData1.xml", "//LoginCredentials/Credentials[@ID='Admin']",
            @"..\Framework\TestData2.xml", "//TestUrls/Url[@TestGroup='1']/.",
            @"..\Framework\TestData3.xml", "//TestData/TestCase/TestStep/Customer")]
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
        public void FourFilesJoined(dynamic Credentials, dynamic Url, dynamic Customer, dynamic Test)
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

            TestLog.WriteLine("Test.Data.Value = " + Test.Data.Value);
            TestLog.WriteLine("Test.Name.Value = " + Test.Name.Value);
        }
        
    }
}
