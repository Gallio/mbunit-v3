// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

//	Original XmlUnit license
/*
******************************************************************
Copyright (c) 2001, Jeff Martin, Tim Bacon
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.
    * Neither the name of the xmlunit.sourceforge.net nor the names
      of its contributors may be used to endorse or promote products
      derived from this software without specific prior written
      permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.

******************************************************************
*/



namespace MbUnit.Framework.Xml 
{
    /// <summary>
    /// Describes the type of difference found.
    /// </summary>
    public enum DifferenceType
	{
        /// <summary>
        /// Comparing an implied attribute value against an explicit value 
        /// </summary>
    	AttributeValueExplicitlySpecified = 1,
    	
        /// <summary>
        /// Comparing 2 elements and one has an attribute the other does not
        /// </summary>
        AttributeNameNotFound = 2,
    	
    	/// <summary>
    	/// Comparing 2 attributes with the same name but different values
    	/// </summary>
    	AttributeValue = 3,
    	
    	/// <summary>
		/// Comparing 2 attribute lists with the same attributes in different sequence
    	/// </summary>
    	AttributeSequence = 4,
    	
        /// <summary>
        /// Comparing 2 CDATA sections with different values
        /// </summary>
        CDATAValue = 5,
    	
        /// <summary>
        /// Comparing 2 comments with different values
        /// </summary>
        CommentValue = 6,
    	
        /// <summary>
        /// Comparing 2 document types with different names
        /// </summary>
        DOCTYPE_NAME_ID = 7,
    	
        /// <summary>
        /// Comparing 2 document types with different public identifiers
        /// </summary>
        DocTypePublicID = 8,
    	
        /// <summary>
        /// Comparing 2 document types with different system identifiers
        /// </summary>
        DocTypeSystemID = 9,
    	
        /// <summary>
        /// Comparing 2 elements with different tag names
        /// </summary>
        ElementTagName = 10,
    	
        /// <summary>
        /// Comparing 2 elements with different number of attributes
        /// </summary>
        ELEMENT_NUM_ATTRIBUTES_ID = 11,
    	
        /// <summary>
        /// Comparing 2 processing instructions with different targets
        /// </summary>
        PROCESSING_INSTRUCTION_TARGET_ID = 12,
    	
        /// <summary>
        /// Comparing 2 processing instructions with different instructions
        /// </summary>
        PROCESSING_INSTRUCTION_DATA_ID = 13,
    	
        /// <summary>
        /// Comparing 2 different text values
        /// </summary>
        TEXT_VALUE_ID = 14,
    	
        /// <summary>
        /// Comparing 2 nodes with different namespace prefixes
        /// </summary>
        NAMESPACE_PREFIX_ID = 15,
    	
        /// <summary>
        /// Comparing 2 nodes with different namespace URIs
        /// </summary>
        NAMESPACE_URI_ID = 16,
    	
        /// <summary>
        /// Comparing 2 nodes with different node types
        /// </summary>
        NODE_TYPE_ID = 17,
    	
        /// <summary>
        /// Comparing 2 nodes but only one has any children
        /// </summary>
        HAS_CHILD_NODES_ID = 18,
    	
        /// <summary>
        /// Comparing 2 nodes with different numbers of children
        /// </summary>
        CHILD_NODELIST_LENGTH_ID = 19,
    	
        /// <summary>
        /// Comparing 2 nodes with children whose nodes are in different sequence
        /// </summary>
        CHILD_NODELIST_SEQUENCE_ID = 20,
    	
        /// <summary>
        /// Comparing 2 Documents only one of which has a doctype
        /// </summary>
        HAS_DOCTYPE_DECLARATION_ID = 21,
	
        /// <summary>
        /// Comparing 2 Documents only one of which has an XML Prefix Declaration
        /// </summary>
        HAS_XML_DECLARATION_PREFIX_ID = 22,
    }
}
