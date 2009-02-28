namespace MbUnit.Tests.Compatibility.Framework.Xml {
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
	using System.Reflection;
    using MbUnit.Framework;
    using MbUnit.Framework.Xml;
    
    [TestFixture]
    public class ValidatorTests {    

		public static Stream GetTestFile(string file)
		{
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "MbUnit.Framework.Tests.Asserts.XmlUnitTests.etc." + file);
			Assert.IsNotNull(s);
			return s;
		}

		public static StreamReader GetTestReader(string file)
		{
			return new StreamReader(GetTestFile(file));
		}

		public static Stream ValidFile
		{
			get
			{
				return GetTestFile("BookXsdGenerated.xml");
			}
		}
		public static Stream InvalidFile
		{
			get
			{
				return GetTestFile("invalidBook.xml");
			}
		}

                
        private Validator PerformAssertion(Stream input, bool expected) 
		{
			using(input)
			{
                Validator validator = new Validator(new XmlInput(new StreamReader(input)));
                Assert.AreEqual(expected, validator.IsValid, validator.ValidationMessage);
                return validator;
            }
        }
        
        //[Test] 
        //public void XsdValidFileIsValid() 
        //{
        //    // output validfile
        //    Console.Out.WriteLine( new StreamReader(ValidFile).ReadToEnd() );
        //    PerformAssertion(ValidFile, true);
        //}
 
        //[Test] 
        //public void XsdInvalidFileIsNotValid() {
        //    Console.Out.WriteLine( new StreamReader(InvalidFile).ReadToEnd() );
        //    Validator validator = PerformAssertion(InvalidFile, false);
        //    string expected = "The element 'http://www.publishing.org:Book' has incomplete content";
        //    Assert.AreEqual(true,validator.ValidationMessage.StartsWith(expected));
        //}
    }
}
