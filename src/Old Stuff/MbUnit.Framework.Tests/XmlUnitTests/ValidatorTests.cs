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
            //Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(
            //        "MbUnit.Tests.XmlUnit.etc." + file);
            Stream s = File.Open("../../XmlUnitTests/etc/" + file, FileMode.Open);
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
