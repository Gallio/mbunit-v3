namespace MbUnit.Tests.Compatibility.Framework.Xml {
	using MbUnit.Framework;	
	using MbUnit.Framework.Xml;
	using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    


    [TestFixture]
    public class DiffConfigurationTests 
	{
        private static string xmlWithWhitespace = "<elemA>as if<elemB> \r\n </elemB>\t</elemA>";
        private static string xmlWithoutWhitespaceElement = "<elemA>as if<elemB/>\r\n</elemA>";
        private static string xmlWithWhitespaceElement = "<elemA>as if<elemB> </elemB></elemA>";
        private static string xmlWithoutWhitespace = "<elemA>as if<elemB/></elemA>";
                
        [Test] public void DefaultConfiguredWithGenericDescription() {
            DiffConfiguration diffConfiguration = new DiffConfiguration();
            Assert.AreEqual(DiffConfiguration.DEFAULT_DESCRIPTION, 
                                   diffConfiguration.Description);
            
            Assert.AreEqual(DiffConfiguration.DEFAULT_DESCRIPTION, 
                                   new XmlDiff("", "").OptionalDescription);
        }
        
        //[Test] public void DefaultConfiguredToUseValidatingParser() {
        //    DiffConfiguration diffConfiguration = new DiffConfiguration();
        //    Assert.AreEqual(DiffConfiguration.DEFAULT_USE_VALIDATING_PARSER, 
        //                           diffConfiguration.UseValidatingParser);
            
        //    Stream controlFileStream = ValidatorTests.ValidFile;
        //    Stream testFileStream = ValidatorTests.InvalidFile;
        //    try {         
        //        XmlDiff diff = new XmlDiff(new StreamReader(controlFileStream), 
        //                                   new StreamReader(testFileStream));
        //        diff.Compare();
        //        Assert.Fail("Expected validation failure");
        //    } catch (XmlSchemaException e) {
        //        string message = e.Message; // to prevent 'unused variable' compiler warning 
        //    } finally {
        //        controlFileStream.Close();
        //        testFileStream.Close();
        //    }
        //}
                
        //[Test] public void CanConfigureNotToUseValidatingParser() {
        //    DiffConfiguration diffConfiguration = new DiffConfiguration(false);
        //    Assert.AreEqual(false, diffConfiguration.UseValidatingParser);
            
        //    Stream controlFileStream = ValidatorTests.ValidFile;
        //    Stream testFileStream = ValidatorTests.InvalidFile;
        //    try {         
        //        XmlDiff diff = new XmlDiff(new XmlInput(controlFileStream), 
        //                                   new XmlInput(testFileStream),
        //                                   diffConfiguration);
        //        diff.Compare();
        //    } catch (XmlSchemaException e) {
        //        Assert.Fail("Unexpected validation failure: " + e.Message);
        //    } finally {
        //        controlFileStream.Close();
        //        testFileStream.Close();
        //    }
        //}
        
        //[Test] public void DefaultConfiguredWithWhitespaceHandlingAll() {
        //    DiffConfiguration diffConfiguration = new DiffConfiguration();
        //    Assert.AreEqual(WhitespaceHandling.All, diffConfiguration.WhitespaceHandling);
            
        //    PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespaceElement, false);
        //    PerformAssertion(xmlWithoutWhitespace, xmlWithoutWhitespaceElement, false);
        //    PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespace, false);
        //    PerformAssertion(xmlWithoutWhitespaceElement, xmlWithWhitespaceElement, false);
        //}
        
        private void PerformAssertion(string control, string test, bool assertion) {
            XmlDiff diff = new XmlDiff(control, test);
            PerformAssertion(diff, assertion);
        }
        private void PerformAssertion(string control, string test, bool assertion, 
                                      DiffConfiguration xmlUnitConfiguration) {
            XmlDiff diff = new XmlDiff(new XmlInput(control), new XmlInput(test), 
                                       xmlUnitConfiguration);
            PerformAssertion(diff, assertion);
        }        
        private void PerformAssertion(XmlDiff diff, bool assertion) {
            Assert.AreEqual(assertion, diff.Compare().Equal);            
            Assert.AreEqual(assertion, diff.Compare().Identical);            
        }

        [Test] public void CanConfigureWhitespaceHandlingSignificant() {
            DiffConfiguration xmlUnitConfiguration = 
                new DiffConfiguration (WhitespaceHandling.Significant);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithoutWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespace, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespaceElement, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
        }
        
        [Test] public void CanConfigureWhitespaceHandlingNone() {
            DiffConfiguration xmlUnitConfiguration = 
                new DiffConfiguration(WhitespaceHandling.None);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithoutWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespace, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespaceElement, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
        }        
    }
}
