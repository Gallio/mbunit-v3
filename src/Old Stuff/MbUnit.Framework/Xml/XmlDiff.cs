namespace MbUnit.Framework.Xml 
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// <para>
    /// The <b>MbUnit.Framework.Xml</b> contains Xml-specific assertion.
    /// The classes of this namespace are extracted from the XmlUnit project. 
    /// </para>
    /// <code>
    /// /*
    /// ******************************************************************
    /// Copyright (c) 2001, Jeff Martin, Tim Bacon
    /// All rights reserved.
    /// 
    /// Redistribution and use in source and binary forms, with or without
    /// modification, are permitted provided that the following conditions
    /// are met:
    /// 
    /// 	* Redistributions of source code must retain the above copyright
    /// 	  notice, this list of conditions and the following disclaimer.
    /// 	* Redistributions in binary form must reproduce the above
    /// 	  copyright notice, this list of conditions and the following
    /// 	  disclaimer in the documentation and/or other materials provided
    /// 	  with the distribution.
    /// 	* Neither the name of the xmlunit.sourceforge.net nor the names
    /// 	  of its contributors may be used to endorse or promote products
    /// 	  derived from this software without specific prior written
    /// 	  permission.
    /// 
    /// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    /// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    /// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
    /// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
    /// COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
    /// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
    /// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
    /// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    /// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
    /// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
    /// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
    /// POSSIBILITY OF SUCH DAMAGE.
    /// 
    /// ******************************************************************
    /// */
    /// </code>
    /// </summary>
    public class XmlDiff {
        private readonly XmlReader _controlReader; 
        private readonly XmlReader _testReader;
        private readonly DiffConfiguration _diffConfiguration;
        private DiffResult _diffResult;
                
        public XmlDiff(XmlInput control, XmlInput test, 
                       DiffConfiguration diffConfiguration) {
            _diffConfiguration =  diffConfiguration;
            _controlReader = CreateXmlReader(control);
            if (control.Equals(test)) {
                _testReader = _controlReader;
            } else {
                _testReader = CreateXmlReader(test);
            }
        }
        
        public XmlDiff(XmlInput control, XmlInput test)
            : this(control, test, new DiffConfiguration()) {
        }

        public XmlDiff(TextReader control, TextReader test)
            : this(new XmlInput(control), new XmlInput(test)) {
        }
        
        public XmlDiff(string control, string test) 
            : this(new XmlInput(control), new XmlInput(test)) {
        }
        
        private XmlReader CreateXmlReader(XmlInput forInput) {
            XmlReader xmlReader = forInput.CreateXmlReader();
        	
        	if (xmlReader is XmlTextReader) {
        		((XmlTextReader) xmlReader ).WhitespaceHandling = _diffConfiguration.WhitespaceHandling;
        	}
            
            if (_diffConfiguration.UseValidatingParser) {
	            XmlValidatingReader validatingReader = new XmlValidatingReader(xmlReader);
	            return validatingReader;
            }
            
            return xmlReader;
        }
        
        public DiffResult Compare() {
            if (_diffResult == null) {
                _diffResult = new DiffResult();
                if (!_controlReader.Equals(_testReader)) {
                    Compare(_diffResult);
                }
            }
            return _diffResult;
        }
        
        private void Compare(DiffResult result) {
            bool controlRead, testRead;
            try {
                do {
                    controlRead = _controlReader.Read();
                    testRead = _testReader.Read();
                	Compare(result, ref controlRead, ref testRead);
                } while (controlRead && testRead) ;
            } catch (FlowControlException e) {       
                Console.Out.WriteLine(e.Message);
				throw;
            }
        }        
        
        private void Compare(DiffResult result, ref bool controlRead, ref bool testRead) {        	
            if (controlRead) {
                if(testRead) {
                    CompareNodes(result);
                    CheckEmptyOrAtEndElement(result, ref controlRead, ref testRead);
                } else {
                    DifferenceFound(DifferenceType.CHILD_NODELIST_LENGTH_ID, result);
                } 
            }
        }
                
        private void CompareNodes(DiffResult result) {
            XmlNodeType controlNodeType = _controlReader.NodeType;
            XmlNodeType testNodeType = _testReader.NodeType;
            if (!controlNodeType.Equals(testNodeType)) {
            	CheckNodeTypes(controlNodeType, testNodeType, result);
            } else if (controlNodeType == XmlNodeType.Element) {
                CompareElements(result);
            } else if (controlNodeType == XmlNodeType.Text) {
                CompareText(result);
            }
        }
        
        private void CheckNodeTypes(XmlNodeType controlNodeType, XmlNodeType testNodeType, DiffResult result) {        
        	XmlReader readerToAdvance = null;
        	if (controlNodeType.Equals(XmlNodeType.XmlDeclaration)) {
        		readerToAdvance = _controlReader;
        	} else if (testNodeType.Equals(XmlNodeType.XmlDeclaration)) {        			
        		readerToAdvance = _testReader;
        	}
        	
        	if (readerToAdvance != null) {
            	DifferenceFound(DifferenceType.HAS_XML_DECLARATION_PREFIX_ID, 
            	                controlNodeType, testNodeType, result);
        		readerToAdvance.Read();
        		CompareNodes(result);
    		} else {
            	DifferenceFound(DifferenceType.NODE_TYPE_ID, controlNodeType, 
             	                testNodeType, result);
    		}       
        }
        
        private void CompareElements(DiffResult result) {
            string controlTagName = _controlReader.Name;
            string testTagName = _testReader.Name;
            if (controlTagName!=testTagName) {
                DifferenceFound(DifferenceType.ElementTagName, result);
            } 
			else 
			{
                int controlAttributeCount = _controlReader.AttributeCount;
                int testAttributeCount = _testReader.AttributeCount;
                if (controlAttributeCount != testAttributeCount) {
                    DifferenceFound(DifferenceType.ELEMENT_NUM_ATTRIBUTES_ID, result);
                } else {
                    CompareAttributes(result, controlAttributeCount);
                }
            }
        }
        
        private void CompareAttributes(DiffResult result, int controlAttributeCount) {
            string controlAttrValue, controlAttrName;
            string testAttrValue, testAttrName;
            
            _controlReader.MoveToFirstAttribute();
            _testReader.MoveToFirstAttribute();
            for (int i=0; i < controlAttributeCount; ++i) {
                
                controlAttrName = _controlReader.Name;
                testAttrName = _testReader.Name;
                
                controlAttrValue = _controlReader.Value;
                testAttrValue = _testReader.Value;
                
                if (controlAttrName!=testAttrName) {
                    DifferenceFound(DifferenceType.AttributeSequence, result);
                
                    if (!_testReader.MoveToAttribute(controlAttrName)) {
                        DifferenceFound(DifferenceType.AttributeNameNotFound, result);
                    }
                    testAttrValue = _testReader.Value;
                }
                
                if (controlAttrValue!=testAttrValue) 
				{
                    DifferenceFound(DifferenceType.AttributeValue, result);
                }
                
                _controlReader.MoveToNextAttribute();
                _testReader.MoveToNextAttribute();
            }
        }
        
        private void CompareText(DiffResult result) {
            string controlText = _controlReader.Value;
            string testText = _testReader.Value;
            if (controlText!=testText) {
                DifferenceFound(DifferenceType.TEXT_VALUE_ID, result);
            }
        }
        
        private void DifferenceFound(DifferenceType differenceType, DiffResult result) {
            DifferenceFound(new Difference(differenceType), result);
        }
        
        private void DifferenceFound(Difference difference, DiffResult result) {
            result.DifferenceFound(this, difference);
            if (!ContinueComparison(difference)) {
                throw new FlowControlException(difference);
            }
        }
        
        private void DifferenceFound(DifferenceType differenceType, 
                                     XmlNodeType controlNodeType,
                                     XmlNodeType testNodeType, 
                                     DiffResult result) {
            DifferenceFound(new Difference(differenceType, controlNodeType, testNodeType),
                            result);
        }
        
        private bool ContinueComparison(Difference afterDifference) {
            return !afterDifference.MajorDifference;
        }
        
        private void CheckEmptyOrAtEndElement(DiffResult result, 
                                              ref bool controlRead, ref bool testRead) {
            if (_controlReader.IsEmptyElement) {
                if (!_testReader.IsEmptyElement) {
                    CheckEndElement(_testReader, ref testRead, result);
                }
            } else {
                if (_testReader.IsEmptyElement) {
                    CheckEndElement(_controlReader, ref controlRead, result);
                }
            }
        }
        
        private void CheckEndElement(XmlReader reader, ref bool readResult, DiffResult result) {            
            readResult = reader.Read();
            if (!readResult || reader.NodeType != XmlNodeType.EndElement) {
                DifferenceFound(DifferenceType.CHILD_NODELIST_LENGTH_ID, result);
            }        
        }
        
        public string OptionalDescription {
            get {
                return _diffConfiguration.Description;
            }
        }
    }
}
