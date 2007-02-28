
namespace MbUnit.Framework.Xml 
{
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
    public enum DifferenceType
	{        
    	/** Comparing an implied attribute value against an explicit value */
    	AttributeValueExplicitlySpecified = 1,
    	
    	/** Comparing 2 elements and one has an attribute the other does not */
    	AttributeNameNotFound = 2,
    	
    	/// <summary>
    	/// Comparing 2 attributes with the same name but different values 
    	/// </summary>
    	AttributeValue = 3,
    	
    	/// <summary>
		///  Comparing 2 attribute lists with the same attributes in different sequence
    	/// </summary>
    	AttributeSequence = 4,
    	
    	/** Comparing 2 CDATA sections with different values */
    	CDATAValue = 5,
    	
    	/** Comparing 2 comments with different values */
    	CommentValue = 6,
    	
    	/** Comparing 2 document types with different names */
    	DOCTYPE_NAME_ID = 7,
    	
    	/** Comparing 2 document types with different public identifiers */
    	DocTypePublicID = 8,
    	
    	/** Comparing 2 document types with different system identifiers */
    	DocTypeSystemID = 9,
    	
    	/** Comparing 2 elements with different tag names */
    	ElementTagName = 10,
    	
    	/** Comparing 2 elements with different number of attributes */
    	ELEMENT_NUM_ATTRIBUTES_ID = 11,
    	
    	/** Comparing 2 processing instructions with different targets */
    	PROCESSING_INSTRUCTION_TARGET_ID = 12,
    	
    	/** Comparing 2 processing instructions with different instructions */
    	PROCESSING_INSTRUCTION_DATA_ID = 13,
    	
    	/** Comparing 2 different text values */
    	TEXT_VALUE_ID = 14,
    	
    	/** Comparing 2 nodes with different namespace prefixes */
    	NAMESPACE_PREFIX_ID = 15,
    	
    	/** Comparing 2 nodes with different namespace URIs */
    	NAMESPACE_URI_ID = 16,
    	
    	/** Comparing 2 nodes with different node types */
    	NODE_TYPE_ID = 17,
    	
    	/** Comparing 2 nodes but only one has any children*/
    	HAS_CHILD_NODES_ID = 18,
    	
    	/** Comparing 2 nodes with different numbers of children */
    	CHILD_NODELIST_LENGTH_ID = 19,
    	
    	/** Comparing 2 nodes with children whose nodes are in different sequence*/
    	CHILD_NODELIST_SEQUENCE_ID = 20,
    	
    	/** Comparing 2 Documents only one of which has a doctype */
    	HAS_DOCTYPE_DECLARATION_ID = 21,
	
    	/** Comparing 2 Documents only one of which has an XML Prefix Declaration */
    	HAS_XML_DECLARATION_PREFIX_ID = 22,
    } ;
}
