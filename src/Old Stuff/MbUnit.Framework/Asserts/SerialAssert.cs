// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace MbUnit.Framework
{
	public sealed class SerialAssert
	{
		#region Hiding constructor
		private SerialAssert()
		{}
		#endregion
		
		/// <summary>
		/// Verifies that the type is serializable with the XmlSerializer object.
		/// </summary>
		/// <param name="t">
		/// type to test.
		/// </param>
		public static void IsXmlSerializable(Type t)
		{
			Assert.IsNotNull(t);
			XmlSerializer ser=new XmlSerializer(t);
		}
		
		/// <summary>
		/// Serializes and deserialies to/from XML and checks that the results are the same.
		/// </summary>
		/// <param name="o">
		/// Object to test
		/// </param>
		public static void TwoWaySerialization(Object o)
		{
			Assert.IsNotNull(o);
			XmlSerializer ser =new XmlSerializer(o.GetType());
			
			// create xml writer
			StringWriter sw = new StringWriter();
			XmlTextWriter xsw = new XmlTextWriter(sw);
			xsw.Formatting = Formatting.Indented;
			ser.Serialize(xsw,o);
			xsw.Flush();
			xsw.Close();
			
			// deserialize
			StringReader sr = new StringReader(sw.ToString());
			XmlTextReader xsr = new XmlTextReader(sw.ToString());
			Object oReturn = ser.Deserialize(xsr);	
			
			// compare both
			Assert.AreEqual(o,oReturn);
		}

		public static string OneWaySerialization(Object o)
		{
			Assert.IsNotNull(o);
			XmlSerializer ser =new XmlSerializer(o.GetType());
			
			// create xml writer
			StringWriter sw = new StringWriter();
			XmlTextWriter xsw = new XmlTextWriter(sw);
			xsw.Formatting = Formatting.Indented;
			ser.Serialize(xsw,o);
			xsw.Flush();
			xsw.Close();

			// try parsing
			StringReader sr = new StringReader(sw.ToString());
			XmlTextReader wr = new XmlTextReader(sr);
			XmlDocument doc = new XmlDocument();
			doc.Load(wr);

			return sw.ToString();
		}
	}
}
